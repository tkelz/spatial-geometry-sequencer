using System.IO;
using UnityEngine;
using SFB;
using System;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSession()
    {
        SessionData sessionData = new SessionData();

        // Save stem data
        sessionData.stems = new StemData[StemManager.Instance.stems.Count];
        for (int i = 0; i < StemManager.Instance.stems.Count; i++)
        {
            var stem = StemManager.Instance.stems[i];
            var torus = stem.GetComponentInChildren<TorusKnotGenerator>(true);
            var spiral = stem.GetComponentInChildren<SpiralGenerator>(true);
            var line = stem.GetComponentInChildren<WaveLineGenerator>(true);
            var circle = stem.GetComponentInChildren<CirclePathGenerator>(true);
            sessionData.stems[i] = new StemData
            {
                name = stem.gameObject.name,
                color = stem.stemColor,
                activeShapeName = stem.activeShapeName,
                audioName = stem.audioName,
                audioUrl = stem.audioUrl,
                position = stem.shapeParent.position,
                rotation = stem.shapeParent.rotation.eulerAngles,
                scale = stem.shapeParent.localScale.x,
                bpm = stem.bead.bpm,
                offset = stem.bead.offset,
                spatialize = stem.beadAudioSource.spatialize,

                torusP = torus.p,
                torusQ = torus.q,
                torusSegments = torus.segments,
                torusRadius = torus.radius,
                torusTube = torus.tube,

                spiralTurns = spiral.turns,
                spiralSpacing = spiral.spacing,
                spiralWidthStart = spiral.widthStart,
                spiralWidthEnd = spiral.widthEnd,
                spiralPointsPerTurn = spiral.pointsPerTurn,

                lineLength = line.length,
                lineAmplitude = line.amplitude,
                lineWaveCount = line.waveCount,
                linePointsPerWave = line.pointsPerWave,

                circleRadius = circle.radius,
                circlePointCount = circle.pointCount
            };
        }

        // Save reverb zone data
        ReverbZoneData reverbZoneData = new ReverbZoneData();
        reverbZoneData.enabled = StemManager.Instance.audioReverbZone.enabled;
        reverbZoneData.preset = StemManager.Instance.audioReverbZone.reverbPreset;
        reverbZoneData.minDistance = StemManager.Instance.audioReverbZone.minDistance;
        reverbZoneData.maxDistance = StemManager.Instance.audioReverbZone.maxDistance;
        reverbZoneData.roomSize = StemManager.Instance.audioReverbZone.room;
        reverbZoneData.roomHF = StemManager.Instance.audioReverbZone.roomHF;
        reverbZoneData.roomLF = StemManager.Instance.audioReverbZone.roomLF;
        reverbZoneData.decayTime = StemManager.Instance.audioReverbZone.decayTime;
        reverbZoneData.decayHFRatio = StemManager.Instance.audioReverbZone.decayHFRatio;
        reverbZoneData.reflectionsLevel = StemManager.Instance.audioReverbZone.reflections;
        reverbZoneData.reflectionsDelay = StemManager.Instance.audioReverbZone.reflectionsDelay;
        reverbZoneData.reverbLevel = StemManager.Instance.audioReverbZone.reverb;
        reverbZoneData.reverbDelay = StemManager.Instance.audioReverbZone.reverbDelay;
        reverbZoneData.hfReference = StemManager.Instance.audioReverbZone.HFReference;
        reverbZoneData.lfReference = StemManager.Instance.audioReverbZone.LFReference;
        reverbZoneData.diffusion = StemManager.Instance.audioReverbZone.diffusion;
        reverbZoneData.density = StemManager.Instance.audioReverbZone.density;
        sessionData.reverbZoneData = reverbZoneData;

        string data = JsonUtility.ToJson(sessionData, true);

        var path = StandaloneFileBrowser.SaveFilePanel("Save Session", "", "session", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, data);
        }
    }

    public void LoadSession()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Load Session", "", "json", false);
        if (paths.Length == 0)
        {
            return;
        }
        StemManager.Instance.DestroyAllStems();
        string data = File.ReadAllText(paths[0]);
        SessionData sessionData = JsonUtility.FromJson<SessionData>(data);
        // Load stem data
        for (int i = 0; i < sessionData.stems.Length; i++)
        {
            var stemData = sessionData.stems[i];
            var stemItem = StemManager.Instance.AddNewStem();
            var torus = stemItem.GetComponentInChildren<TorusKnotGenerator>(true);
            var spiral = stemItem.GetComponentInChildren<SpiralGenerator>(true);
            var line = stemItem.GetComponentInChildren<WaveLineGenerator>(true);
            var circle = stemItem.GetComponentInChildren<CirclePathGenerator>(true);

            torus.p = stemData.torusP;
            torus.q = stemData.torusQ;
            torus.segments = stemData.torusSegments;
            torus.radius = stemData.torusRadius;
            torus.tube = stemData.torusTube;

            spiral.turns = stemData.spiralTurns;
            spiral.spacing = stemData.spiralSpacing;
            spiral.widthStart = stemData.spiralWidthStart;
            spiral.widthEnd = stemData.spiralWidthEnd;
            spiral.pointsPerTurn = stemData.spiralPointsPerTurn;

            line.length = stemData.lineLength;
            line.amplitude = stemData.lineAmplitude;
            line.waveCount = stemData.lineWaveCount;
            line.pointsPerWave = stemData.linePointsPerWave;

            circle.radius = stemData.circleRadius;
            circle.pointCount = stemData.circlePointCount;

            stemItem.SetStemColor(stemData.color);
            stemItem.ChangeShape(stemData.activeShapeName);
            stemItem.ChangeShapePosition(stemData.position);
            stemItem.ChangeShapeRotation(stemData.rotation);
            stemItem.ChangeShapeScale(stemData.scale);
            stemItem.ChangeBeadSpeed(stemData.bpm);
            stemItem.ChangeBeadOffset(stemData.offset);
            stemItem.EnableSpatialize(stemData.spatialize);
            stemItem.ChangeAudioName(stemData.audioName);
            stemItem.SetAudioUrl(stemData.audioUrl);
        }

        // Load reverb zone data
        var reverbZoneData = sessionData.reverbZoneData;
        StemManager.Instance.audioReverbZone.enabled = reverbZoneData.enabled;
        StemManager.Instance.audioReverbZone.reverbPreset = reverbZoneData.preset;
        StemManager.Instance.audioReverbZone.minDistance = reverbZoneData.minDistance;
        StemManager.Instance.audioReverbZone.maxDistance = reverbZoneData.maxDistance;
        StemManager.Instance.audioReverbZone.room = reverbZoneData.roomSize;
        StemManager.Instance.audioReverbZone.roomHF = reverbZoneData.roomHF;
        StemManager.Instance.audioReverbZone.roomLF = reverbZoneData.roomLF;
        StemManager.Instance.audioReverbZone.decayTime = reverbZoneData.decayTime;
        StemManager.Instance.audioReverbZone.decayHFRatio = reverbZoneData.decayHFRatio;
        StemManager.Instance.audioReverbZone.reflections = reverbZoneData.reflectionsLevel;
        StemManager.Instance.audioReverbZone.reflectionsDelay = reverbZoneData.reflectionsDelay;
        StemManager.Instance.audioReverbZone.reverb = reverbZoneData.reverbLevel;
        StemManager.Instance.audioReverbZone.reverbDelay = reverbZoneData.reverbDelay;
        StemManager.Instance.audioReverbZone.HFReference = reverbZoneData.hfReference;
        StemManager.Instance.audioReverbZone.LFReference = reverbZoneData.lfReference;
        StemManager.Instance.audioReverbZone.diffusion = reverbZoneData.diffusion;
        StemManager.Instance.audioReverbZone.density = reverbZoneData.density;
        OptionUIManager.Instance.UpdateReverbSettings();
    }

    public void NewSession()
    {
        StemManager.Instance.DestroyAllStems();
    }
}

