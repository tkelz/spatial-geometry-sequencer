using UnityEngine;
using UnityEngine.UIElements;

[AddComponentMenu("SGS/Option UI Manager (OUM)")]
public class OptionUIManager : MonoBehaviour
{
    public static OptionUIManager Instance;

    [Header("The UI Document that holds your sliders")]
    public UIDocument uiDocument;

    Button musicOpenBtn;

    GroupBox stemOptions;
    SliderInt beadSpeed, beadOffset;
    DropdownField shapeDropdown;
    Slider posX, posY, posZ;
    Slider rotX, rotY, rotZ;
    Slider size;

    Toggle spatializeToggle;
    Toggle reverbToggle;
    DropdownField reverbDropdown;
    SliderInt reverbRoomSize, reverbLevel, reverbReflection;
    Slider reverbDelay, reverbReflectionDelay;

    ProgressBar recordingProgressBar;

    StemItem stemItem;

    // Torus Options
    SliderInt torusP, torusQ, torusSegments;
    Slider torusRadius, torusTube;
    // Spiral Options
    Slider spiralTurns, spiralSpacing, spiralStartWidth, spiralEndWidth;
    SliderInt spiralPointsPerTurn;
    // Circle Options
    Slider circleRadius;
    SliderInt circlePointCount;
    // Line Options
    Slider lineLength, lineAmplitude;
    SliderInt lineWaveCount, linePointsPerWave;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (uiDocument == null)
        {
            Debug.LogError("OptionUIManager: Assign both a UIDocument!");
            enabled = false;
            return;
        }

        // Grab the root VisualElement
        var root = uiDocument.rootVisualElement;
        stemOptions = root.Q<GroupBox>("StemOptions");

        musicOpenBtn = root.Q<Button>("MusicOpenBtn");
        spatializeToggle = root.Q<Toggle>("Spatialize");
        beadSpeed = root.Q<SliderInt>("BeadSpeed");
        beadOffset = root.Q<SliderInt>("BeadOffset");
        shapeDropdown = root.Q<DropdownField>("ShapeType");
        reverbToggle = root.Q<Toggle>("ReverbToggle");
        reverbDropdown = root.Q<DropdownField>("ReverbPreset");
        reverbRoomSize = root.Q<SliderInt>("RoomSize");
        reverbLevel = root.Q<SliderInt>("ReverbLevel");
        reverbDelay = root.Q<Slider>("ReverbDelay");
        reverbReflection = root.Q<SliderInt>("Reflection");
        reverbReflectionDelay = root.Q<Slider>("ReflectionDelay");

        // Look up sliders by the name attribute in UXML
        posX = root.Q<Slider>("PosX");
        posY = root.Q<Slider>("PosY");
        posZ = root.Q<Slider>("PosZ");

        rotX = root.Q<Slider>("RotX");
        rotY = root.Q<Slider>("RotY");
        rotZ = root.Q<Slider>("RotZ");

        size = root.Q<Slider>("Size");

        // Torus options
        torusP = root.Q<SliderInt>("TorusP");
        torusQ = root.Q<SliderInt>("TorusQ");
        torusSegments = root.Q<SliderInt>("TorusSegments");
        torusRadius = root.Q<Slider>("TorusRadius");
        torusTube = root.Q<Slider>("TorusTube");
        // Spiral options
        spiralTurns = root.Q<Slider>("SpiralTurns");
        spiralSpacing = root.Q<Slider>("SpiralSpacing");
        spiralStartWidth = root.Q<Slider>("SpiralStartWidth");
        spiralEndWidth = root.Q<Slider>("SpiralEndWidth");
        spiralPointsPerTurn = root.Q<SliderInt>("SpiralPointsPerTurn");
        // Circle options
        circleRadius = root.Q<Slider>("RingRadius");
        circlePointCount = root.Q<SliderInt>("RingPointCount");
        // Line options
        lineLength = root.Q<Slider>("LineLength");
        lineAmplitude = root.Q<Slider>("LineAmplitude");
        lineWaveCount = root.Q<SliderInt>("LineWaveCount");
        linePointsPerWave = root.Q<SliderInt>("LinePointsPerWave");

