using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("SGS/Path Transform UIController (UITK)")]
public class PathTransformUIController : MonoBehaviour
{
    [Header("The UI Document that holds your sliders")]
    public UIDocument uiDocument;

    [Header("Game Manager")]
    public GameManager gameManager;

    // Runtime references to our sliders
    Slider beadSpeed;
    DropdownField shapeDropdown;
    Slider posX, posY, posZ;
    Slider rotX, rotY, rotZ;
    Slider size;

    Toggle reverbToggle;

    void OnEnable()
    {
        if (uiDocument == null || gameManager == null)
        {
            Debug.LogError("PathTransformUIController: Assign both a UIDocument and GameManager!");
            enabled = false;
            return;
        }

        // Grab the root VisualElement
        var root = uiDocument.rootVisualElement;

        beadSpeed = root.Q<Slider>("BeadSpeed");
        shapeDropdown = root.Q<DropdownField>("ShapeType");
        reverbToggle = root.Q<Toggle>("ReverbToggle");

        // Look up sliders by the name attribute in UXML
        posX = root.Q<Slider>("PosX");
        posY = root.Q<Slider>("PosY");
        posZ = root.Q<Slider>("PosZ");

        rotX = root.Q<Slider>("RotX");
        rotY = root.Q<Slider>("RotY");
        rotZ = root.Q<Slider>("RotZ");

        size = root.Q<Slider>("Size");

        // Hook up callbacks
        beadSpeed.RegisterValueChangedCallback(evt =>
        {
            // Change the speed of the bead
            gameManager.ChangeBeadSpeed(evt.newValue);
            // Update the slider label
            beadSpeed.label = $"Bead Speed: {evt.newValue:F2}";
        });
        shapeDropdown.RegisterValueChangedCallback(evt =>
        {
            // Change the shape of the path
            gameManager.ChangeShape((evt.newValue));
        });
        reverbToggle.RegisterValueChangedCallback(evt =>
        {
            // Enable or disable the audio reverb
            gameManager.EnableAudioReverb(evt.newValue);
        });

        posX.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapePosition(new Vector3(evt.newValue, posY.value, posZ.value));
            posX.label = $"Pos X: {evt.newValue:F2}";
        });
        posY.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapePosition(new Vector3(posX.value, evt.newValue, posZ.value));
            posY.label = $"Pos Y: {evt.newValue:F2}";
        });
        posZ.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapePosition(new Vector3(posX.value, posY.value, evt.newValue));
            posZ.label = $"Pos Z: {evt.newValue:F2}";
        });

        rotX.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapeRotation(new Vector3(evt.newValue, rotY.value, rotZ.value));
            rotX.label = $"Rot X: {evt.newValue:F1}";
        });
        rotY.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapeRotation(new Vector3(rotX.value, evt.newValue, rotZ.value));
            rotY.label = $"Rot Y: {evt.newValue:F1}";
        });
        rotZ.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapeRotation(new Vector3(rotX.value, rotY.value, evt.newValue));
            rotZ.label = $"Rot Z: {evt.newValue:F1}";
        });

        // Update the size slider callback with clamping and incremental scaling
        size.RegisterValueChangedCallback(evt =>
        {
            gameManager.ChangeShapeScale(evt.newValue);
            // Update the slider label
            size.label = $"Size: {evt.newValue:F2}";
        });

        // Initialize UI to match current transform
        RefreshUI();
    }

    /// <summary>
    /// Pull the current transform values into the sliders.
    /// </summary>
    public void RefreshUI()
    {
        var p = gameManager.shapeParent.position;
        posX.value = p.x; posX.label = $"Pos X: {p.x:F2}";
        posY.value = p.y; posY.label = $"Pos Y: {p.y:F2}";
        posZ.value = p.z; posZ.label = $"Pos Z: {p.z:F2}";

        var r = gameManager.shapeParent.localEulerAngles;
        rotX.value = r.x; rotX.label = $"Rot X: {r.x:F1}";
        rotY.value = r.y; rotY.label = $"Rot Y: {r.y:F1}";
        rotZ.value = r.z; rotZ.label = $"Rot Z: {r.z:F1}";

        float s = gameManager.shapeParent.localScale.x;
        size.value = s; size.label = $"Size: {s:F2}";

        print($"Current shape: {gameManager.activeShapeName}");
        shapeDropdown.value = gameManager.activeShapeName;
    }
}