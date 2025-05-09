using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("SGS/Path Transform UIController (UITK)")]
public class PathTransformUIController : MonoBehaviour
{
    [Header("The UI Document that holds your sliders")]
    public UIDocument uiDocument;

    [Header("The object you want to drive")]
    public Transform target;

    // Runtime references to our sliders
    Slider posX, posY, posZ;
    Slider rotX, rotY, rotZ;
    Slider size;

    void OnEnable()
    {
        if (uiDocument == null || target == null)
        {
            Debug.LogError("PathTransformUIController: Assign both a UIDocument and a target Transform!");
            enabled = false;
            return;
        }

        // Grab the root VisualElement
        var root = uiDocument.rootVisualElement;

        // Look up sliders by the name attribute in UXML
        posX = root.Q<Slider>("PosX");
        posY = root.Q<Slider>("PosY");
        posZ = root.Q<Slider>("PosZ");

        rotX = root.Q<Slider>("RotX");
        rotY = root.Q<Slider>("RotY");
        rotZ = root.Q<Slider>("RotZ");

        size = root.Q<Slider>("Size");

        // Hook up callbacks
        posX.RegisterValueChangedCallback(evt =>
        {
            var p = target.localPosition;
            p.x = evt.newValue;
            target.localPosition = p;
            posX.label = $"Pos X: {evt.newValue:F2}";
        });
        posY.RegisterValueChangedCallback(evt =>
        {
            var p = target.localPosition;
            p.y = evt.newValue;
            target.localPosition = p;
            posY.label = $"Pos Y: {evt.newValue:F2}";
        });
        posZ.RegisterValueChangedCallback(evt =>
        {
            var p = target.localPosition;
            p.z = evt.newValue;
            target.localPosition = p;
            posZ.label = $"Pos Z: {evt.newValue:F2}";
        });

        rotX.RegisterValueChangedCallback(evt =>
        {
            var r = target.localEulerAngles;
            r.x = evt.newValue;
            target.localEulerAngles = r;
            rotX.label = $"Rot X: {evt.newValue:F1}";
        });
        rotY.RegisterValueChangedCallback(evt =>
        {
            var r = target.localEulerAngles;
            r.y = evt.newValue;
            target.localEulerAngles = r;
            rotY.label = $"Rot Y: {evt.newValue:F1}";
        });
        rotZ.RegisterValueChangedCallback(evt =>
        {
            var r = target.localEulerAngles;
            r.z = evt.newValue;
            target.localEulerAngles = r;
            rotZ.label = $"Rot Z: {evt.newValue:F1}";
        });

        // Update the size slider callback with clamping and incremental scaling
        size.RegisterValueChangedCallback(evt =>
        {
            // Clamp the slider value to a reasonable range (e.g., 0.1 to 5.0)
            float clampedValue = Mathf.Clamp(evt.newValue, 0.1f, 5.0f);

            // Apply uniform scaling based on the clamped value
            float scaleFactor = clampedValue / target.localScale.x;
            target.localScale *= scaleFactor;

            // Update the slider label
            size.label = $"Size: {clampedValue:F2}";
        });

        // Initialize UI to match current transform
        RefreshUI();
    }

    /// <summary>
    /// Pull the current transform values into the sliders.
    /// </summary>
    public void RefreshUI()
    {
        var p = target.localPosition;
        posX.value = p.x; posX.label = $"Pos X: {p.x:F2}";
        posY.value = p.y; posY.label = $"Pos Y: {p.y:F2}";
        posZ.value = p.z; posZ.label = $"Pos Z: {p.z:F2}";

        var r = target.localEulerAngles;
        rotX.value = r.x; rotX.label = $"Rot X: {r.x:F1}";
        rotY.value = r.y; rotY.label = $"Rot Y: {r.y:F1}";
        rotZ.value = r.z; rotZ.label = $"Rot Z: {r.z:F1}";

        float s = target.localScale.x;
        size.value = s; size.label = $"Size: {s:F2}";
    }
}