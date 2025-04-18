using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 2f; // Jump height in meters
    public float rotationSpeed = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        // Ground check using CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (move.magnitude > 0.1f)
        {
            Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z) * moveSpeed;
            rb.linearVelocity = newVelocity;

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = Mathf.Sqrt(2f * jumpForce * Mathf.Abs(Physics.gravity.y));
            rb.linearVelocity = velocity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
