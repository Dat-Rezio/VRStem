using UnityEngine;

public class LiquidContainer : MonoBehaviour
{
    [Header("Cau hinh binh chua")]
    public float maxCapacity = 250f;

    // Them phan Khoi tao ban dau de nhap truc tiep tu Unity Editor
    [Header("Khoi tao ban dau")]
    public string initialName = "Empty";
    public float initialVolume = 0f;
    public float initialPh = 7f;
    public Color initialColor = new Color(1f, 1f, 1f, 0f);

    [Tooltip("Goi du lieu hoa hoc cua binh nay")]
    public LiquidData liquidData;

    [Header("Ket noi Do hoa")]
    public MeshRenderer liquidRenderer;
    public float shaderMinFill = -0.5f;
    public float shaderMaxFill = 0.5f;

    private Material liquidMaterial;
    private int fillLevelPropID;
    private int colorPropID;

    void Start()
    {
        // Thay vi tao coc rong, he thong se tao du lieu dua tren thong so ban da nhap o Inspector
        liquidData = new LiquidData(initialVolume, initialPh, initialColor, initialName);

        if (liquidRenderer != null)
        {
            liquidMaterial = liquidRenderer.material;
            fillLevelPropID = Shader.PropertyToID("_FillLevel");
            colorPropID = Shader.PropertyToID("_LiquidColor");
        }

        UpdateVisuals();
    }

    // Ham nay de cac he thong khac (nhu ong hut, binh rot) goi vao khi muon them nuoc
    // Thay doi tham so: Nhan vao toan bo goi du lieu cua coc rot (incomingData) thay vi chi nhan Mau sac
    public void ReceiveLiquid(float addedVolume, LiquidData incomingData)
    {
        // Kiem tra tran binh
        if (liquidData.volume + addedVolume > maxCapacity)
        {
            addedVolume = maxCapacity - liquidData.volume;
        }

        // Goi Bo Nao Hoa Hoc ra lam viec
        // Truyen goi du lieu cua coc nay, goi du lieu cua coc rot, va luong nuoc rot vao
        if (ChemistryEngine.Instance != null)
        {
            ChemistryEngine.Instance.MixLiquids(this.liquidData, incomingData, addedVolume);
        }
        else
        {
            Debug.LogWarning("Chua co ChemistryEngine trong Scene!");
        }

        UpdateVisuals();
    }

    // Ham nay de dich tu The tich (ml) sang Chieu cao Shader (-0.5 den 0.5)
    public void UpdateVisuals()
    {
        if (liquidMaterial == null) return;

        // Tinh toan phan tram nuoc trong binh (Tu 0 den 1)
        float fillPercentage = liquidData.volume / maxCapacity;

        // Chuyen phan tram do sang toa do cua Shader
        float currentFillLevel = Mathf.Lerp(shaderMinFill, shaderMaxFill, fillPercentage);

        // Gui du lieu xuong Card do hoa
        liquidMaterial.SetFloat(fillLevelPropID, currentFillLevel);
        liquidMaterial.SetColor(colorPropID, liquidData.liquidColor);
    }
}