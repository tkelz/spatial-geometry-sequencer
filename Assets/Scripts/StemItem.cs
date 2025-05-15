using UnityEngine;
using UnityEngine.UIElements;

public class StemItem : MonoBehaviour
{
    public BeadMover bead;
    public Transform shapeParent;
    public AudioSource beadAudioSource;
    public FileManager fileManager;

    public Color stemColor;

    public string activeShapeName {private set; get;}
    public string audioName {private set; get;}

    void Awake()
    {
        // Initialize the first shape
        for(int i = 0; i < shapeParent.childCount; i++) {
            shapeParent.GetChild(i).gameObject.SetActive(false);
        }
        shapeParent.GetChild(0).gameObject.SetActive(true);
        activeShapeName = shapeParent.GetChild(0).name;

        audioName = "Example Audio";
    }

    public void SetStemColor(Color newColor) {
        stemColor = newColor;
        bead.pathLine.material.color = stemColor;
    }

    public void SetVisualElements(VisualElement root) {
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
        for(int i = 0; i < shapeParent.childCount; i++) {
            var shape = shapeParent.GetChild(i).gameObject;

            if(shapeName == shape.name) {
                activeShapeName = shapeName;
                shape.SetActive(true);
                bead.pathLine = shape.GetComponentInChildren<LineRenderer>();
                bead.pathLine.material.color = stemColor;
            } else {
                shape.SetActive(false);
            }
        }
    }

    public void ChangeShapePosition(Vector3 newPosition) {
        shapeParent.position = newPosition;
    }

    public void ChangeShapeRotation(Vector3 newRotation) {
        shapeParent.rotation = Quaternion.Euler(newRotation);
    }

    public void ChangeShapeScale(float scale) {
        shapeParent.localScale = Vector3.one * scale;
    }

    public void ChangeBeadSpeed(float speed) {
        bead.bpm = speed;
    }

    public void EnableSpatialize(bool enable) {
        beadAudioSource.spatialize = enable;
    }

    public void OpenFile() {
        fileManager.OpenDialog();
    }

    public void ChangeAudioName(string name) {
        audioName = name;
        StemUIManager.Instance.RefreshItems();
        OptionUIManager.Instance.ChangeMusicName(name);
        // uiManager.ChangeMusicName(name);
    }
}
