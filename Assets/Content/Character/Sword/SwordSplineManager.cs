using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SwordSplineManager : MonoBehaviour
{
    private SplineContainer container;
    private Spline mainSpline;

    private void Awake()
    {
        container = GetComponent<SplineContainer>();
        mainSpline = new Spline();
        container.Spline = mainSpline;
    }

    public void GenerateSpline(List<Transform> targets)
    {
        foreach (Transform target in targets)
        {
            mainSpline.Add(new BezierKnot(target.position, float3.zero, float3.zero, Quaternion.identity));
        }
    }
}
