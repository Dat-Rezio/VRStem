using UnityEngine;

public class ChemicalSource : MonoBehaviour
{
    [Header("Thong so Hoa chat Goc")]
    [Tooltip("Ten cua hoa chat (VD: HCl, NaOH, Nuoc Cat)")]
    public string chemicalName = "Nuoc Cat";

    [Tooltip("Do pH cua dung dich goc nay")]
    public float phValue = 7f;

    [Tooltip("Mau sac cua dung dich")]
    public Color chemicalColor = new Color(1f, 1f, 1f, 0f);

    [Tooltip("Nong do Mol/L. Neu la nuoc cat thi de la 0")]
    public float molarity = 0f;

    [Header("Thiet lap Voi Bom")]
    [Tooltip("The tich duoc bom ra moi lan bam nut (ml)")]
    public float volumePerClick = 10f;

    [Tooltip("Coc dang nam duoi voi de hung nuoc (co the de trong)")]
    public LiquidContainer currentTarget;

    // Ham nay se duoc goi bang Event khi nguoi choi tuong tac (vi du: bam nut)
    public void DispenseChemical()
    {
        // Kiem tra xem co coc nao dang hung ben duoi khong
        if (currentTarget == null)
        {
            Debug.Log("Khong co coc de hung hoa chat.");
            return;
        }

        // 1. Tao mot bieu mau du lieu moi dung voi thong so da setup o tren
        LiquidData dispensedData = new LiquidData(volumePerClick, phValue, chemicalColor, chemicalName);

        // 2. Tinh toan so Mol neu chat nay khong phai la nuoc tinh khiet
        if (molarity > 0f)
        {
            // Cong thuc: n = C_M * V (Luu y V phai doi tu ml sang Lit)
            float moles = molarity * (volumePerClick / 1000f);
            dispensedData.AddChemical(chemicalName, moles);
        }

        // 3. Bom goi du lieu nay vao cai coc dang hung ben duoi
        currentTarget.ReceiveLiquid(volumePerClick, dispensedData);
    }

    // Ham phu tro: Dung Trigger Collider de tu dong nhan dien coc dat vao
    private void OnTriggerEnter(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null)
        {
            currentTarget = container;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LiquidContainer container = other.GetComponentInParent<LiquidContainer>();
        if (container != null && container == currentTarget)
        {
            currentTarget = null; // Xoa muc tieu khi rut coc ra khoi voi
        }
    }
}