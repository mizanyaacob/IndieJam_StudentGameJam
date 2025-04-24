using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
    public float angle = 45f;
    public float range = 10f;
    public int segments = 30;

    void Start()
    {
        GenerateConeMesh();
    }

    void GenerateConeMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // center point

        float step = (angle * 2) / segments;
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle + step * i;
            float rad = currentAngle * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Sin(rad) * range, 0f, Mathf.Cos(rad) * range);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
