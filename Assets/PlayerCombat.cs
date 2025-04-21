using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;

    [Header("Punch Hitboxes")]
    [SerializeField] private GameObject rightFistHitboxObject;
    [SerializeField] private GameObject leftFistHitboxObject;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f; // Total time to dash

    private float lastDashTime;
    private bool isDashing;
    private float dashStartTime;
    private Vector3 dashStartPosition;
    private Vector3 dashEndPosition;

    private float lastPunchTime;
    private bool isPunching;

    private PunchHitbox rightFistHitbox;
    private PunchHitbox leftFistHitbox;
    private PlayerInput playerInput;
    private CharacterController characterController;

    private PlayerFoodConsumer playerFoodConsumer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        rightFistHitbox = rightFistHitboxObject.GetComponent<PunchHitbox>();
        leftFistHitbox = leftFistHitboxObject.GetComponent<PunchHitbox>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        playerFoodConsumer = GetComponent<PlayerFoodConsumer>();
    }

    public void OnAttack()
    {
        lastPunchTime = Time.time;

        if (!isPunching && !playerFoodConsumer.IsConsuming())
        {
            isPunching = true;
            animator.SetBool("isPunching", true);
        }
    }

    public void OnDash()
    {
        if (Time.time - lastDashTime < dashCooldown || isDashing) return;

        lastDashTime = Time.time;
        dashStartTime = Time.time;

        dashStartPosition = transform.position;
        dashEndPosition = transform.position + transform.forward * dashDistance;

        isDashing = true;

        // animator.SetTrigger("Dash"); // optional animation
    }

    private void Update()
    {
        if (isPunching && Time.time - lastPunchTime > punchTimeout)
        {
            isPunching = false;
            animator.SetBool("isPunching", false);
        }

        if (isDashing)
        {
            float elapsed = Time.time - dashStartTime;
            float t = elapsed / dashDuration;

            if (t < 1f)
            {
                Vector3 nextPos = Vector3.Lerp(dashStartPosition, dashEndPosition, t);
                Vector3 movement = nextPos - transform.position;
                characterController.Move(movement);
            }
            else
            {
                isDashing = false;
            }
        }
    }

    // Animation Events
    public void StartRightPunch() => rightFistHitbox?.StartPunch();
    public void EndRightPunch() => rightFistHitbox?.EndPunch();
    public void StartLeftPunch() => leftFistHitbox?.StartPunch();
    public void EndLeftPunch() => leftFistHitbox?.EndPunch();
}
