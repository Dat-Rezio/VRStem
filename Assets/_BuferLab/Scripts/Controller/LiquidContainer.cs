using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviour
{
    public LiquidData liquidData;
    public float maxVolume = 250f; 

    [Header("Ket noi Do hoa")]
    public Renderer liquidRenderer;
    public float shaderMinFill = -0.5f;
    public float shaderMaxFill = 0.5f;

    [Header("Che do mau Sac")]
    [Tooltip("Tich de xem mau theo do pH, bo tich de xem mau dung dich thuc te")]
    public bool showPHColorMode = false;

    public UnityEvent OnLiquidChanged;

    void Start()
    {
        if (liquidData == null) liquidData = new LiquidData();
        UpdateVisuals();
    }

    public void ReceiveLiquid(float amount, LiquidData incomingData)
    {
        if (liquidData.volume + amount > maxVolume)
        {
            amount = maxVolume - liquidData.volume;
        }
        
        if (ChemistryEngine.Instance != null)
        {
            ChemistryEngine.Instance.MixLiquids(liquidData, incomingData, amount);
        }
        
        UpdateVisuals();
        OnLiquidChanged?.Invoke();
    }

    // Ham de goi tu nut bam VR
    public void TogglePHMode()
    {
        showPHColorMode = !showPHColorMode;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (liquidRenderer != null)
        {
            float fillRatio = liquidData.volume / maxVolume;
            float currentFill = Mathf.Lerp(shaderMinFill, shaderMaxFill, fillRatio);
            liquidRenderer.material.SetFloat("_FillLevel", currentFill);

            Color displayColor = showPHColorMode ? 
                ChemistryEngine.Instance.GetColorFromPH(liquidData.phValue) : 
                liquidData.liquidColor;

            liquidRenderer.material.SetColor("_LiquidColor", displayColor);
        }
    }
}