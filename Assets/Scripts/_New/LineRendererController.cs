using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class VerticalCurve : MonoBehaviour
{
    public int curveResolution = 10;
    public float height = 5f; // 曲线的高度
    public float curvePercentage = 1f; // 曲线显示的百分比

    private LineRenderer lineRenderer;
    private Transform startPoint;
    private Transform endPoint;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = curveResolution + 1;

        // 根据玩家索引选择起点
        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        startPoint = transform;

        // 根据玩家索引选择终点
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
            t *= curvePercentage; // 应用曲线的显示百分比

            points[i] = Vector3.Lerp(startPoint.position, endPoint.position, t);
            points[i] += Vector3.up * height * Mathf.Sin(t * Mathf.PI); // 加上垂直偏移
        }

        lineRenderer.SetPositions(points);
    }
}
