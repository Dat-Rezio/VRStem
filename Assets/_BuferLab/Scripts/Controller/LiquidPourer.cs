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
    public float maxContainerVolume = 250f;
    [Tooltip("Diem thap nhat cua day coc (Keo chinh object coc vao day neu pivot o day)")]
    public Transform cupBottom;

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

        // 1. Tim diem thap nhat tren vanh coc (Quet 360 do)
        Vector3 lowestRimPoint = rimCenter.position;
        float minHeight = float.MaxValue;
        
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * rimRadius;
            // Chuyen tu toa do RimCenter ra toa do the gioi
            Vector3 worldPoint = rimCenter.TransformPoint(offset);
            
            if (worldPoint.y < minHeight)
            {
                minHeight = worldPoint.y;
                lowestRimPoint = worldPoint;
            }
        }

        // 2. Y tuong cua ban: Tinh chieu cao mat nuoc trong the gioi thuc
        // Chieu cao nuoc ty le thuan voi the tich
        float fillRatio = Mathf.Clamp01(myContainer.liquidData.volume / maxContainerVolume);
        
        // Gia lap do cao cua mat nuoc (Tu day coc tro len)
        float currentWaterLevelY = Mathf.Lerp(cupBottom.position.y, rimCenter.position.y, fillRatio);

        // 3. Kich hoat rot neu mat nuoc cao hon diem thap nhat cua mieng coc
        if (currentWaterLevelY > lowestRimPoint.y)
        {
            // Tinh toan luc rot dua vao viec mat nuoc vuot qua mieng coc bao nhieu
            float overflowAmount = currentWaterLevelY - lowestRimPoint.y;
            // Chuan hoa gia tri overflow de lam ty le rot (0 den 1)
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
            streamVisuals.BeginPour(myContainer.liquidData.liquidColor, streamWidth);
            
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

    // CONG CU KIEM TRA TRUC QUAN TRONG EDITOR
    void OnDrawGizmos()
    {
        if (rimCenter == null) return;

        // Ve vong tron 360 do tai mieng coc bang mau vang
        Gizmos.color = Color.yellow;
        Vector3 prevPoint = rimCenter.TransformPoint(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * rimRadius);
        
        Vector3 lowestPoint = prevPoint;
        float minHeight = prevPoint.y;

        for (int i = 1; i <= 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * rimRadius;
            Vector3 currentPoint = rimCenter.TransformPoint(offset);
            
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;

            if (currentPoint.y < minHeight)
            {
                minHeight = currentPoint.y;
                lowestPoint = currentPoint;
            }
        }

        // Ve mot khoi cau mau do chi dung diem se rot nuoc ra
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lowestPoint, 0.01f);
    }
}