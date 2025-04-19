using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != gameObject) // Avoid hitting self
        {
            KnockbackReceiver receiver = other.GetComponent<KnockbackReceiver>();
            if (receiver != null)
            {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                receiver.ApplyKnockback(direction, knockbackForce, knockbackDuration);
            }
        }
    }
}
