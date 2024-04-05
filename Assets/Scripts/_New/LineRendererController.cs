using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class VerticalCurve : MonoBehaviour
{
    public int curveResolution = 10;
    public float height = 5f;
    public float curvePercentage = 1f; 

    private LineRenderer lineRenderer;
    private Transform startPoint;
    private Transform endPoint;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = curveResolution + 1;

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        startPoint = transform;

        endPoint = GameObject.FindGameObjectWithTag(playerIndex == 0 ? "AimTargetA" : "AimTargetB").transform;
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        if (endPoint == null)
            return;

        Vector3[] points = new Vector3[curveResolution + 1];

        for (int i = 0; i <= curveResolution; i++)
        {
            float t = (float)i / curveResolution;
            t *= curvePercentage;

            points[i] = Vector3.Lerp(startPoint.position, endPoint.position, t);
            points[i] += Vector3.up * height * Mathf.Sin(t * Mathf.PI);
        }

        lineRenderer.SetPositions(points);
    }
}
