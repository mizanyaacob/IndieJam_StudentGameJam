using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Base")
        {
            Destroy(gameObject);
        }
    }
}
