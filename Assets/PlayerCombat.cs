using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;

    [Header("Punch Hitboxes")]
    [SerializeField] private GameObject rightFistHitboxObject; // Right fist
    [SerializeField] private GameObject leftFistHitboxObject;  // Left fist

    private float lastPunchTime;
    private bool isPunching;
    private PunchHitbox rightFistHitbox;
    private PunchHitbox leftFistHitbox;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        rightFistHitbox = rightFistHitboxObject.GetComponent<PunchHitbox>();
        leftFistHitbox = leftFistHitboxObject.GetComponent<PunchHitbox>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Called by the Input System
    public void OnAttack()
    {
        lastPunchTime = Time.time;

        if (!isPunching)
        {
            isPunching = true;
            animator.SetBool("isPunching", true);
        }
    }

    private void Update()
    {
        if (isPunching && Time.time - lastPunchTime > punchTimeout)
        {
            isPunching = false;
            animator.SetBool("isPunching", false);
        }
    }

    // Animation Events
    public void StartRightPunch() => rightFistHitbox?.StartPunch();
    public void EndRightPunch() => rightFistHitbox?.EndPunch();
    public void StartLeftPunch() => leftFistHitbox?.StartPunch();
    public void EndLeftPunch() => leftFistHitbox?.EndPunch();
}
