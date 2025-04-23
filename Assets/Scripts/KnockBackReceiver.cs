using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KnockbackReceiver : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Vector3 knockbackVelocity;
    private float knockbackTimeRemaining;
    private ThirdPersonController playerController;
    private Animator animator;

    [Header("Hit Counter")]
    [SerializeField] private int hitCount = 0;
    [SerializeField] private int hitsUntilDizzy = 10;
    [SerializeField] private string dizzyAnimationBool = "IsDizzy";
    [SerializeField] private float dizzyDuration = 3f;
    private float dizzyTimeRemaining = 0f;
    private bool isDizzy = false;

    //public GameObject hitEffectPrefab; // Prefab to instantiate on hit

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    // In KnockbackReceiver.cs
    public void ApplyKnockback(Vector3 direction, float force, float duration, bool countAsHit = true)
    {
        if (isDizzy) return; // Ignore knockback if dizzy

        knockbackVelocity = direction * force;
        knockbackTimeRemaining = duration;

        // Only increment hit counter if countAsHit is true
        if (countAsHit)
        {
            hitCount++;
            // Check if player should be dizzy
            if (hitCount % hitsUntilDizzy == 0)
            {
                TriggerDizzyState();
            }
        }

        // Let other components know the player is being knocked back
        if (playerController != null)
        {
            playerController.IsKnockback = true;
        }
    }

    private void TriggerDizzyState(bool resetDuration = true)
    {
        isDizzy = true;

        if (resetDuration)
            dizzyTimeRemaining = dizzyDuration;

        if (animator != null)
        {
            animator.SetBool(dizzyAnimationBool, true);
        }

        Debug.Log($"Player is dizzy! Hit count: {hitCount}");
    }


    private void Update()
    {
        // Handle knockback movement
        if (knockbackTimeRemaining > 0f)
        {
            characterController.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimeRemaining -= Time.deltaTime;
        }
        else if (knockbackTimeRemaining <= 0f && playerController != null && playerController.IsKnockback)
        {
            knockbackVelocity = Vector3.zero; // Reset velocity when knockback time is over
            playerController.IsKnockback = false; // Reset knockback state
        }

        // Handle dizzy state timing
        if (isDizzy)
        {
            dizzyTimeRemaining -= Time.deltaTime;

            // End dizzy state when time is up
            if (dizzyTimeRemaining <= 0f)
            {
                isDizzy = false;

                // Set dizzy animation bool to false when recovered
                if (animator != null)
                {
                    animator.SetBool(dizzyAnimationBool, false);
                }

                // Re-enable player controls
                if (playerController != null)
                {
                    playerController.enabled = true;
                }

                Debug.Log("Player recovered from dizzy state");
            }
        }
    }

    // Public getter for hit count
    public int GetHitCount()
    {
        return hitCount;
    }

    // Method to reset hit counter if needed
    public void ResetHitCount()
    {
        hitCount = 0;
    }

    // Method to check if player is currently dizzy
    public bool IsDizzy()
    {
        return isDizzy;
    }

    public void ForceDizzy(float customDuration = -1f)
    {
        if (!isDizzy)
        {
            hitCount = hitsUntilDizzy;

            // If custom duration is provided, override the default
            if (customDuration > 0f)
            {
                dizzyDuration = customDuration;
            }

            TriggerDizzyState();
        }
    }

}