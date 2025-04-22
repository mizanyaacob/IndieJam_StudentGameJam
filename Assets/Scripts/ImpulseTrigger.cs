using Unity.Cinemachine;
using UnityEngine;

public class ImpulseTrigger : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void TriggerImpulse()
    {
        impulseSource.GenerateImpulse();
    }

    // Optional: For directional impulse
    public void TriggerImpulseWithDirection(Vector3 direction)
    {
        impulseSource.GenerateImpulse(direction);
    }
}
