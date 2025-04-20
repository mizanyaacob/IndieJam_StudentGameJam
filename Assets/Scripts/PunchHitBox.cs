using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private bool isHitboxActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isHitboxActive) return;

        if (other.CompareTag("Player") && other.gameObject != transform.root.gameObject)
        {
            if (other.TryGetComponent(out KnockbackReceiver receiver))
            {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                receiver.ApplyKnockback(direction, knockbackForce, knockbackDuration);
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
