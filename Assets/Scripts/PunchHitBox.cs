// PunchHitbox.cs
using StarterAssets;
using UnityEngine;
public class PunchHitbox : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private bool useUpwardKnockback = true;
    [SerializeField] private float upwardForce = 1f;

    public GameObject hitPrefabs;

    private bool isHitboxActive = false;

    private ThirdPersonController playerController;

    private void OnTriggerEnter(Collider other)
    {
        // Uncommented the check to make sure we only apply knockback when the hitbox is active
        if (!isHitboxActive) return;

        // Make sure we're hitting another player, not ourselves
        if (other.CompareTag("Player") && other.gameObject != transform.root.gameObject)
        {
            if (other.TryGetComponent(out KnockbackReceiver receiver))
            {
                // Calculate direction from punch origin to the target's center
                Vector3 direction = (other.transform.position - transform.position).normalized;

                // Ensure horizontal direction is dominant
                direction.y = 0;
                direction.Normalize();

                // Add upward component if enabled
                if (useUpwardKnockback)
                {
                    direction.y = upwardForce;
                }

                if(hitPrefabs != null)
                {

                    Instantiate(
                        hitPrefabs,
                        transform.position,
                        Quaternion.identity
                    );
                }
                playerController = other.GetComponent<ThirdPersonController>();
                playerController.IsKnockback = true;
                // Apply knockback
                receiver.ApplyKnockback(direction, knockbackForce, knockbackDuration, true);
            }
        }
    }

    // Call these via animation events
    public void StartPunch()
    {
        isHitboxActive = true;
    }

    public void EndPunch()
    {
        isHitboxActive = false;
    }
}