using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] public float sensX = 100f;   // mouse (player 1)
    [SerializeField] public float sensY = 100f;   // mouse (player 1)

    [Header("Gamepad Look (player 2)")]
    [SerializeField] public float gamepadLookSpeedX = 220f; // degrees per second
    [SerializeField] public float gamepadLookSpeedY = 220f;
    [SerializeField] private float stickDeadzone = 0.15f;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Player Settings")]
    [SerializeField] public int playerIndex = 0; // 0 = player 1 (mouse), 1 = player 2 (gamepad)

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        // Only lock the cursor for player 1 (mouse aiming). Player 2 uses a gamepad.
        if (playerIndex == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Find orientation if not set
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
            // Player 1: mouse look. Mouse delta is already frame-scaled, so no Time.deltaTime on the delta.
            if (Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                lookX = mouseDelta.x * sensX * Time.deltaTime;
                lookY = mouseDelta.y * sensY * Time.deltaTime;
            }
        }
        else
        {
            // Player 2 (and beyond): gamepad right stick. Stick is a rate, so scale by Time.deltaTime.
            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.rightStick.ReadValue();
                if (stick.magnitude < stickDeadzone) stick = Vector2.zero;
                lookX = stick.x * gamepadLookSpeedX * Time.deltaTime;
                lookY = stick.y * gamepadLookSpeedY * Time.deltaTime;
            }
        }

        yRotation += lookX;
        xRotation -= lookY; // correct FPS inversion
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}