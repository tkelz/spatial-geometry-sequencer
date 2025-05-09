using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TorusKnotGenerator : MonoBehaviour
{
    [Tooltip("How many times the curve winds around the central axis")]
    public int p = 2;

    [Tooltip("How many times the curve winds around a circle in the tube")]
    public int q = 3;

    [Min(16), Tooltip("Number of points sampled along the curve")]
    public int segments = 128;

    [Tooltip("Radius from center to the center of the tube")]
    public float radius = 5f;

    [Tooltip("Radius of the tube itself (thickness of the knot)")]
    public float tube = 1f;

    [Tooltip("Should the knot close automatically?")]
    public bool loop = true;

    private LineRenderer _lr;
    private Vector3[] _points;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        Generate();
    }

    // Re-generate in the editor as you tweak parameters
    void OnValidate()
    {
        if (_lr == null) _lr = GetComponent<LineRenderer>();
        Generate();
    }

    /// <summary>
    /// Returns the last-generated points for use by other scripts (e.g. bead mover).
    /// </summary>
    public Vector3[] GetPoints() => _points;

    private void Generate()
    {
        _points = new Vector3[segments];
        float twoPi = Mathf.PI * 2f;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments * twoPi;
            // Parametric torus-knot formula in XZ plane:
            float cosP = Mathf.Cos(p * t);
            float sinP = Mathf.Sin(p * t);
            float cosQ = Mathf.Cos(q * t);
            float sinQ = Mathf.Sin(q * t);

            // (R + r * cos(q·t))·cos(p·t)
            float x = (radius + tube * cosQ) * cosP;
            // r·sin(q·t)
            float y = tube * sinQ;
            // (R + r * cos(q·t))·sin(p·t)
            float z = (radius + tube * cosQ) * sinP;

            _points[i] = new Vector3(x, y, z);
        }

        _lr.positionCount = segments;
        _lr.loop = loop;
        _lr.SetPositions(_points);
    }
}
