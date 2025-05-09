using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("SGS/Geometry/Path Line Connector")]
public class PathLineConnector : MonoBehaviour
{
    [Header("Source Geometry")]
    [Tooltip("The object that has your CirclePathGenerator")]
    public CirclePathGenerator circleSource;

    [Header("Target")]
    [Tooltip("The LineRenderer which we will drive from the circle source")]
    public LineRenderer targetLine;

    void Awake()
    {
        if (circleSource == null || targetLine == null)
        {
            Debug.LogError("PathLineConnector: missing references!");
            enabled = false;
            return;
        }

        // Force circleSource to regenerate its points
        circleSource.Generate();

        // Copy all positions from circleSource's LineRenderer into our targetLine
        var srcLine = circleSource.GetComponent<LineRenderer>();
        int count = srcLine.positionCount;
        Vector3[] positions = new Vector3[count];
        srcLine.GetPositions(positions);

        targetLine.positionCount = count;
        targetLine.loop = true;
        targetLine.useWorldSpace = false;
        targetLine.SetPositions(positions);
    }
}


