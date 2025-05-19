using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using SFB;

public class StemItem : MonoBehaviour
{
    public BeadMover bead;
    public Transform shapeParent;
    public AudioSource beadAudioSource;
    public FileManager fileManager;

    public Color stemColor;

    public string activeShapeName { private set; get; }
    public string audioName { private set; get; }
    public string audioUrl { private set; get; }

    void Awake()
    {
        // Initialize the first shape
        for (int i = 0; i < shapeParent.childCount; i++)
        {
            shapeParent.GetChild(i).gameObject.SetActive(false);
        }
        shapeParent.GetChild(0).gameObject.SetActive(true);
        activeShapeName = shapeParent.GetChild(0).name;

        audioUrl = "Audio/example_audio";
        audioName = "Example Audio";
    }

    void Start()
    {
        print("Audio Info:");
        print(beadAudioSource.clip.samples);
        print(beadAudioSource.clip.channels);
        print(beadAudioSource.clip.frequency);
        print(beadAudioSource.clip.length);
    }

    public void SetStemColor(Color newColor)
    {
        stemColor = newColor;
        bead.pathLine.material.color = stemColor;
    }

    public void SetVisualElements(VisualElement root)
    {
        // Set the color of the stem in the UI
        var stemName = root.Q<Label>("StemName");
        stemName.text = gameObject.name;
        var stemTrack = root.Q<VisualElement>("StemTrack");
        stemTrack.style.backgroundColor = new Color(stemColor.r, stemColor.g, stemColor.b, 0.5f);
        var audioLabel = root.Q<Label>("AudioName");
        audioLabel.text = audioName;
    }

    public void ChangeShape(string shapeName)
    {
        // Hide all shapes
        for (int i = 0; i < shapeParent.childCount; i++)
        {
            var shape = shapeParent.GetChild(i).gameObject;

            if (shapeName == shape.name)
            {
                activeShapeName = shapeName;
                shape.SetActive(true);
                bead.pathLine = shape.GetComponentInChildren<LineRenderer>();
                bead.pathLine.material.color = stemColor;
            }
            else
            {
                shape.SetActive(false);
            }
        }
    }

    public void ChangeShapePosition(Vector3 newPosition)
    {
        shapeParent.position = newPosition;
    }

    public void ChangeShapeRotation(Vector3 newRotation)
    {
        shapeParent.rotation = Quaternion.Euler(newRotation);
    }

    public void ChangeShapeScale(float scale)
    {
        shapeParent.localScale = Vector3.one * scale;
    }

    public void ChangeBeadSpeed(float speed)
    {
        bead.bpm = speed;
    }

    public void EnableSpatialize(bool enable)
    {
        beadAudioSource.spatialize = enable;
    }

    public void OpenDialog()
    {
        var extensions = new[] {
            // new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            new ExtensionFilter("Sound Files", "mp3", "wav" ),
            // new ExtensionFilter("All Files", "*" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Audio File", "", extensions, false);
        if (paths.Length > 0)
        {
            StartCoroutine(LoadAndPlay(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    public void LoadAudioSession(string url)
    {
        if (audioName == "Example Audio")
        {
            print(audioUrl);
            var audioClip = Resources.Load<AudioClip>(audioUrl);
            beadAudioSource.clip = audioClip;
            beadAudioSource.Play();
        }
        else
        {
            StartCoroutine(LoadAndPlay(url));
        }
    }

    IEnumerator LoadAndPlay(string url)
    {
        var loader = new WWW(url);
        yield return loader;
        beadAudioSource.clip = loader.GetAudioClip(false, false);
        beadAudioSource.Play();
        audioUrl = url;
        ChangeAudioName(Path.GetFileNameWithoutExtension(url));
    }

    public void ChangeAudioName(string name)
    {
        audioName = name;
        StemUIManager.Instance.RefreshItems();
        OptionUIManager.Instance.ChangeMusicName(name);
        // uiManager.ChangeMusicName(name);
    }
}
