using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[ExecuteInEditMode]
public class StemSpline : MonoBehaviour
{
    [SerializeField] StemPreset stemPreset;
    [SerializeField] float maxSize = 20f;
    [SerializeField] int minNodes = 2;
    [SerializeField] int maxNodes = 10;

    [Range(0f,3f)]
    [SerializeField] float roughnessLateralMin;
    [Range(0f, 3f)]
    [SerializeField] float roughnessLateralMax;

    [Header("Gizmos")]
    [SerializeField] bool showGizmoPoints;

    [Range(0f,1f)]
    [SerializeField] float gizmoSize;

    int totalNodes => (splines * 3) + 1;

    float roughnessLateral => Mathf.Lerp(roughnessLateralMin, roughnessLateralMax, stemPreset.roughness);

    int splines => Mathf.RoundToInt(Mathf.Lerp(minNodes, maxNodes, stemPreset.roughness)) + 1;


    List<OrientedPoint> nodePositions;

    public void OnValidate()
    {
        SpawnPoints();
    }

    public OrientedPoint EvaluateStemSpline(float t)
    {
        t = Mathf.Clamp(t, 0, 0.999f);
        int splineIndex = Mathf.FloorToInt(t * splines);
        float localT = (t * splines) % 1;
        Vector3 position = EvaluateSpline(splineIndex, localT);

        return new OrientedPoint(position, Quaternion.identity);
    }

    private void SpawnPoints()
    {
        var height = stemPreset.GetSize(maxSize);
        var segmentSize = height / splines;
        var controlNodeDistance = segmentSize / 2;

        nodePositions = new List<OrientedPoint>();

        Vector3 firstNodePosition = Vector3.zero;
        nodePositions.Add(new OrientedPoint(firstNodePosition, Quaternion.identity));

        int nodeCounter = 1;
        for (int i = 1; i < totalNodes; i++)
        {
            if (nodeCounter == 0)
            {
                Vector3 nodePosition = new Vector3(LateralSpread(), segmentSize * i, LateralSpread());
                nodePositions.Add(new OrientedPoint(nodePosition, Quaternion.identity));
            }
            if(nodeCounter == 1)
            {
                Vector3 nodePosition = new Vector3(LateralSpread(), (segmentSize * i) - segmentSize / 3, LateralSpread());
                nodePositions.Add(new OrientedPoint(nodePosition, Quaternion.identity));
            }
            if (nodeCounter == 2)
            {
                Vector3 nodePosition = new Vector3(LateralSpread(), (segmentSize * i) + segmentSize / 3, LateralSpread());
                nodePositions.Add(new OrientedPoint(nodePosition, Quaternion.identity));
            }

            nodeCounter++;

            if (nodeCounter == 3)
            {
                nodeCounter = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmoPoints) GizmoPoints();
    }

    private void GizmoPoints()
    {
        int nodeCounter = 0;
        for (int i = 0; i < nodePositions.Count - 1; i++)
        {
            if(nodeCounter == 0)
            {
                Gizmos.color = Color.white;
            }
            else
            {
                Gizmos.color = Color.blue;
            }

            nodeCounter++;
            if (nodeCounter == 3)
            {
                nodeCounter = 0;
            }

            Gizmos.DrawSphere(nodePositions[i].position, gizmoSize);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(nodePositions[nodePositions.Count -1].position, gizmoSize);
    }

    private float LateralSpread()
    {
        return Random.Range(-roughnessLateral, roughnessLateral);
    }

    private Vector3 EvaluateSpline(int splineIndex, float localT)
    {
        localT = Mathf.Clamp(localT, 0, 0.999f);
        if ((splineIndex * 3) + 3 > nodePositions.Count) Debug.LogError("Index Out of range");

        int startingIndex = splineIndex * 3;
        OrientedPoint p0 = nodePositions[startingIndex];
        OrientedPoint p1 = nodePositions[startingIndex + 1];
        OrientedPoint p2 = nodePositions[startingIndex + 2];
        OrientedPoint p3 = nodePositions[startingIndex + 3];

        Vector3 a = Vector3.Lerp(p0.position, p1.position, localT);
        Vector3 b = Vector3.Lerp(p1.position, p2.position, localT);
        Vector3 c = Vector3.Lerp(p2.position, p3.position, localT);

        Vector3 d = Vector3.Lerp(a, b, localT);
        Vector3 e = Vector3.Lerp(b, c, localT);

        return Vector3.Lerp(d, e, localT);
    }

    private Vector3 GetBezierPoint(float t)
    {
        return Vector3.up;
    }
}
