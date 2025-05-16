using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public InputManager inputManager;
    public Transform mainCamera;
    public Transform lookTarget;

    [Tooltip("Camera Settings")]
    public float dragSpeed = 100;
    [Range(1, 5)]
    public float minZoom;
    [Range(5, 20)]
    public float maxZoom;
    [Range(50, 85)]
    public float maxAngle;

    private Vector2 dragOrigin;

    void Update()
    {
        if (inputManager.inputMouseLClick && !EventSystem.current.IsPointerOverGameObject())
        {
            if (dragOrigin == Vector2.zero)
            {
                dragOrigin = inputManager.inputLook;
            }

            Vector3 pos = Camera.main.ScreenToViewportPoint(inputManager.inputLook);

            transform.RotateAround(lookTarget.position, Vector3.up, pos.x * dragSpeed);
            mainCamera.RotateAround(lookTarget.position, mainCamera.right, -pos.y * dragSpeed);
        }
        else
        {
            dragOrigin = Vector2.zero;
        }
        mainCamera.LookAt(lookTarget.position);

        if (inputManager.inputZoom != Vector2.zero && !EventSystem.current.IsPointerOverGameObject())
        {
            float zoom = inputManager.inputZoom.y;
            if (
                zoom > 0 && Vector3.Distance(mainCamera.position, lookTarget.position) > minZoom ||
                zoom < 0 && Vector3.Distance(mainCamera.position, lookTarget.position) < maxZoom
            )
            {
                mainCamera.position += mainCamera.forward * zoom * Time.deltaTime * dragSpeed;
            }
            if (Vector3.Distance(mainCamera.position, lookTarget.position) <= minZoom)
            {
                mainCamera.position = mainCamera.position.normalized * minZoom;
            }
            if (Vector3.Distance(mainCamera.position, lookTarget.position) >= maxZoom)
            {
                mainCamera.position = mainCamera.position.normalized * maxZoom;
            }
        }
    }
}