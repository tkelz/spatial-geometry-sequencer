using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class WaveLineGenerator : MonoBehaviour
{
    [Header("Wave Parameters")]

    [Tooltip("Total length of the line along the X-axis")]
    [Min(0.1f)] public float length = 5f;

    [Tooltip("Peak deviation up/down from center")]
    [Min(0f)] public float amplitude = 1f;

    [Tooltip("How many full sine-wave cycles over the length")]
    [Min(1)] public int waveCount = 2;

    [Tooltip("Samples (points) per wave cycle; higher = smoother")]
    [Min(2)] public int pointsPerWave = 20;

    [Header("LineRenderer Settings")]
    [Tooltip("If enabled, the line will loop back to its start point")]
    public bool loop = false;

    private LineRenderer _lr;
    private Vector3[] _points;

    // Cache last values for change detection
    private float _lastLength, _lastAmplitude;
    private int _lastWaveCount, _lastPointsPerWave;
    private bool _lastLoop;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = false;  // draw in local space so transforms apply
        GenerateWave();
    }

    void OnValidate()
    {
        if (_lr == null) _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = false;
        GenerateWave();
    }

    void Update()
    {
        // Regenerate if any parameter changed
        if (!Mathf.Approximately(length, _lastLength) ||
            !Mathf.Approximately(amplitude, _lastAmplitude) ||
            waveCount != _lastWaveCount ||
            pointsPerWave != _lastPointsPerWave ||
            loop != _lastLoop)
        {
            GenerateWave();
        }
    }

    /// <summary>
    /// Returns the current array of points along the wave.
    /// </summary>
    public Vector3[] GetPoints() => _points;

    private void GenerateWave()
    {
        // Cache parameters
        _lastLength = length;
        _lastAmplitude = amplitude;
        _lastWaveCount = waveCount;
        _lastPointsPerWave = pointsPerWave;
        _lastLoop = loop;

        // Compute total sample count
        int totalPoints = waveCount * pointsPerWave + 1;
        totalPoints = Mathf.Max(2, totalPoints);

        _points = new Vector3[totalPoints];
        float dx = length / (totalPoints - 1);
        for (int i = 0; i < totalPoints; i++)
        {
            float x = dx * i;
            float t = x / length;
            float y = Mathf.Sin(t * waveCount * 2 * Mathf.PI) * amplitude;
            _points[i] = new Vector3(x, y, 0);
        }

        // Apply to LineRenderer
        _lr.positionCount = _points.Length;
        _lr.loop = loop;
        _lr.SetPositions(_points);
    }
}
