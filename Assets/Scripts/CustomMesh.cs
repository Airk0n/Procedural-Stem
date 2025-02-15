using UnityEngine;

[ExecuteInEditMode]
public class CustomMesh : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [SerializeField] private float modifier;

    void Awake()
    {
        GenerateMesh();
    }

    void OnValidate()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        // Ensure MeshFilter & Renderer exist
        meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Create Mesh
        UnityEngine.Mesh mesh = new UnityEngine.Mesh();
        mesh.name = "GeneratedMesh";

        // Define Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1 * modifier, 0, 0),
            new Vector3(0, 1 * modifier, 0),
            new Vector3(1 * modifier, 1 * modifier, 0)
        };

        // Define Triangles
        int[] triangles = new int[]
        {
            0, 2, 1, // First triangle
            2, 3, 1  // Second triangle
        };

        // Assign Mesh Data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Apply Mesh to MeshFilter
        meshFilter.sharedMesh = mesh;
    }
}
