
using System.Collections;
using UnityEngine;

public class Script_SpawnManager : MonoBehaviour
{
    public Vector3 SpawnerSize;
    public Color GizmosColor;
    public GameObject TargetObject;

    [Header("Objects to Spawn (up to 7)")]
    public GameObject[] Objects = new GameObject[7]; // 7 objects to choose from

    public int ObjectCount;

    [Header("Rotation Settings")]
    public bool useRandomRotation = false;
    public Vector3 fixedRotation = Vector3.zero;



    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (CheckSpawn(ObjectCount))
        {
            yield return StartCoroutine(SpawnObject());
            yield return new WaitForSeconds(1f);
        }
    }

    bool CheckSpawn(int count)
    {
        return count > 0;
    }

    IEnumerator SpawnObject()
    {
        Vector3 spawnPos = TargetObject.transform.position + new Vector3(
        Random.Range(-SpawnerSize.x / 2, SpawnerSize.x / 2),
        Random.Range(-SpawnerSize.y / 2, SpawnerSize.y / 2),
        0);



        GameObject selectedObject = Objects[Random.Range(0, Objects.Length)];

        if (selectedObject != null)
        {
            Quaternion rotation = useRandomRotation
                ? Random.rotation
                : Quaternion.Euler(fixedRotation);

            Instantiate(selectedObject, spawnPos, rotation);
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
