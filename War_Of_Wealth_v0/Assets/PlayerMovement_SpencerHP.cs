using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement_SpencerHP : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float groundDrag = 5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 0.4f;
    private bool readyToJump = true;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [Header("References")]
    [SerializeField] private Transform orientation;

    [Header("Status")]
    public bool canMove = true;

    [Header("Player Input")]
    [SerializeField] public int playerIndex = 0; // 0 = player 1 (keyboard), 1 = player 2 (gamepad)
    [SerializeField] private float stickDeadzone = 0.15f;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        orientation = transform.Find("Orientation");
    }

    private void Update()
    {
        if (!canMove) return;

        // Respawn check
        if (transform.position.y <= -50f)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = Vector3.zero;
        }

        // Ground check
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            whatIsGround
        );

        ReadInput();
        ControlDrag();

        // Jump input based on player index
        if (grounded && readyToJump)
        {
            bool jumpPressed = false;

            if (playerIndex == 0)
            {
                jumpPressed = Keyboard.current != null &&
                              Keyboard.current.spaceKey.wasPressedThisFrame;
            }
            else // player 2: gamepad south button (A on Xbox / X on PlayStation)
            {
                jumpPressed = Gamepad.current != null &&
                              Gamepad.current.buttonSouth.wasPressedThisFrame;
            }

            if (jumpPressed)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private void FixedUpdate()
    {
        if (canMove) MovePlayer();
    }

    private void ReadInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;

        if (playerIndex == 0)
        {
            // Player 1: WASD
            if (Keyboard.current == null) return;

            horizontalInput =
                (Keyboard.current.dKey.isPressed ? 1 : 0) -
                (Keyboard.current.aKey.isPressed ? 1 : 0);

            verticalInput =
                (Keyboard.current.wKey.isPressed ? 1 : 0) -
                (Keyboard.current.sKey.isPressed ? 1 : 0);
        }
        else
        {
            // Player 2: gamepad left stick
            if (Gamepad.current == null) return;

            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.magnitude < stickDeadzone) stick = Vector2.zero;

            horizontalInput = stick.x;
            verticalInput = stick.y;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput
                      + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ControlDrag()
    {
        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = position;
        transform.rotation = rotation;

        Debug.Log($"Player teleported to {position}");
    }
}