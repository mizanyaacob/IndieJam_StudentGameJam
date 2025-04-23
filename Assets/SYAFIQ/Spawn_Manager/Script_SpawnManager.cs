

using System.Collections;
using UnityEngine;

public class Script_SpawnManager : MonoBehaviour
{
    public Vector3 SpawnerSize;
    public Color GizmosColor;
    public GameObject TargetObject;

    [Header("Objects to Spawn (up to 7)")]
    public GameObject[] Objects = new GameObject[7]; // 7 objects to choose from

    [Header("Spawn Position Settings")]
    public float SpawnZ = 0f;

    [Header("Rotation Settings")]
    public bool useRandomRotation = false;
    public Vector3 fixedRotation = Vector3.zero;

    [Header("Spawn Timing")]
    public float spawnInterval = 1f;

    [SerializeField] private float spawnRadius = 10f;

    [SerializeField] private MovingAnnouncementUI movingAnnouncementUI; // Reference to the UI script



    public int ObjectCount;

    void Start()
    {
        StartCoroutine(SpawnLoop());

        movingAnnouncementUI = FindObjectOfType<MovingAnnouncementUI>();

    }

    IEnumerator SpawnLoop()
    {
        while (CheckSpawn(ObjectCount))
        {
            yield return StartCoroutine(SpawnObject());
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    bool CheckSpawn(int count)
    {
        return count > 0;
    }

    IEnumerator SpawnObject()
    {
        Vector3 localOffset = new Vector3(
            Random.Range(-SpawnerSize.x / 2, SpawnerSize.x / 2),
            0f, // Keep Y fixed for horizontal plane
            Random.Range(-SpawnerSize.y / 2, SpawnerSize.y / 2)); // Random Z (depth)


        //float radius = 10f;
        Vector3 point = TargetObject.transform.position + Random.insideUnitSphere * spawnRadius;
        point.y = 10f; // Control height if needed


        // Apply rotation from TargetObject to localOffset
        Vector3 spawnPos = TargetObject.transform.position + TargetObject.transform.rotation * localOffset;

        // Override Z to fix depth regardless of rotation
        spawnPos.y = SpawnZ; // Set Y position instead of Z to control height

        GameObject selectedObject = Objects[Random.Range(0, Objects.Length)];

        if (selectedObject != null)
        {
            Quaternion rotation = useRandomRotation
                ? Random.rotation
                : Quaternion.Euler(fixedRotation);

            Instantiate(selectedObject, point, rotation);
            ObjectCount -= 1;
        }

        yield return null;
    }



    private void OnDrawGizmos()
    {
        if (TargetObject != null)
        {
            Gizmos.color = GizmosColor;

            // Draw cube area (if still needed)
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(TargetObject.transform.position, TargetObject.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, SpawnerSize);
            Gizmos.matrix = oldMatrix;

            // Draw spawn sphere radius
            Gizmos.DrawWireSphere(TargetObject.transform.position, 10f); // match your spawn radius
        }
    }



}
