using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Transform))]
public class RoomGizmoDrawer : MonoBehaviour
{
    // color for the wireframe
    public Color gizmoColor = new Color(0, 1, 1, 1);

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        // draw a wire cube at this Transform’s position and scale
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
