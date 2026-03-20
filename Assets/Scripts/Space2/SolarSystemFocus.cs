using System;
using UnityEngine;

public class SolarSystemFocus : MonoBehaviour
{
    public static SolarSystemFocus Instance;
    public Transform solarRoot;
    
    public PlanetController planetController;
    public Handle handle;
    public float zoomSpeed = 2f;
    public float minScale = 0.5f;
    public float targetScale = 100f;
    public float modelAppearScale = 1f;

    [SerializeField]private bool showModel = false;
    
    public void SetSystemScale(float progress)
    {
        Debug.Log(progress);
        // Chuyển đổi progress từ (0-100) sang (0-1)
        float t = progress / 100f;

        // Nội suy giá trị scale dựa trên progress
        float newScale = Mathf.Lerp(minScale, targetScale, t);

        // Áp dụng cho solarRoot hoặc pivot hiện tại
        if (pivot != null)
        {
            pivot.localScale = Vector3.one * newScale;
        }

        
        // Cập nhật trạng thái hiển thị Model/Marker dựa trên scale mới
        // UpdateVisuals(newScale);
    }

    public Transform pivot;
    public bool focusing;

    public PlanetVisual planetVisual;
    
    public XRScaleKnobDelta scaleKnob;

    private void Awake()
    {
        Instance = this;
        pivot = solarRoot;
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        planetVisual = visual;
        planetController.SetPlanetZoom(visual);
        // dùng hàm ChangePivot
        Debug.Log(planet.name);
        pivot = ChangePivot(solarRoot, planet.position);
        focusing = true;
    }

    void Update()
    {
        //Check scale
        // Debug.Log(solarRoot.localScale + " " + modelAppearScale + " " + solarRoot.lossyScale);
        if (solarRoot.lossyScale.x >= modelAppearScale && !showModel)
        {
            planetVisual.ShowModel();
            showModel = true;
        }
        else if (solarRoot.lossyScale.x < modelAppearScale && showModel)
        {
            planetVisual.ShowMarker();
            showModel = false;
        }

        if (!focusing) return;

        float scale = Mathf.Lerp(
            pivot.localScale.x,
            targetScale,
            Time.deltaTime * zoomSpeed
        );
        
        handle.UpdateHandleByScale(scale);

        pivot.localScale = Vector3.one * scale;
        
        if (Mathf.Abs(scale - targetScale) < 0.01f)
            focusing = false;
    }

    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        Transform pivot;

        // nếu đã có pivot
        if (objectToMove.parent != null && objectToMove.parent.name.Contains("_Pivot"))
        {
            pivot = objectToMove.parent;

            Transform oldParent = pivot.parent;

            // tạm tháo object ra
            objectToMove.SetParent(oldParent);

            // di chuyển pivot
            pivot.position = newPivotPosition;
            pivot.rotation = objectToMove.rotation;

            // gắn lại object vào pivot
            objectToMove.SetParent(pivot);
        }
        else
        {
            // Create new pivot object (giữ nguyên logic cũ)
            GameObject pivotObj = new GameObject(objectToMove.name + "_Pivot");
            pivot = pivotObj.transform;

            pivot.position = newPivotPosition;
            pivot.rotation = objectToMove.rotation;

            Transform oldParent = objectToMove.parent;

            if (oldParent != null)
                pivot.SetParent(oldParent);

            objectToMove.SetParent(pivot);
        }

        return pivot;
    }
}