using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Shape Settings")]
    public Transform shapeParent;
    public GameObject[] shapes;

    [Header("Bead Settings")]
    public BeadMover beadMover;
    public AudioSource beadAudio;

    [Header("Audio Settings")]
    public AudioReverbZone audioReverbZone;

    [Header("File Dialog Settings")]
    public FileManager fileManager;

    [Header("UI Settings")]
    public PathTransformUIController uiManager;

    public string activeShapeName {set{ _activeShapeName = value;} get { return _activeShapeName;}}

    string _activeShapeName = "Torus";

    void Awake()
    {
        // Initialize the first shape
        foreach (GameObject shape in shapes) {
            shape.SetActive(false);
        }
        shapes[0].SetActive(true);
        activeShapeName = shapes[0].name;
    }

    public void ChangeShape(string shapeName)
    {
        // Hide all shapes
        foreach (GameObject shape in shapes)
        {
            if(shapeName == shape.name) {
                activeShapeName = shapeName;
                shape.SetActive(true);
                beadMover.pathLine = shape.GetComponentInChildren<LineRenderer>();
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
        beadMover.bpm = speed;
    }

    public void EnableAudioReverb(bool enable)
    {
        audioReverbZone.enabled = enable;
    }

    public void ChangeReverbPreset(AudioReverbPreset preset)
    {
        audioReverbZone.reverbPreset = preset;
    }

    public void EnableSpatialize(bool enable) {
        beadAudio.spatialize = enable;
    }

    public void OpenFile() {
        fileManager.OpenDialog();
    }

    public void ChangeAudioName(string name) {
        uiManager.ChangeMusicName(name);
    }
}
