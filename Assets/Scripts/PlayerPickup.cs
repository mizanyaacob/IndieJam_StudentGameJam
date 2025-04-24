using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform firecrackerHolder;
    private Firecracker heldFirecracker;

    private KnockbackReceiver knockbackReceiver;

    private bool isHoldingFirecracker => heldFirecracker != null;

    private void Awake()
    {
        if (firecrackerHolder == null)
        {
            Debug.LogError("Firecracker holder not assigned in PlayerPickup.");
            return;
        }

        knockbackReceiver = GetComponent<KnockbackReceiver>();
        if (knockbackReceiver == null)
        {
            Debug.LogError("KnockbackReceiver component not found on the player.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (heldFirecracker != null || isHoldingFirecracker || knockbackReceiver.IsDizzy()) return;

        Firecracker firecracker = other.GetComponent<Firecracker>();
        if (firecracker != null)
        {
            heldFirecracker = firecracker;
            firecracker.PickedUpBy(firecrackerHolder);
            Debug.Log("Picked up: " + firecracker.firecrackerName);
        }
    }

    public void UseFirecracker()
    {
        if (heldFirecracker == null) return;

        // Throw, trigger, or explode logic here
        Debug.Log("Used firecracker!");

        Destroy(heldFirecracker.gameObject);
        heldFirecracker = null;
    }

    public bool IsHoldingFirecracker()
    {
        return heldFirecracker != null;
    }

    public void DropFirecracker()
    {
        if (heldFirecracker == null) return;

        // Detach firecracker
        heldFirecracker.transform.SetParent(null);

        // Reactivate physics and collision
        Rigidbody rb = heldFirecracker.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // optional: add a small bounce
        }

        Collider col = heldFirecracker.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Debug.Log("Dropped firecracker!");

        heldFirecracker = null;
    }

}
