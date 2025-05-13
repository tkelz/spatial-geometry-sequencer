using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Tooltip("Player Input")]
    public InputActionAsset playerControls;

    // Inputs
    public Vector2 inputLook { get; private set; }
    public Vector2 inputZoom {get; private set; }
    public bool inputMouseLClick { get; private set; }

    InputAction lClickAction;
    InputAction lookAction;
    InputAction zoomAction;

    void Awake()
    {
        // Initialize the input actions
        lookAction = playerControls.FindAction("Look");
        lClickAction = playerControls.FindAction("CameraMove");
        zoomAction = playerControls.FindAction("Zoom");
        RegisterInputActions();
    }

    void OnEnable()
    {
        // Enable the input actions when the object is enabled
        lookAction.Enable();
        lClickAction.Enable();
        zoomAction.Enable();
    }

    void RegisterInputActions()
    {
        // Register the input actions to the corresponding methods
        lookAction.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => inputLook = Vector2.zero;

        zoomAction.performed += ctx => inputZoom = ctx.ReadValue<Vector2>();
        zoomAction.canceled += ctx => inputZoom = Vector2.zero;

        lClickAction.performed += ctx => inputMouseLClick = true;
        lClickAction.canceled += ctx => inputMouseLClick = false;
    }

    void OnDisable()
    {
        // Disable the input actions when the object is disabled
        lClickAction.Disable();
        lookAction.Disable();
        zoomAction.Disable();
    }
}