        // Export Options
        recordingProgressBar = root.Q<ProgressBar>("RecordingProgress");

        // Hook up callbacks
        musicOpenBtn.RegisterCallback<ClickEvent>(evt => stemItem.OpenDialog());
        beadSpeed.RegisterValueChangedCallback(evt =>
        {
            // Change the speed of the bead
            stemItem.ChangeBeadSpeed(evt.newValue);
            beadSpeed.label = $"BPM: {evt.newValue:F2}";
        });
        beadOffset.RegisterValueChangedCallback(evt =>
        {
            // Change the offset of the bead
            stemItem.ChangeBeadOffset(evt.newValue);
            beadOffset.label = $"Offset: {evt.newValue}";
        });
        shapeDropdown.RegisterValueChangedCallback(evt =>
        {
            // Change the shape of the path
            stemItem.ChangeShape(evt.newValue);

            // Update the shape dropdown label
            UpdatePathOptions();
        });
        torusP.RegisterValueChangedCallback(evt =>
        {
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusKnotGenerator.p = evt.newValue;
            torusKnotGenerator.Generate();
            torusP.label = $"P: {evt.newValue}";
        });
        torusQ.RegisterValueChangedCallback(evt =>
        {
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusKnotGenerator.q = evt.newValue;
            torusKnotGenerator.Generate();
            torusQ.label = $"Q: {evt.newValue}";
        });
        torusSegments.RegisterValueChangedCallback(evt =>
        {
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusKnotGenerator.segments = evt.newValue;
            torusKnotGenerator.Generate();
            torusSegments.label = $"Segments: {evt.newValue}";
        });
        torusRadius.RegisterValueChangedCallback(evt =>
        {
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusKnotGenerator.radius = evt.newValue;
            torusKnotGenerator.Generate();
            torusRadius.label = $"Radius: {evt.newValue:F2}";
        });
        torusTube.RegisterValueChangedCallback(evt =>
        {
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusKnotGenerator.tube = evt.newValue;
            torusKnotGenerator.Generate();
            torusTube.label = $"Tube: {evt.newValue:F2}";
        });
        spiralTurns.RegisterValueChangedCallback(evt =>
        {
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiral.turns = evt.newValue;
            spiral.Regenerate();
            spiralTurns.label = $"Turns: {evt.newValue:F2}";
        });
        spiralSpacing.RegisterValueChangedCallback(evt =>
        {
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiral.spacing = evt.newValue;
            spiral.Regenerate();
            spiralSpacing.label = $"Spacing: {evt.newValue:F2}";
        });
        spiralStartWidth.RegisterValueChangedCallback(evt =>
        {
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiral.widthStart = evt.newValue;
            spiral.Regenerate();
            spiralStartWidth.label = $"Start Width: {evt.newValue:F2}";
        });
        spiralEndWidth.RegisterValueChangedCallback(evt =>
        {
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiral.widthEnd = evt.newValue;
            spiral.Regenerate();
            spiralEndWidth.label = $"End Width: {evt.newValue:F2}";
        });
        spiralPointsPerTurn.RegisterValueChangedCallback(evt =>
        {
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiral.pointsPerTurn = evt.newValue;
            spiral.Regenerate();
            spiralPointsPerTurn.label = $"Points Per Turn: {evt.newValue}";
        });
        circleRadius.RegisterValueChangedCallback(evt =>
        {
            CirclePathGenerator circle = stemItem.shapeParent.GetComponentInChildren<CirclePathGenerator>();
            circle.radius = evt.newValue;
            circle.Generate();
            circleRadius.label = $"Radius: {evt.newValue:F2}";
        });
        circlePointCount.RegisterValueChangedCallback(evt =>
        {
            CirclePathGenerator circle = stemItem.shapeParent.GetComponentInChildren<CirclePathGenerator>();
            circle.pointCount = evt.newValue;
            circle.Generate();
            circlePointCount.label = $"Point Count: {evt.newValue}";
        });
        lineLength.RegisterValueChangedCallback(evt =>
        {
            WaveLineGenerator linePath = stemItem.shapeParent.GetComponentInChildren<WaveLineGenerator>();
            linePath.length = evt.newValue;
            linePath.Generate();
            lineLength.label = $"Length: {evt.newValue:F2}";
        });
        lineAmplitude.RegisterValueChangedCallback(evt =>
        {
            WaveLineGenerator linePath = stemItem.shapeParent.GetComponentInChildren<WaveLineGenerator>();
            linePath.amplitude = evt.newValue;
            linePath.Generate();
            lineAmplitude.label = $"Amplitude: {evt.newValue:F2}";
        });
        lineWaveCount.RegisterValueChangedCallback(evt =>
        {
            WaveLineGenerator linePath = stemItem.shapeParent.GetComponentInChildren<WaveLineGenerator>();
            linePath.waveCount = evt.newValue;
            linePath.Generate();
            lineWaveCount.label = $"Wave Count: {evt.newValue}";
        });
        linePointsPerWave.RegisterValueChangedCallback(evt =>
        {
            WaveLineGenerator linePath = stemItem.shapeParent.GetComponentInChildren<WaveLineGenerator>();
            linePath.pointsPerWave = evt.newValue;
            linePath.Generate();
            linePointsPerWave.label = $"Points Per Wave: {evt.newValue}";
        });

