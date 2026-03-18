using UnityEngine;

[RequireComponent(typeof(LiquidContainer))]
public class LiquidPourer : MonoBehaviour
{
    [Header("Cau Hinh Vanh Coc")]
    public Transform rimCenter;
    public float rimRadius = 0.05f;

    [Header("Cau Hinh Rot Nuoc")]
    public float maxPourRate = 50f;
    public LiquidStream streamVisuals;
    
    [Header("Thong so Tinh toan The tich")]
    public Transform cupBottom;
    [Tooltip("So am de tang do nhay, so duong de giam do nhay")]
    public float pourSensitivityOffset = -0.01f; 

    private LiquidContainer myContainer;
    private bool isPouring = false;

    void Start()
    {
        myContainer = GetComponent<LiquidContainer>();
        if (cupBottom == null) cupBottom = transform;
    }

    void Update()
    {
        if (myContainer.liquidData.volume <= 0f)
        {
            if (isPouring) StopPouring();
            return;
        }

        Vector3 lowestRimPoint = rimCenter.position;
        float minHeight = float.MaxValue;
        
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * rimRadius;
            Vector3 worldPoint = rimCenter.TransformPoint(offset);
            
            if (worldPoint.y < minHeight)
            {
                minHeight = worldPoint.y;
                lowestRimPoint = worldPoint;
            }
        }

        float fillRatio = Mathf.Clamp01(myContainer.liquidData.volume / myContainer.maxVolume);
        
        // Them bu tru vao day de ban chu dong viec cham mep hay chua
        float currentWaterLevelY = Mathf.Lerp(cupBottom.position.y, rimCenter.position.y, fillRatio) + pourSensitivityOffset;

        if (currentWaterLevelY > lowestRimPoint.y)
        {
            float overflowAmount = currentWaterLevelY - lowestRimPoint.y;
            float tiltPercentage = Mathf.Clamp01(overflowAmount / (rimRadius * 2f));
            PourLiquid(tiltPercentage, lowestRimPoint);
        }
        else if (isPouring)
        {
            StopPouring();
        }
    }

    void PourLiquid(float tiltPercentage, Vector3 pourPosition)
    {
        isPouring = true;
        float currentPourRate = maxPourRate * tiltPercentage;
        float amountToPour = currentPourRate * Time.deltaTime;

        if (myContainer.liquidData.volume < amountToPour) amountToPour = myContainer.liquidData.volume;

        myContainer.liquidData.volume -= amountToPour;
        myContainer.UpdateVisuals();

        RaycastHit hit;
        Vector3 targetPoint = pourPosition + Vector3.down * 1.5f;

        if (Physics.Raycast(pourPosition, Vector3.down, out hit, 1.5f))
        {
            targetPoint = hit.point;
            LiquidContainer targetContainer = hit.collider.GetComponentInParent<LiquidContainer>();
            if (targetContainer != null && targetContainer != myContainer)
            {
                targetContainer.ReceiveLiquid(amountToPour, myContainer.liquidData);
            }
        }

        if (streamVisuals != null)
        {
            float streamWidth = Mathf.Lerp(0.005f, 0.02f, tiltPercentage);
            Color streamColor = myContainer.showPHColorMode ? 
                ChemistryEngine.Instance.GetColorFromPH(myContainer.liquidData.phValue) : 
                myContainer.liquidData.liquidColor;
            
            streamVisuals.BeginPour(streamColor, streamWidth);
            
            Vector3 pourDirection = (pourPosition - rimCenter.position).normalized;
            Vector3 initialVelocity = pourDirection * (1.5f * tiltPercentage); 
            streamVisuals.UpdateParabola(pourPosition, targetPoint, initialVelocity);
        }
    }

    void StopPouring()
    {
        isPouring = false;
        if (streamVisuals != null) streamVisuals.EndPour();
    }
}