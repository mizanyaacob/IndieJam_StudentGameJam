using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpinner : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    public float maxSpinSpeed = 360f;           // Maximum rotation speed in degrees per second
    public float acceleration = 180f;           // How fast the spin speed increases
    public GameObject projectilePrefab;         // Assign in inspector
    public float projectileSpeed = 10f;         // Speed of the projectile
    public Transform shootPoint;                // Optional: where the projectile spawns from
    [SerializeField] private float currentSpinSpeed = 0f;
    [SerializeField] private float finalProjectileSpeed = 0f; // Final projectile speed after boost
    private bool isSpinning = false;
    private bool wasSpinningLastFrame = false;
    [SerializeField] float boostAmount = 5f; // Amount to boost projectile speed based on spin speed

    // Input System references
    [Header("Input Actions")]
    [SerializeField] private InputActionReference spinAction;

    private void OnEnable()
    {
        // Enable input actions when script is enabled
        if (spinAction != null)
        {
            spinAction.action.Enable();

            // Setup callbacks for input actions
            spinAction.action.started += OnSpinStarted;
            spinAction.action.canceled += OnSpinCanceled;
        }
    }

    private void OnDisable()
    {
        // Disable input actions and remove callbacks when script is disabled
        if (spinAction != null)
        {
            spinAction.action.Disable();

            spinAction.action.started -= OnSpinStarted;
            spinAction.action.canceled -= OnSpinCanceled;
        }
    }

    private void OnSpinStarted(InputAction.CallbackContext context)
    {
        isSpinning = true;
    }

    private void OnSpinCanceled(InputAction.CallbackContext context)
    {
        isSpinning = false;
    }

    private void Awake()
    {
        InputAction action = GetComponentInChildren<PlayerInput>().actions.FindAction("Spin", true);
        if (action != null)
        {
            spinAction = InputActionReference.Create(action);
        }
    }
    private void Start()
    {
        
        // Make sure input actions are enabled
        if (spinAction != null && !spinAction.action.enabled)
        {
            spinAction.action.Enable();
        }
    }

    void Update()
    {
        transform.position = playerPos.position; // Keep the spinner at the player's position

        if (isSpinning)
        {
            // Gradually increase spin speed
            currentSpinSpeed = Mathf.MoveTowards(currentSpinSpeed, maxSpinSpeed, acceleration * Time.deltaTime);
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
        }
        else
        {
            // Check if player just stopped spinning
            if (wasSpinningLastFrame)
            {
                ShootProjectile();
                currentSpinSpeed = 0f;
            }
        }
        // Save spinning state for next frame comparison
        wasSpinningLastFrame = isSpinning;
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null) return;
        // Use shootPoint if assigned, otherwise use the player's position
        Transform spawnTransform = shootPoint != null ? shootPoint : transform;
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnTransform.position, spawnTransform.rotation);
        // Add forward velocity to the projectile based on spin speed
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Normalize spin speed relative to max and scale the projectile speed
            float speedMultiplier = currentSpinSpeed / maxSpinSpeed;
            finalProjectileSpeed = projectileSpeed + (speedMultiplier * boostAmount);
            rb.linearVelocity = spawnTransform.forward * finalProjectileSpeed;
        }
    }

    // Public property to check if spinning
    public bool IsSpinning { get { return isSpinning; } }
}