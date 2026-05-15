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
    public bool canMove = true; // moved inside the class

    [Header("Player Input")]
    [SerializeField] public int playerIndex = 0; // 0 for player 1, 1 for player 2, etc.

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
            if ((playerIndex == 0 && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                (playerIndex == 1 && Keyboard.current.enterKey.wasPressedThisFrame))
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
        if (Keyboard.current == null) return;

        if (playerIndex == 0)
        {
            // Player 1: WASD
            horizontalInput =
                (Keyboard.current.dKey.isPressed ? 1 : 0) -
                (Keyboard.current.aKey.isPressed ? 1 : 0);

            verticalInput =
                (Keyboard.current.wKey.isPressed ? 1 : 0) -
                (Keyboard.current.sKey.isPressed ? 1 : 0);
        }
        else if (playerIndex == 1)
        {
            // Player 2: Arrow keys
            horizontalInput =
                (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) -
                (Keyboard.current.leftArrowKey.isPressed ? 1 : 0);

            verticalInput =
                (Keyboard.current.upArrowKey.isPressed ? 1 : 0) -
                (Keyboard.current.downArrowKey.isPressed ? 1 : 0);
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
        rb.linearDamping = grounded ? groundDrag : 0f; // linearDamping → drag
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        // Stop physics movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleport the player
        transform.position = position;
        transform.rotation = rotation;

        Debug.Log($"Player teleported to {position}");
    }
}