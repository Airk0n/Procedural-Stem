using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(StemSpline))]
public class StemMesh : MonoBehaviour
{
    [UnityEngine.Range(0, 32)]
    [SerializeField] int radialSegments = 3;

    [SerializeField] int lengthSegments;

    [SerializeField] float lengthMin;
    [SerializeField] float lengthMax;

    [SerializeField] float thicknessMin;
    [SerializeField] float thicknessMax;

    [SerializeField] Material material;
    [SerializeField] StemPreset stemPreset;

    [UnityEngine.Range(0f, 1f)]
    public float splineT;

    [Header("Gizmos")]
    [SerializeField] bool showGizmoRings;
    [SerializeField] bool showGizmoT;
    [SerializeField] bool showGizmoVerticalSegments;
    [SerializeField] bool showGizmoMesh;
    [UnityEngine.Range(0f,1f)]
    [SerializeField] float GizmosSize;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    StemSpline stemSpline;
    Mesh mesh;
    float thicknessTop;
    float thicknessBot;

    private void Awake()
    {
        UpdateMesh();

    }
    public void OnValidate()
    {
        UpdateMesh();
    }

    private float GetThickness(float t)
    {
        thicknessBot = Mathf.Lerp(thicknessMin, thicknessMax, stemPreset.baseThickness);
        thicknessTop = Mathf.Lerp(thicknessMin, thicknessMax, stemPreset.topThickness);
        return Mathf.Lerp(thicknessBot, thicknessTop, t);
    }

    private void UpdateMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        stemSpline = GetComponent<StemSpline>();
        if (!VariablesAssignedInInspector()) return;
        if (radialSegments < 3) radialSegments = 3;

        GenerateMesh();
        meshRenderer.sharedMaterial = material;
        meshFilter.sharedMesh = mesh;
    }

    bool VariablesAssignedInInspector()
    {
        return meshRenderer && stemPreset && stemSpline;
    }


    private void GenerateMeshLeg()
    {
        mesh = new Mesh();
        mesh.name = "ProceduralStem";
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        float segmentSize = 1f / lengthSegments;
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int r = 0; r < radialSegments; r++)
            {
                float angle = r * Mathf.PI * 2 / radialSegments;
                Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * GetThickness(segmentSize * l);
                vertices.Add(position);
            }
        }

        int[] triangles = new int[vertices.Count/3];
        for (int i = 0; i < vertices.Count; i++)
        {

        }

        //mesh.SetVertices(vertices);
        //mesh.triangles = triangles;
        //mesh.RecalculateNormals();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "ProceduralStem";
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        float segmentSize = 1f / lengthSegments;

        // Generate vertices and normals
        for (int l = 0; l <= lengthSegments; l++)
        {
            Vector3 center = stemSpline.EvaluateStemSpline(l * segmentSize).position;
            Vector3 forward = (stemSpline.EvaluateStemSpline((l + 1) * segmentSize).position - center).normalized; // Approximate tangent
            Vector3 up = Vector3.up; // Use a better method if your spline is more complex
            Vector3 right = Vector3.Cross(forward, up).normalized;
            up = Vector3.Cross(right, forward).normalized;  // Ensure up is corrected

            for (int r = 0; r < radialSegments; r++)
            {
                float angle = r * Mathf.PI * 2 / radialSegments;
                Vector3 localPos = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * GetThickness(segmentSize * l);
                vertices.Add(center + localPos);
                normals.Add(localPos.normalized);
            }
        }

        // Generate triangles
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int r = 0; r < radialSegments; r++)
            {
                int current = l * radialSegments + r;
                int next = current + radialSegments;
                int nextR = (r + 1) % radialSegments; // Wrap around the ring

                int currentNext = l * radialSegments + nextR;
                int nextNext = currentNext + radialSegments;

                // First triangle
                triangles.Add(current);
                triangles.Add(next);
                triangles.Add(nextNext);

                // Second triangle
                triangles.Add(current);
                triangles.Add(nextNext);
                triangles.Add(currentNext);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        mesh.RecalculateBounds();

    }


    void OnDrawGizmosSelected()
    {
        if(showGizmoRings)              GizmoMeshRings();
        if(showGizmoT)                  GizmoTValueFromSpline();
        if(showGizmoVerticalSegments)   GizmoVerticalSegments();
        if (showGizmoMesh)              GizmoMesh();
    }

    private void GizmoMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        float segmentSize = 1f / lengthSegments;
        for (int l = 0; l < lengthSegments; l++)
        {
            Vector3 offset = stemSpline.EvaluateStemSpline(segmentSize * l).position;
            for (int r = 0; r < radialSegments; r++)
            {
                float angle = r * Mathf.PI * 2 / radialSegments;
                Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * GetThickness(segmentSize * l);
                vertices.Add(position + offset);
            }
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(vertices[i], GizmosSize);
        }
    }

    private void GizmoTValueFromSpline()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(stemSpline.EvaluateStemSpline(splineT).position, GizmosSize);
    }

    private void GizmoVerticalSegments()
    {
        float segmentSize = 1f / lengthSegments;
        for (int i = 0; i < lengthSegments; i++)
        {
            Vector3 position = stemSpline.EvaluateStemSpline(i * segmentSize).position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(position, GizmosSize);
        }
    }

    private void GizmoMeshRings()
    {
        Gizmos.color = Color.green;

        OrientedPoint splineEvaluatedPoint = stemSpline.EvaluateStemSpline(splineT);

        for (int i = 0; i < radialSegments; i++)
        {
            float angle = i * Mathf.PI * 2 / radialSegments; 
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * GetThickness(splineT);
            position = splineEvaluatedPoint.LocalToWorld(position);

            Gizmos.DrawSphere(transform.position + position, 0.05f);
        }

        for (int i = 0; i < radialSegments; i++)
        {
            float angle = i * Mathf.PI * 2 / radialSegments; 
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * thicknessBot;
            //position = splineEvaluatedPoint.LocalToWorld(position);
            Gizmos.DrawSphere(transform.position + position, 0.05f);
        }
    }
}
