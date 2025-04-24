using UnityEngine;

public class Firecracker : MonoBehaviour
{
    public string firecrackerName = "Firecracker";

    public void PickedUpBy(Transform firecrackerHolder)
    {
        transform.SetParent(firecrackerHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Optional: Disable physics if needed
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}