[Serializable]
public class SessionData
{
    public StemData[] stems;
    public ReverbZoneData reverbZoneData;
}

[Serializable]
public class StemData
{
    public string name;
    public Color color;
    public string activeShapeName;
    public string audioName;
    public string audioUrl;
    public bool spatialize;

    public Vector3 position;
    public Vector3 rotation;
    public float scale;
    public float bpm;
    public int offset;

    public int torusP;
    public int torusQ;
    public int torusSegments;
    public float torusRadius;
    public float torusTube;

    public float spiralTurns;
    public float spiralSpacing;
    public float spiralWidthStart;
    public float spiralWidthEnd;
    public int spiralPointsPerTurn;

    public float lineLength;
    public float lineAmplitude;
    public int lineWaveCount;
    public int linePointsPerWave;

    public float circleRadius;
    public int circlePointCount;
}

[Serializable]
public class ReverbZoneData
{
    public bool enabled;
    public AudioReverbPreset preset;
    public float minDistance;
    public float maxDistance;
    public int roomSize;
    public int roomHF;
    public int roomLF;
    public float decayTime;
    public float decayHFRatio;
    public int reflectionsLevel;
    public float reflectionsDelay;
    public int reverbLevel;
    public float reverbDelay;
    public float hfReference;
    public float lfReference;
    public float diffusion;
    public float density;
}