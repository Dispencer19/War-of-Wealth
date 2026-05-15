using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] public float sensX = 100f;
    [SerializeField] public float sensY = 100f;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Player Settings")]
    [SerializeField] public int playerIndex = 0; // 0 for player 1, 1 for player 2, etc.

    private float xRotation;
    
    private float yRotation;

    private void Start()
    {
        // Only lock cursor for player 1 (main player) to avoid conflicts
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
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Only allow mouse control for the current player's camera
        // In split-screen, each player controls their own camera
        float mouseX = mouseDelta.x * sensX * Time.deltaTime;
        float mouseY = mouseDelta.y * sensY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY; // correct FPS inversion

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}