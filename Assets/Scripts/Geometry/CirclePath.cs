using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]                // allow OnValidate in the Editor
public class CirclePathGenerator : MonoBehaviour
{
    [Header("Circle Settings")]
    [Min(3)] public int pointCount = 32;
    [Min(0f)] public float radius = 5f;

    LineRenderer line;
    int lastCount;
    float lastRadius;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        Generate();
    }

    void Update()
    {
        // At runtime, watch for changes to our public fields:
        if (Application.isPlaying)
        {
            if (pointCount != lastCount || !Mathf.Approximately(radius, lastRadius))
                Generate();
        }
    }

    // This runs in Edit mode whenever you change a value in the Inspector:
#if UNITY_EDITOR
    void OnValidate()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        Generate();
    }
#endif

    public void Generate()
    {
        lastCount = pointCount;
        lastRadius = radius;

        // ensure at least a triangle:
        pointCount = Mathf.Max(3, pointCount);
        line.positionCount = pointCount;

        Vector3[] pts = new Vector3[pointCount];
        float inc = 2 * Mathf.PI / pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float a = inc * i;
            pts[i] = new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
        }

        line.SetPositions(pts);
    }
}
