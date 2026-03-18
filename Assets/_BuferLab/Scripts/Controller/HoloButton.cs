using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Dung cho XRI 3.0+

public class HoloButton : MonoBehaviour
{
    [Header("Ket noi He thong")]
    [Tooltip("Keo bệ máy chiếu HoloView vào đây")]
    public HoloView targetHoloView;

    [Tooltip("Keo XR Simple Interactable cua nut nay vao day")]
    public XRSimpleInteractable buttonInteractable;

    [Header("Thong so Hoa chat cua Nut nay")]
    public string chemicalName = "HCl";
    public float phValue = 1f;
    public Color chemicalColor = Color.red;
    public float molarity = 0.1f;
    public float volumePerClick = 5f; // Moi lan bam nho 5ml

    void OnEnable()
    {
        if (buttonInteractable != null)
        {
            // Lang nghe su kien khi tay VR bam hoac ban tia laser chon vao nut
            buttonInteractable.selectEntered.AddListener(OnButtonPressed);
        }
    }

    void OnDisable()
    {
        if (buttonInteractable != null)
        {
            buttonInteractable.selectEntered.RemoveListener(OnButtonPressed);
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (targetHoloView == null) return;

        // Hoi may chieu xem co coc nao dang dat tren do khong
        LiquidContainer container = targetHoloView.GetCurrentContainer();

        if (container != null)
        {
            // Tao ra mot giot hoa chat moi
            LiquidData dispensedData = new LiquidData(volumePerClick, phValue, chemicalColor, chemicalName);

            if (molarity > 0f)
            {
                float moles = molarity * (volumePerClick / 1000f);
                dispensedData.AddChemical(chemicalName, moles);
            }

            // Bom vao coc
            container.ReceiveLiquid(volumePerClick, dispensedData);

            Debug.Log("Da them " + volumePerClick + "ml " + chemicalName + " vao may chieu.");
        }
    }
}