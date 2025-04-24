using UnityEngine;

public class AuntyStick : MonoBehaviour
{
    private GameObject currentTarget;
    public GameObject SlapEffect;
    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }

    // This method can be called via animation event
    public void SlapCurrentTarget()
    {
        if (currentTarget == null) return;

        
        GameObject effect = Instantiate(SlapEffect, currentTarget.transform.position, Quaternion.identity);
        Debug.Log($"{name} hit {currentTarget.name}!");

        PlayerFoodConsumer consumer = currentTarget.GetComponent<PlayerFoodConsumer>();
        if (consumer != null)
        {
            consumer.SetIsConsuming(false);
        }

        KnockbackReceiver knockback = currentTarget.GetComponent<KnockbackReceiver>();
        if (knockback != null)
        {
            knockback.ApplyKnockback(
                (currentTarget.transform.position - transform.position).normalized,
                15f,
                0.5f,
                false
            );

            knockback.ForceDizzy(10f);
        }
    }
}
