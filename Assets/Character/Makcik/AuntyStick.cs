using UnityEngine;
using System.Collections;

public class AuntyStick : MonoBehaviour
{
    private GameObject currentTarget;
    public GameObject SlapEffect;

    public float slapCooldownDuration = 2f; // how long Aunty waits before hitting again
    private bool isOnCooldown = false;

    public void SetTarget(GameObject target)
    {
        if (!isOnCooldown)
        {
            currentTarget = target;
        }
    }

    // This method can be called via animation event
    public void SlapCurrentTarget()
    {
        if (currentTarget == null || isOnCooldown) return;

        GameObject effect = Instantiate(SlapEffect, currentTarget.transform.position, Quaternion.identity);
        effect.transform.localScale *= 2f;

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

        StartCoroutine(SlapCooldown());
    }

    private IEnumerator SlapCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(slapCooldownDuration);
        isOnCooldown = false;
        currentTarget = null; // Clear target so Aunty can pick a new one
    }
}

