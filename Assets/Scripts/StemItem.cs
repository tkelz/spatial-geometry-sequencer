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
    public Light spotLight;
    public SkinnedMeshRenderer fairyMesh;

    public Color stemColor;

    public string activeShapeName { private set; get; }
    public string audioName { private set; get; }
    public string audioUrl { private set; get; }

    AudioClip audioClip;

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
        LoadAudio(audioUrl);
        if (StemManager.Instance.isPlaying)
        {
            StartStem();
        }
    }

    public void StartStem()
    {
        beadAudioSource.Play();
        bead.ResetToStart();
        bead.isMoving = true;
    }

    public void PauseStem()
    {
        beadAudioSource.Stop();
        bead.ResetToStart();
    }

    public void Restart()
    {
        beadAudioSource.PlayOneShot(audioClip);
        bead.ResetToStart();
    }

    public void SetStemColor(Color newColor)
    {
        stemColor = newColor;
        bead.pathLine.material.color = stemColor;
        spotLight.color = stemColor;
        foreach (var material in fairyMesh.materials)
        {
            material.color = stemColor;
            float intensity = Mathf.Pow(2, 5);
            material.SetColor("_EmissionColor", stemColor * intensity);
        }
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
                bead.pathLine.material.color = new Color(stemColor.r, stemColor.g, stemColor.b, 0.5f);
                bead.ResetToStart();
                bead.isMoving = StemManager.Instance.isPlaying;
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

    public void ChangeBeadOffset(int offset)
    {
        bead.SetOffset(offset);
        // bead.ResetToStart();
    }

    public void EnableSpatialize(bool enable)
    {
        beadAudioSource.spatialize = enable;
    }

    public void EnableAudio(bool enable)
    {
        beadAudioSource.mute = !enable;
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

    public void LoadAudio(string url)
    {
        if (audioName == "Example Audio")
        {
            audioClip = Resources.Load<AudioClip>(audioUrl);
            beadAudioSource.clip = audioClip;
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
        audioClip = loader.GetAudioClip(false, false);
        beadAudioSource.clip = audioClip;
        audioUrl = url;
        ChangeAudioName(Path.GetFileNameWithoutExtension(url));
    }

    public void SetAudioUrl(string url)
    {
        audioUrl = url;
    }

    public void ChangeAudioName(string name)
    {
        audioName = name;
        StemUIManager.Instance.RefreshItems();
        OptionUIManager.Instance.ChangeMusicName(name);
        // uiManager.ChangeMusicName(name);
    }
}
