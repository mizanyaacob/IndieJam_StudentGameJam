using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;

    [SerializeField] private GameObject punchHitboxObject1; // e.g., Right fist
    [SerializeField] private GameObject punchHitboxObject2; // e.g., Left fist

    private float lastPunchTime;
    private bool isPunching;

    private PunchHitbox punchHitbox1;
    private PunchHitbox punchHitbox2;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        punchHitbox1 = punchHitboxObject1.GetComponent<PunchHitbox>();
        punchHitbox2 = punchHitboxObject2.GetComponent<PunchHitbox>();
    }

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
    public void StartPunch1() => punchHitbox1?.StartPunch(); // Right punch
    public void EndPunch1() => punchHitbox1?.EndPunch();

    public void StartPunch2() => punchHitbox2?.StartPunch(); // Left punch
    public void EndPunch2() => punchHitbox2?.EndPunch();
}
