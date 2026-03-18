using UnityEngine;

public class ChemicalSource : MonoBehaviour
{
    public string chemicalName = "Nuoc Cat";
    public Color chemicalColor = new Color(1f, 1f, 1f, 0.1f);
    public float phValue = 7.0f;
    public float molarity = 0f; 
    public float volumePerClick = 10f; 

    public LiquidContainer currentTarget; 

    public void DispenseChemical()
    {
        if (currentTarget != null && currentTarget.liquidData.volume < currentTarget.maxVolume)
        {
            LiquidData incomingData = new LiquidData();
            incomingData.liquidName = chemicalName;
            incomingData.volume = volumePerClick;
            incomingData.liquidColor = chemicalColor;
            incomingData.phValue = phValue;

            // Bắt buộc phải tính và nạp số Mol thì ChemistryEngine mới hoạt động
            if (molarity > 0f)
            {
                float moles = molarity * (volumePerClick / 1000f);
                incomingData.AddChemical(chemicalName, moles);
            }

            currentTarget.ReceiveLiquid(volumePerClick, incomingData);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null) currentTarget = container;
    }

    private void OnTriggerExit(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null && currentTarget == container) currentTarget = null;
    }
}