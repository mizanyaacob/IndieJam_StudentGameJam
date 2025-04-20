

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



    public int ObjectCount;

    void Start()
    {
        StartCoroutine(SpawnLoop());
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


        float radius = 10f;
        Vector3 point = Random.insideUnitSphere * radius;

        point.y = 10f;

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

            // Save the current matrix
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply rotation and position to the Gizmos matrix
            Gizmos.matrix = Matrix4x4.TRS(TargetObject.transform.position, TargetObject.transform.rotation, Vector3.one);

            // Draw the rotated wire cube
            Gizmos.DrawWireCube(Vector3.zero, SpawnerSize);

            // Restore the old matrix
            Gizmos.matrix = oldMatrix;
        }
    }

}
