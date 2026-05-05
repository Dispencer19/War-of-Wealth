using UnityEngine;
using UnityEngine.InputSystem;

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

    }

    private void FixedUpdate()
    {
        if (canMove) MovePlayer();
    }

    private void ReadInput()
    {
        if (Keyboard.current == null) return;

        horizontalInput =
            (Keyboard.current.dKey.isPressed ? 1 : 0) -
            (Keyboard.current.aKey.isPressed ? 1 : 0);

        verticalInput =
            (Keyboard.current.wKey.isPressed ? 1 : 0) -
            (Keyboard.current.sKey.isPressed ? 1 : 0);
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