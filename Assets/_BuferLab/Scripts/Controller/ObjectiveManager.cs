using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Ket noi Giao dien")]
    [Tooltip("Keo dong text hien thi huong dan tren tuong vao day")]
    public TextMeshProUGUI instructionText;

    [Header("Ket noi Du lieu")]
    [Tooltip("Keo cai coc thi nghiem chinh tren ban vao day")]
    public LiquidContainer targetContainer;

    private int currentStep = 0;

    void Update()
    {
        // Kiem tra lien tuc neu da gan day du thanh phan
        if (targetContainer == null || instructionText == null) return;

        LiquidData data = targetContainer.liquidData;

        // Su dung Switch-case de chuyen doi giua cac buoc cua bai hoc
        switch (currentStep)
        {
            case 0:
                instructionText.text = "Buoc 1: Do khoang 50ml nuoc cat vao coc thi nghiem.";

                // Kiem tra the tich va do tinh khiet (pH = 7)
                if (data.volume >= 45f && data.volume <= 55f && data.phValue > 6.9f && data.phValue < 7.1f)
                {
                    currentStep = 1; // Chuyen sang buoc tiep theo
                }
                // Neu lo do axit hay base vao ngay tu dau
                else if (data.volume > 0f && (data.phValue < 6.5f || data.phValue > 7.5f))
                {
                    instructionText.text = "Canh bao: Ban da do sai chat. Hay vut coc nay vao thung rac, lay coc moi va lam lai.";
                }
                break;

            case 1:
                instructionText.text = "Buoc 2: Nho them dung dich HCl vao coc de giam pH xuong duoi 3.0.";

                // Kiem tra xem pH da dat chuan acid chua
                if (data.phValue < 3.0f)
                {
                    currentStep = 2;
                }
                // Canh bao neu coc bi tran ma van chua dat chuan
                else if (data.volume > 200f)
                {
                    instructionText.text = "Canh bao: Coc da day ma pH van chua dat. Hay tieu huy va thu lai.";
                }
                break;

            case 2:
                instructionText.text = "Chuc mung! Ban da hieu cach doc pH. Buoc 3: Trung hoa dung dich bang NaOH de dua pH ve lai muc 7.0.";

                // Nguoi choi phai bom NaOH vao de keo pH len lai
                if (data.phValue >= 6.8f && data.phValue <= 7.2f && data.volume > 60f)
                {
                    currentStep = 3;
                }
                break;

            case 3:
                instructionText.text = "Xuat sac! Ban da hoan thanh bai thuc hanh Mo dau. Ban co the tiep tuc pha che tu do.";
                break;
        }
    }
}