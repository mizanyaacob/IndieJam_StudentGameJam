using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KnockbackReceiver : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Vector3 knockbackVelocity;
    private float knockbackTimeRemaining;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        knockbackVelocity = direction * force;
        knockbackTimeRemaining = duration;
    }

    private void Update()
    {
        if (knockbackTimeRemaining > 0f)
        {
            characterController.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimeRemaining -= Time.deltaTime;
        }
        else
            knockbackVelocity = Vector3.zero; // Reset velocity when knockback time is over
    }
}
