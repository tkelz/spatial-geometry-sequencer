using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public InputManager inputManager;
    public Transform mainCamera;

    [Tooltip("Camera Drag Speed")]
    public float dragSpeed = 100;
    [Range(1, 5)]
    public float minZoom;
    [Range(5, 20)]
    public float maxZoom;
    private Vector2 dragOrigin;

    void Update()
    {
        if (inputManager.inputMouseLClick)
        {
            if(dragOrigin == Vector2.zero) {
                dragOrigin = inputManager.inputLook;
            }
            
            Vector3 pos = Camera.main.ScreenToViewportPoint(inputManager.inputLook);

            transform.RotateAround(Vector3.zero, Vector3.up, pos.x * dragSpeed);
            mainCamera.RotateAround(Vector3.zero, mainCamera.right, -pos.y * dragSpeed);
        }
        else {
            dragOrigin = Vector2.zero;
        }
        mainCamera.LookAt(Vector3.zero);


        if(inputManager.inputZoom != Vector2.zero) {
            float zoom = inputManager.inputZoom.y;
            if(
                zoom > 0 && Vector3.Distance(mainCamera.position, Vector3.zero) > minZoom ||
                zoom < 0 && Vector3.Distance(mainCamera.position, Vector3.zero) < maxZoom
            ) {
                mainCamera.position += mainCamera.forward * zoom * Time.deltaTime * dragSpeed;
            }
            if(Vector3.Distance(mainCamera.position, Vector3.zero) <= minZoom) {
                mainCamera.position = mainCamera.position.normalized * minZoom;
            }
            if(Vector3.Distance(mainCamera.position, Vector3.zero) >= maxZoom) {
                mainCamera.position = mainCamera.position.normalized * maxZoom;
            }
        }
    }
}