        posX.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapePosition(new Vector3(evt.newValue, posY.value, posZ.value));
            posX.label = $"Pos X: {evt.newValue:F2}";
        });
        posY.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapePosition(new Vector3(posX.value, evt.newValue, posZ.value));
            posY.label = $"Pos Y: {evt.newValue:F2}";
        });
        posZ.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapePosition(new Vector3(posX.value, posY.value, evt.newValue));
            posZ.label = $"Pos Z: {evt.newValue:F2}";
        });

        rotX.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapeRotation(new Vector3(evt.newValue, rotY.value, rotZ.value));
            rotX.label = $"Rot X: {evt.newValue:F1}";
        });
        rotY.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapeRotation(new Vector3(rotX.value, evt.newValue, rotZ.value));
            rotY.label = $"Rot Y: {evt.newValue:F1}";
        });
        rotZ.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapeRotation(new Vector3(rotX.value, rotY.value, evt.newValue));
            rotZ.label = $"Rot Z: {evt.newValue:F1}";
        });

        // Update the size slider callback with clamping and incremental scaling
        size.RegisterValueChangedCallback(evt =>
        {
            stemItem.ChangeShapeScale(evt.newValue);
            // Update the slider label
            size.label = $"Size: {evt.newValue:F2}";
        });

        spatializeToggle.RegisterValueChangedCallback(evt => stemItem.EnableSpatialize(evt.newValue));

        // Reverb
        reverbToggle.RegisterValueChangedCallback(evt => StemManager.Instance.EnableAudioReverb(evt.newValue));
        foreach (var preset in System.Enum.GetValues(typeof(AudioReverbPreset)))
        {
            reverbDropdown.choices.Add(preset.ToString());
        }
        reverbDropdown.value = AudioReverbPreset.Bathroom.ToString();
        reverbDropdown.RegisterValueChangedCallback(evt =>
        {
            AudioReverbPreset preset = (AudioReverbPreset)System.Enum.Parse(typeof(AudioReverbPreset), evt.newValue);
            StemManager.Instance.ChangeReverbPreset(preset);

            reverbRoomSize.value = StemManager.Instance.audioReverbZone.room;
            reverbLevel.value = StemManager.Instance.audioReverbZone.reverb;
            reverbDelay.value = StemManager.Instance.audioReverbZone.reverbDelay;
            reverbReflection.value = StemManager.Instance.audioReverbZone.reflections;
            reverbReflectionDelay.value = StemManager.Instance.audioReverbZone.reflectionsDelay;

            StemManager.Instance.ChangeReverbPreset(AudioReverbPreset.User);
        });
        reverbDropdown.value = AudioReverbPreset.Bathroom.ToString();
        reverbRoomSize.RegisterValueChangedCallback(evt => StemManager.Instance.SetReverbRoomSize(evt.newValue));
        reverbLevel.RegisterValueChangedCallback(evt => StemManager.Instance.SetReverbLevel(evt.newValue));
        reverbDelay.RegisterValueChangedCallback(evt => StemManager.Instance.SetReverbDelay(evt.newValue));
        reverbReflection.RegisterValueChangedCallback(evt => StemManager.Instance.SetReverbReflections(evt.newValue));
        reverbReflectionDelay.RegisterValueChangedCallback(evt => StemManager.Instance.SetReverbReflectionsDelay(evt.newValue));

        // Export
        recordingProgressBar.value = 0;
    }

    public void EnableStemOptions(bool enabled)
    {
        stemOptions.SetEnabled(enabled);
    }

    public void SetStemData(StemItem stemItem)
    {
        if (!stemItem)
        {
            EnableStemOptions(false);
            return;
        }

        EnableStemOptions(!StemManager.Instance.isPlaying);

        this.stemItem = stemItem;
        var p = stemItem.shapeParent.position;
        posX.value = p.x; posX.label = $"Pos X: {p.x:F2}";
        posY.value = p.y; posY.label = $"Pos Y: {p.y:F2}";
        posZ.value = p.z; posZ.label = $"Pos Z: {p.z:F2}";

        var r = stemItem.shapeParent.localEulerAngles;
        rotX.value = r.x; rotX.label = $"Rot X: {r.x:F1}";
        rotY.value = r.y; rotY.label = $"Rot Y: {r.y:F1}";
        rotZ.value = r.z; rotZ.label = $"Rot Z: {r.z:F1}";

        float s = stemItem.shapeParent.localScale.x;
        size.value = s; size.label = $"Size: {s:F2}";

        shapeDropdown.SetValueWithoutNotify(stemItem.activeShapeName);

        ChangeMusicName(stemItem.audioName);

        spatializeToggle.SetValueWithoutNotify(stemItem.beadAudioSource.spatialize);

        beadSpeed.SetValueWithoutNotify((int)stemItem.bead.bpm);
        beadSpeed.label = $"BPM: {stemItem.bead.bpm:F2}";
        beadOffset.SetValueWithoutNotify(stemItem.bead.offset);
        beadOffset.label = $"Offset: {stemItem.bead.offset}";

        UpdatePathOptions();
    }

    void UpdatePathOptions()
    {
        // Show the new shape options
        var root = uiDocument.rootVisualElement;
        root.Q<TemplateContainer>("TorusOption").style.display = DisplayStyle.None;
        root.Q<TemplateContainer>("SpiralOption").style.display = DisplayStyle.None;
        root.Q<TemplateContainer>("LineOption").style.display = DisplayStyle.None;
        root.Q<TemplateContainer>("RingOption").style.display = DisplayStyle.None;

        if (shapeDropdown.value == "Torus")
        {
            root.Q<TemplateContainer>("TorusOption").style.display = DisplayStyle.Flex;
            TorusKnotGenerator torusKnotGenerator = stemItem.shapeParent.GetComponentInChildren<TorusKnotGenerator>();
            torusP.value = torusKnotGenerator.p;
            torusQ.value = torusKnotGenerator.q;
            torusSegments.value = torusKnotGenerator.segments;
            torusRadius.value = torusKnotGenerator.radius;
            torusTube.value = torusKnotGenerator.tube;

            torusP.label = $"P: {torusKnotGenerator.p}";
            torusQ.label = $"Q: {torusKnotGenerator.q}";
            torusSegments.label = $"Segments: {torusKnotGenerator.segments}";
            torusRadius.label = $"Radius: {torusKnotGenerator.radius:F2}";
            torusTube.label = $"Tube: {torusKnotGenerator.tube:F2}";
        }
        else if (shapeDropdown.value == "Spiral")
        {
            root.Q<TemplateContainer>("SpiralOption").style.display = DisplayStyle.Flex;
            SpiralGenerator spiral = stemItem.shapeParent.GetComponentInChildren<SpiralGenerator>();
            spiralTurns.value = spiral.turns;
            spiralSpacing.value = spiral.spacing;
            spiralStartWidth.value = spiral.widthStart;
            spiralEndWidth.value = spiral.widthEnd;
            spiralPointsPerTurn.value = spiral.pointsPerTurn;

            spiralTurns.label = $"Turns: {spiral.turns:F2}";
            spiralSpacing.label = $"Spacing: {spiral.spacing:F2}";
            spiralStartWidth.label = $"Start Width: {spiral.widthStart:F2}";
            spiralEndWidth.label = $"End Width: {spiral.widthEnd:F2}";
            spiralPointsPerTurn.label = $"Points Per Turn: {spiral.pointsPerTurn}";
        }
        else if (shapeDropdown.value == "Line")
        {
            root.Q<TemplateContainer>("LineOption").style.display = DisplayStyle.Flex;
            WaveLineGenerator linePath = stemItem.shapeParent.GetComponentInChildren<WaveLineGenerator>();
            lineLength.value = linePath.length;
            lineAmplitude.value = linePath.amplitude;
            lineWaveCount.value = linePath.waveCount;
            linePointsPerWave.value = linePath.pointsPerWave;

            lineLength.label = $"Length: {linePath.length:F2}";
            lineAmplitude.label = $"Amplitude: {linePath.amplitude:F2}";
            lineWaveCount.label = $"Wave Count: {linePath.waveCount}";
            linePointsPerWave.label = $"Points Per Wave: {linePath.pointsPerWave}";
        }
        else if (shapeDropdown.value == "Ring")
        {
            root.Q<TemplateContainer>("RingOption").style.display = DisplayStyle.Flex;
            CirclePathGenerator circle = stemItem.shapeParent.GetComponentInChildren<CirclePathGenerator>();
            circleRadius.value = circle.radius;
            circlePointCount.value = circle.pointCount;

            circleRadius.label = $"Radius: {circle.radius:F2}";
            circlePointCount.label = $"Point Count: {circle.pointCount}";
        }
    }

    public void UpdateReverbSettings()
    {
        reverbToggle.value = StemManager.Instance.audioReverbZone.enabled;
        reverbDropdown.value = StemManager.Instance.audioReverbZone.reverbPreset.ToString();
        reverbRoomSize.value = StemManager.Instance.audioReverbZone.room;
        reverbLevel.value = StemManager.Instance.audioReverbZone.reverb;
        reverbDelay.value = StemManager.Instance.audioReverbZone.reverbDelay;
        reverbReflection.value = StemManager.Instance.audioReverbZone.reflections;
        reverbReflectionDelay.value = StemManager.Instance.audioReverbZone.reflectionsDelay;
    }

    public void ChangeMusicName(string name)
    {
        if (name.Length > 13)
        {
            musicOpenBtn.text = name.Substring(0, 13) + "...";
        }
        else
        {
            musicOpenBtn.text = name;
        }
    }

    public void SetRecordProgressBar(float progress)
    {
        recordingProgressBar.value = progress;
        recordingProgressBar.title = $"Export Progress ({progress:F2} %)";
    }
}