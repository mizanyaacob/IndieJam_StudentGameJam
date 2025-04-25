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

    [Header("Dizzy Effect")]
    [SerializeField] private GameObject dizzyEffect;
    [SerializeField] private Transform dizzyEffectSpawnPoint; // Spawn point for the dizzy effect
    GameObject dizzyEffectInstance;
    private float dizzyTimeRemaining = 0f;
    private bool isDizzy = false;

    [Header("Firecracker Impact")]
    [SerializeField] private float dizzyThresholdMultiplierWhenHoldingFirecracker = 0.5f; // 50% fewer hits needed


    [SerializeField] private GameObject PlayerMarker;


    private PlayerPickup playerPickup; // Reference to PlayerPickup script

    //public GameObject hitEffectPrefab; // Prefab to instantiate on hit

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        playerPickup = GetComponent<PlayerPickup>(); // Initialize PlayerPickup reference

    }

    // In KnockbackReceiver.cs
    public void ApplyKnockback(Vector3 direction, float force, float duration, bool countAsHit = true)
    {
        if (isDizzy) return; // Ignore knockback if dizzy

        knockbackVelocity = direction * force;
        knockbackTimeRemaining = duration;

        if (countAsHit)
        {
            hitCount++;

            int currentDizzyThreshold = hitsUntilDizzy;

            // 💥 Reduce threshold if holding firecracker
            if (playerPickup != null && playerPickup.IsHoldingFirecracker())
            {
                currentDizzyThreshold = Mathf.CeilToInt(hitsUntilDizzy * dizzyThresholdMultiplierWhenHoldingFirecracker);
            }

            if (hitCount % currentDizzyThreshold == 0)
            {
                TriggerDizzyState();
            }
        }

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

        // 🌀 Instantiate the dizzy effect if not already present
        if (dizzyEffectInstance == null && dizzyEffect != null && dizzyEffectSpawnPoint != null)
        {
            dizzyEffectInstance = Instantiate(dizzyEffect, dizzyEffectSpawnPoint.position, Quaternion.identity, dizzyEffectSpawnPoint);
        }

        Debug.Log($"Player is dizzy! Hit count: {hitCount}");

        // 💥 Drop firecracker
        if (playerPickup != null && playerPickup.IsHoldingFirecracker())
        {
            playerPickup.DropFirecracker();
        }
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
            knockbackVelocity = Vector3.zero;
            playerController.IsKnockback = false;
        }

        // Handle dizzy state
        if (isDizzy)
        {
            dizzyTimeRemaining -= Time.deltaTime;

            if (dizzyEffectInstance != null)
            {
                // Optional: Follow head or wiggle effect logic can be here
            }

            if (dizzyTimeRemaining <= 0f)
            {
                isDizzy = false;

                if (animator != null)
                {
                    animator.SetBool(dizzyAnimationBool, false);
                }

                if (playerController != null)
                {
                    playerController.enabled = true;
                }

                // 🧹 Destroy dizzy effect
                if (dizzyEffectInstance != null)
                {
                    Destroy(dizzyEffectInstance);
                    dizzyEffectInstance = null;
                }

                Debug.Log("Player recovered from dizzy state");
            }
        }

        PlayerMarker.SetActive(!isDizzy); // Show player marker when dizzy
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