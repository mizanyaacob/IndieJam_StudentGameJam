using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform SpawnPoint1, SpawnPoint2;
    public GameObject PlayerPrefabs;

    private void Awake()
    {
        Instantiate(PlayerPrefabs, SpawnPoint1.position, SpawnPoint1.rotation);
        Instantiate(PlayerPrefabs, SpawnPoint2.position, SpawnPoint2.rotation);
    }
}
