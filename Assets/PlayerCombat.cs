using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;
    [SerializeField] private GameObject punchHitboxObject1; // Right fist
    [SerializeField] private GameObject punchHitboxObject2; // Left fist

    private float lastPunchTime;
    private bool isPunching;
    private PunchHitbox punchHitbox1;
    private PunchHitbox punchHitbox2;
    private PlayerInput playerInput;


    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        punchHitbox1 = punchHitboxObject1.GetComponent<PunchHitbox>();
        punchHitbox2 = punchHitboxObject2.GetComponent<PunchHitbox>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Called by the Input System
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lastPunchTime = Time.time;

            if (!isPunching)
            {
                isPunching = true;
                animator.SetBool("isPunching", true);
            }
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
    public void StartPunch1() => punchHitbox1?.StartPunch();
    public void EndPunch1() => punchHitbox1?.EndPunch();
    public void StartPunch2() => punchHitbox2?.StartPunch();
    public void EndPunch2() => punchHitbox2?.EndPunch();
}