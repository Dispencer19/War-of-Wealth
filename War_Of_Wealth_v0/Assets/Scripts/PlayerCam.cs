using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] public float sensX = 100f;
    [SerializeField] public float sensY = 100f;

    [Header("Gamepad Look (player 2)")]
    [SerializeField] public float gamepadLookSpeedX = 220f;
    [SerializeField] public float gamepadLookSpeedY = 220f;
    [SerializeField] private float stickDeadzone = 0.15f;

    [Header("ADS (Zoom)")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float adsFOV = 40f;
    [SerializeField] private float zoomSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Player Settings")]
    [SerializeField] public int playerIndex = 0; // 0 = mouse, 1 = gamepad

    private float xRotation;
    private float yRotation;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = normalFOV;

        if (playerIndex == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (orientation == null)
        {
            orientation = transform.parent?.Find("Orientation");
            if (orientation == null && transform.parent != null)
            {
                orientation = transform.parent;
            }
        }
    }

    private void Update()
    {
        float lookX = 0f;
        float lookY = 0f;

        if (playerIndex == 0)
        {
            // Mouse look
            if (Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                lookX = mouseDelta.x * sensX * Time.deltaTime;
                lookY = mouseDelta.y * sensY * Time.deltaTime;
            }
        }
        else
        {
            // Gamepad look
            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.rightStick.ReadValue();
                if (stick.magnitude < stickDeadzone) stick = Vector2.zero;
                lookX = stick.x * gamepadLookSpeedX * Time.deltaTime;
                lookY = stick.y * gamepadLookSpeedY * Time.deltaTime;
            }
        }

        // Rotation
        yRotation += lookX;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        // ADS Zoom
        bool isADS = false;

        if (playerIndex == 0 && Mouse.current != null)
        {
            isADS = Mouse.current.rightButton.isPressed;
        }
        else if (playerIndex != 0 && Gamepad.current != null)
        {
            isADS = Gamepad.current.leftTrigger.ReadValue() > 0.1f;
        }

        float targetFOV = isADS ? adsFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
}