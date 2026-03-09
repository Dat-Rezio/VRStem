using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OvalRing : MonoBehaviour
{
    [Header("Ellipse Radius")]
    public float radiusX = 2f;
    public float radiusY = 1f;

    [Header("Ring Settings")]
    [Range(10, 200)]
    public int segments = 100;
    public float lineWidth = 0.05f;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = lineWidth;
    }

    void Start()
    {
        DrawOval();
    }

    // void Update()
    // {
    //     // Re-draw every frame so radius can change at runtime
    //     DrawOval();
    // }
    
    public void SetUniformRadius(float r)
    {
        radiusX = r;
        radiusY = r;
        DrawOval();
    }

    public void DrawOval()
    {
        lineRenderer.positionCount = segments;

        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radiusX;
            float z = Mathf.Sin(angle) * radiusY;

            lineRenderer.SetPosition(i, new Vector3(x, 0, z));

            angle += (2 * Mathf.PI) / segments;
        }
    }

    // Call this from other scripts to change size
    public void SetRadius(float newX, float newY)
    {
        radiusX = newX;
        radiusY = newY;
        DrawOval();
    }
}