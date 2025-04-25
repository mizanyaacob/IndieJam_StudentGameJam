// PunchHitbox.cs
using StarterAssets;
using UnityEngine;
public class PunchHitbox : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private bool useUpwardKnockback = true;
    [SerializeField] private float upwardForce = 1f;
    [SerializeField] private float currentKnockbackForce;

    // New serialized field for food scaling
    [SerializeField] private float forcePerFood = 1f; // Additional force per food consumed

    private AudioManager audioManager; // Reference to AudioManager script

    public GameObject hitPrefabs;

    private bool isHitboxActive = false;

    private ThirdPersonController playerController;
    private PlayerFoodConsumer playerFoodConsumer; // Reference to food consumer

    private void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        playerFoodConsumer = GetComponentInParent<PlayerFoodConsumer>(); // Get reference to food consumer
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHitboxActive) return;

        if (other.CompareTag("Player") && other.gameObject != transform.root.gameObject)
        {
            if (other.TryGetComponent(out KnockbackReceiver receiver))
            {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                direction.y = 0;
                direction.Normalize();

                if (useUpwardKnockback)
                {
                    direction.y = upwardForce;
                }

                if (hitPrefabs != null)
                {
                    Instantiate(
                        hitPrefabs,
                        transform.position,
                        Quaternion.identity
                    );
                }

                // Calculate food-modified knockback force
                currentKnockbackForce = knockbackForce;
                if (playerFoodConsumer != null)
                {
                    currentKnockbackForce += playerFoodConsumer.GetFoodConsumed() * forcePerFood;
                }

                playerController = other.GetComponent<ThirdPersonController>();
                playerController.IsKnockback = true;

                // Apply modified force
                receiver.ApplyKnockback(direction, currentKnockbackForce, knockbackDuration, true);
            }
        }
    }

    // Call these via animation events
    public void StartPunch()
    {
        audioManager.PlaySFX(audioManager.Punch);
        isHitboxActive = true;
    }

    public void EndPunch()
    {
        isHitboxActive = false;
    }

    public void resetPunchForce()
    {
        currentKnockbackForce = 5f;
    }
}