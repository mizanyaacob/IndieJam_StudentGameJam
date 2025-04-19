using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;

    private float lastPunchTime;
    private bool isPunching;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
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
}
