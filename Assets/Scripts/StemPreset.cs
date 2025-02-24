using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StemPreset", menuName = "Procedural Stem/StemPreset")]
public class StemPreset : ScriptableObject
{
    [Range(0f, 1f)]
    public float baseThickness;

    [Range(0f, 1f)]
    public float topThickness;

    [Range(1f, 200f)]
    public float age;
    public int maxAge;
    public AnimationCurve growthCurve;

    [Range(0f, 1f)]
    public float roughness;

    public float GetSize(float MaxSize)
    {
        float percentageOfMaxAge = age / maxAge;
        return growthCurve.Evaluate(percentageOfMaxAge) * MaxSize;
    }

    private void OnValidate()
    {

        var stemSplines = FindObjectsByType<StemSpline>(FindObjectsSortMode.None);
        var stemMeshes = FindObjectsByType<StemMesh>(FindObjectsSortMode.None);
        foreach (var obj in stemSplines)
        {
            obj.OnValidate();
            EditorUtility.SetDirty(obj);
        }

        foreach (var obj in stemMeshes)
        {
            obj.OnValidate();
            EditorUtility.SetDirty(obj);
        }
    }
}
