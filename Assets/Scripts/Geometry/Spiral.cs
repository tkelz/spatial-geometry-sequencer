using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class SpiralGenerator : MonoBehaviour
{
    [Header("Spiral Shape")]
    [Tooltip("Number of full revolutions the spiral makes")]
    [Min(0.01f)] public float turns = 3f;

    [Tooltip("Vertical distance between each revolution (total height = turns × spacing)")]
    public float spacing = 2f;

    [Tooltip("Radius at the start (bottom) of the spiral")]
    public float widthStart = 1f;

    [Tooltip("Radius at the end (top) of the spiral)")]
    public float widthEnd = 3f;

    [Header("Resolution")]
    [Tooltip("Number of sample points per single turn")]
    [Min(4)] public int pointsPerTurn = 32;

    [Header("LineRenderer Settings")]
    [Tooltip("Should the LineRenderer close the curve back on itself?")]
    public bool loop = false;

    // Cached references & state
    private LineRenderer _lr;
    private Vector3[] _points;

    // Last‐seen parameters for change detection
    private float _lastTurns, _lastSpacing, _lastWS, _lastWE;
    private int _lastPtsPerTurn;
    private bool _lastLoop;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        // Draw in local space so Transform affects the spiral
        _lr.useWorldSpace = false;
        Regenerate();
    }

    void OnValidate()
    {
        if (_lr == null) _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = false;
        Regenerate();
    }

    void Update()
    {
        // Only regenerate when a parameter has actually changed
        if (!Mathf.Approximately(turns, _lastTurns)
         || !Mathf.Approximately(spacing, _lastSpacing)
         || !Mathf.Approximately(widthStart, _lastWS)
         || !Mathf.Approximately(widthEnd, _lastWE)
         || pointsPerTurn != _lastPtsPerTurn
         || loop != _lastLoop)
        {
            Regenerate();
        }
    }

    /// <summary>
    /// Returns the current set of points for external use (e.g. bead mover).
    /// </summary>
    public Vector3[] GetPoints() => _points;

    private void Regenerate()
    {
        // Cache current parameters
        _lastTurns = turns;
        _lastSpacing = spacing;
        _lastWS = widthStart;
        _lastWE = widthEnd;
        _lastPtsPerTurn = pointsPerTurn;
        _lastLoop = loop;

        // Determine total sample count: pointsPerTurn * turns, plus one to include the start
        int totalPoints = Mathf.Max(2, Mathf.CeilToInt(pointsPerTurn * turns) + 1);
        _points = new Vector3[totalPoints];

        float twoPi = Mathf.PI * 2f;

        for (int i = 0; i < totalPoints; i++)
        {
            // t goes from 0 → 1 across the entire spiral
            float t = (float)i / (totalPoints - 1);

            // angle around Y-axis scaled by number of turns
            float angle = twoPi * turns * t;

            // interpolate radius from start → end
            float radius = Mathf.Lerp(widthStart, widthEnd, t);

            // compute local-space position
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float y = spacing * turns * t;

            _points[i] = new Vector3(x, y, z);
        }

        // Update the LineRenderer
        _lr.positionCount = _points.Length;
        _lr.loop = loop;
        _lr.SetPositions(_points);
    }
}
