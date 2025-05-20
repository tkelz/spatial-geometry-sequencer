using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("SGS/Bead Mover")]
public class BeadMover : MonoBehaviour
{
    [Header("Path Settings")]
    public LineRenderer pathLine;

    [Header("Movement Settings")]
    [Tooltip("Beats per minute - controls how fast the bead moves")]
    [SerializeField] private float _bpm = 10f; // Default to 10 BPM

    public float bpm
    {
        get { return _bpm; }
        set
        {
            _bpm = value;
            // Recalculate speed if we have path points
            if (pathPoints.Count > 0)
                speed = totalPathLength / (60f / _bpm);
        }
    }

    [Range(0, 100)]
    public int offset = 0;

    [Header("Easing Settings")]
    [Range(0f, 1f)]
    [Tooltip("0 = No easing, 0.5 = Moderate easing, 1 = Maximum easing")]
    public float easingAmount = 0.5f; // Default to 0.5 for better visibility

    [Range(0.1f, 5f)]
    [Tooltip("How dramatic the speed variation is (higher = more extreme)")]
    public float easingIntensity = 2f; // New parameter for controlling easing strength

    [Tooltip("The easing curve - adjust handles for steeper curves")]
    public AnimationCurve easingCurve;

    public enum MovementMode
    {
        OneShot,    // Move once and stop at the end
        Loop,       // Continuously loop from end to start
        PingPong    // Reverse direction at ends
    }

    public MovementMode movementMode = MovementMode.Loop;
    public bool isMoving { get; set; } = false;

    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentSegment = 0;
    private float segmentProgress = 0f;
    private float speed = 0f; // Units per second
    private float totalPathLength = 0f;
    private bool isMovingForward = true; // Used for ping-pong
    private bool hasCompletedPath = false; // Used for one-shot
    private float pathProgress = 0f; // Overall progress through the path (0-1)

