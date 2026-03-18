using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class LiquidStream : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int resolution = 20; // Ve 20 diem de duong cong muot ma
    public Material streamMaterial;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        if (streamMaterial != null)
        {
            lineRenderer.material = streamMaterial;
        }
        lineRenderer.enabled = false;
        lineRenderer.numCapVertices = 5; 
        lineRenderer.numCornerVertices = 5;
    }

    public void BeginPour(Color liquidColor, float width)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = liquidColor;
        lineRenderer.endColor = liquidColor;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width * 0.5f; // Duoi tia nuoc se nho lai
    }

    public void UpdateParabola(Vector3 startPos, Vector3 endPos, Vector3 initialVelocity)
    {
        lineRenderer.positionCount = resolution;

        // Tinh toan thoi gian roi tuong doi dua tren do cao
        float heightDifference = Mathf.Abs(startPos.y - endPos.y);
        float timeToHit = Mathf.Sqrt(2f * heightDifference / Mathf.Abs(Physics.gravity.y));
        if (timeToHit <= 0.01f) timeToHit = 0.1f;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1) * timeToHit;

            // Phuong trinh chuyen dong nem ngang/xiên
            Vector3 currentPos = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;

            // Khong cho tia nuoc dam xuyen qua day coc muc tieu
            if (currentPos.y < endPos.y) currentPos.y = endPos.y;

            lineRenderer.SetPosition(i, currentPos);
        }
    }

    public void EndPour()
    {
        lineRenderer.enabled = false;
    }
}