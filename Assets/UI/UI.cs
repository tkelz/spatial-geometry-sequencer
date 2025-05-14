using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("SGS/Path Transform UIController (UITK)")]
public class PathTransformUIController : MonoBehaviour
{
    [Header("The UI Document that holds your sliders")]
    public UIDocument uiDocument;

    [Header("Game Manager")]
    public GameManager gameManager;

    Button musicOpenBtn;

    Slider beadSpeed;
    DropdownField shapeDropdown;
    Slider posX, posY, posZ;
    Slider rotX, rotY, rotZ;
    Slider size;

    Toggle spatializeToggle;
    Toggle reverbToggle;
    DropdownField reverbDropdown;
    Slider reverbRoomSize, reverbLevel, reverbDelay, reverbReflection, reverbReflectionDelay;

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

        musicOpenBtn = root.Q<Button>("MusicOpenBtn");
        spatializeToggle = root.Q<Toggle>("Spatialize");
        beadSpeed = root.Q<Slider>("BeadSpeed");
        shapeDropdown = root.Q<DropdownField>("ShapeType");
        reverbToggle = root.Q<Toggle>("ReverbToggle");
        reverbDropdown = root.Q<DropdownField>("ReverbPreset");
        reverbRoomSize = root.Q<Slider>("RoomSize");
        reverbLevel = root.Q<Slider>("ReverbLevel");
        reverbDelay = root.Q<Slider>("ReverbDelay");
        reverbReflection = root.Q<Slider>("Reflection");
        reverbReflectionDelay = root.Q<Slider>("ReflectionDelay");

        // Look up sliders by the name attribute in UXML
        posX = root.Q<Slider>("PosX");
        posY = root.Q<Slider>("PosY");
        posZ = root.Q<Slider>("PosZ");

        rotX = root.Q<Slider>("RotX");
        rotY = root.Q<Slider>("RotY");
        rotZ = root.Q<Slider>("RotZ");

        size = root.Q<Slider>("Size");

        // Hook up callbacks
        musicOpenBtn.RegisterCallback<ClickEvent>(evt => gameManager.OpenFile());
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
            gameManager.ChangeShape(evt.newValue);
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

        spatializeToggle.RegisterValueChangedCallback(evt => gameManager.EnableSpatialize(evt.newValue));

        // Reverb
        reverbToggle.RegisterValueChangedCallback(evt => gameManager.EnableAudioReverb(evt.newValue));
        foreach (var preset in System.Enum.GetValues(typeof(AudioReverbPreset)))
        {
            reverbDropdown.choices.Add(preset.ToString());
        }
        reverbDropdown.value = AudioReverbPreset.Cave.ToString();
        reverbDropdown.RegisterValueChangedCallback(evt =>
        {
            AudioReverbPreset preset = (AudioReverbPreset)System.Enum.Parse(typeof(AudioReverbPreset), evt.newValue);
            gameManager.ChangeReverbPreset(preset);

            reverbRoomSize.value = gameManager.audioReverbZone.room;
            reverbLevel.value = gameManager.audioReverbZone.reverb;
            reverbDelay.value = gameManager.audioReverbZone.reverbDelay;
            reverbReflection.value = gameManager.audioReverbZone.reflections;
            reverbReflectionDelay.value = gameManager.audioReverbZone.reflectionsDelay;
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

        shapeDropdown.value = gameManager.activeShapeName;
    }

    public void ChangeMusicName(string name) {
        if(name.Length > 6) {
            musicOpenBtn.text = name.Substring(0, 6) + "...";
        } else {
            musicOpenBtn.text = name;
        }
    }
}