    void Start()
    {
        if (pathLine == null)
        {
            Debug.LogError("BeadMover: No LineRenderer assigned!");
            enabled = false;
            return;
        }

        // Initialize with a better default curve that makes easing more obvious
        if (easingCurve == null || easingCurve.length < 2)
        {
            // Add more keyframes to allow for tighter curves
            easingCurve = new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 0f),      // Start point
                new Keyframe(0.1f, 0.05f, 1f, 2f), // Early knee point
                new Keyframe(0.3f, 0.6f, 2f, 0f),  // Sharp rise
                new Keyframe(0.7f, 0.4f, 0f, 2f),  // Middle plateau
                new Keyframe(0.9f, 0.95f, 2f, 1f), // Late knee point
                new Keyframe(1f, 1f, 0f, 0f)       // End point
            );

#if UNITY_EDITOR
            // Make the curve more extreme - only in editor
            for (int i = 0; i < easingCurve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(easingCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(easingCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyLeftTangentMode(easingCurve, i, AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(easingCurve, i, AnimationUtility.TangentMode.Free);
            }
#endif
        }

        BakePath(); // Initial bake + place bead at first point
    }

    void Update()
    {
        if (!isMoving) return;

        RebakePathPoints(); // Always refresh the world-space points

        if (pathPoints.Count < 2) return;

        // For one-shot mode, stop moving when we reach the end
        if (movementMode == MovementMode.OneShot && hasCompletedPath)
            return;

        // Get current and next point based on direction
        Vector3 a = pathPoints[currentSegment];
        Vector3 b;

        if (isMovingForward)
        {
            b = currentSegment + 1 < pathPoints.Count ? pathPoints[currentSegment + 1] : pathPoints[0];
        }
        else
        {
            // Moving backward (for ping-pong)
            b = currentSegment - 1 >= 0 ? pathPoints[currentSegment - 1] : pathPoints[0];
        }

        float d = Vector3.Distance(a, b);
        if (d < 0.0001f) d = 0.0001f;

        // Calculate current path progress for easing
        UpdatePathProgress();

        // Apply easing to speed
        float easedSpeed = ApplyEasing(speed);

        segmentProgress += (easedSpeed * Time.deltaTime) / d;
        transform.position = Vector3.Lerp(a, b, segmentProgress);

        if (segmentProgress >= 1f)
        {
            segmentProgress -= 1f;

            if (isMovingForward)
            {
                currentSegment++;

                // Check if we've reached the end of the path
                if (currentSegment >= pathPoints.Count - 1)
                {
                    switch (movementMode)
                    {
                        case MovementMode.OneShot:
                            // Stay at the last point
                            currentSegment = pathPoints.Count - 1;
                            segmentProgress = 0f;
                            transform.position = pathPoints[currentSegment];
                            hasCompletedPath = true;
                            break;

                        case MovementMode.Loop:
                            // Loop back to start
                            currentSegment = 0;
                            // Reset path progress for easing
                            pathProgress = 0f;
                            break;

                        case MovementMode.PingPong:
                            // Reverse direction
                            currentSegment = pathPoints.Count - 2;  // Set to second-to-last point
                            isMovingForward = false;
                            break;
                    }
                }
            }
            else
            {
                // Moving backward (for ping-pong)
                currentSegment--;

                // Check if we've reached the start of the path
                if (currentSegment <= 0)
                {
                    currentSegment = 0;
                    isMovingForward = true;  // Start moving forward again
                    // Reset path progress for easing
                    pathProgress = 0f;
                }
            }
        }
    }

    // Improved easing function that uses intensity parameter
    private float ApplyEasing(float baseSpeed)
    {
        if (easingAmount <= 0.01f) return baseSpeed; // No easing if very low

        // Evaluate the easing curve based on current path progress
        float curveValue = easingCurve.Evaluate(pathProgress);

        // Apply intensity to make variation more dramatic
        // Map curve value from 0-1 to a speed multiplier from slowdown to speedup
        float slowdownFactor = 1f / easingIntensity;  // e.g. 0.5 at intensity 2
        float speedupFactor = easingIntensity;        // e.g. 2.0 at intensity 2

        // Create a speed multiplier that ranges from slowdown to speedup
        float speedMultiplier = Mathf.Lerp(slowdownFactor, speedupFactor, curveValue);

        // Blend between base speed and modified speed based on easing amount
        return baseSpeed * Mathf.Lerp(1f, speedMultiplier, easingAmount);
    }

    // Add these helper methods to allow inspection of the easing during play mode

    // Returns the current speed multiplier for visualization
    public float GetCurrentSpeedMultiplier()
    {
        if (easingAmount <= 0.01f) return 1f;

        float curveValue = easingCurve.Evaluate(pathProgress);
        float slowdownFactor = 1f / easingIntensity;
        float speedupFactor = easingIntensity;
        float multiplier = Mathf.Lerp(slowdownFactor, speedupFactor, curveValue);

        return Mathf.Lerp(1f, multiplier, easingAmount);
    }

    // Returns the current path progress (useful for debugging)
    public float GetPathProgress()
    {
        return pathProgress;
    }

    // Allows resetting the easing curve from editor scripts or inspector buttons
    public void ResetEasingCurve()
    {
        // Create a more dramatic S-curve with steeper tangents
        easingCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 2f),
            new Keyframe(0.25f, 0.5f, 2f, 2f),
            new Keyframe(0.75f, 0.5f, 0f, 2f),
            new Keyframe(1f, 1f, 2f, 0f)
        );

#if UNITY_EDITOR
        // Apply free tangent mode in editor
        for (int i = 0; i < easingCurve.keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(easingCurve, i, AnimationUtility.TangentMode.Free);
            AnimationUtility.SetKeyRightTangentMode(easingCurve, i, AnimationUtility.TangentMode.Free);
        }
#endif
    }

    // Update the overall progress through the path for easing calculations
    private void UpdatePathProgress()
    {
        if (pathPoints.Count <= 1) return;

        // Calculate path progress
        if (isMovingForward)
        {
            float segmentLength = 1f / (pathPoints.Count - 1);
            pathProgress = (currentSegment + segmentProgress) * segmentLength;
        }
        else
        {
            float segmentLength = 1f / (pathPoints.Count - 1);
            pathProgress = 1f - (currentSegment + segmentProgress) * segmentLength;
        }
    }

    void BakePath()
    {
        pathPoints.Clear();
        var xf = pathLine.transform;
        for (int i = 0; i < pathLine.positionCount; i++)
            pathPoints.Add(xf.TransformPoint(pathLine.GetPosition(i)));

        currentSegment = 0;
        segmentProgress = 0f;
        isMovingForward = true;
        hasCompletedPath = false;
        pathProgress = 0f;

        if (pathPoints.Count > 0)
            transform.position = pathPoints[0];

        // Calculate total path length and update speed
        totalPathLength = CalculateTotalPathLength();
        speed = totalPathLength / (60f / bpm);
    }

    void RebakePathPoints()
    {
        pathPoints.Clear();
        var xf = pathLine.transform;
        for (int i = 0; i < pathLine.positionCount; i++)
            pathPoints.Add(xf.TransformPoint(pathLine.GetPosition(i)));

        // Clamp segment index if our shape shrank:
        if (currentSegment >= pathPoints.Count)
        {
            currentSegment = 0;
            segmentProgress = 0f;
            isMovingForward = true;
            hasCompletedPath = false;
            pathProgress = 0f;
        }

        // Recalculate total path length and speed
        totalPathLength = CalculateTotalPathLength();
        speed = totalPathLength / (60f / bpm);
    }

    float CalculateTotalPathLength()
    {
        float length = 0f;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        return length;
    }

    // Public method to reset the bead to the start of the path
    public void ResetToStart()
    {
        BakePath();
        currentSegment = (pathPoints.Count - 1) * offset / 100;
        segmentProgress = 0f;
        isMoving = false;
        isMovingForward = true;
        hasCompletedPath = false;
        pathProgress = 0f;

        if (pathPoints.Count > 0)
            transform.position = pathPoints[currentSegment];
    }
}
