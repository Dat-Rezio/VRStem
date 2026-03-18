using UnityEngine;
using System.Collections.Generic;

public class ChemistryEngine : MonoBehaviour
{
    public static ChemistryEngine Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // HAM DI CHUYEN VAT CHAT (Rot tu coc nay sang coc khac)
    public void MixLiquids(LiquidData targetData, LiquidData incomingData, float addedVolume)
    {
        if (addedVolume <= 0f) return;

        // Cap nhat ten dung dich
        if (targetData.volume <= 0.01f || targetData.liquidName == "Empty")
        {
            targetData.liquidName = incomingData.liquidName;
        }
        else if (targetData.liquidName != incomingData.liquidName)
        {
            targetData.liquidName = "Hon hop dung dich";
        }

        float totalVolume = targetData.volume + addedVolume;
        float incomingRatio = addedVolume / totalVolume;

        // Chuyen giao so Mol tu coc rot sang coc hung
        float transferRatio = addedVolume / (incomingData.volume + addedVolume);
        List<string> keys = new List<string>(incomingData.chemicalComponents.Keys);
        foreach (string key in keys)
        {
            float totalMoles = incomingData.chemicalComponents[key];
            float molesTransferred = totalMoles * transferRatio;

            targetData.AddChemical(key, molesTransferred);
            incomingData.chemicalComponents[key] -= molesTransferred;
        }

        targetData.volume = totalVolume;

        // Sau khi chuyen chat xong, goi bo xu ly phan ung
        ProcessReactions(targetData);
    }

    // HAM TINH TOAN PHAN UNG VA pH
    private void ProcessReactions(LiquidData data)
    {
        // 1. Doc so mol hien tai trong coc (Neu khong co thi bang 0)
        float n_HCl = data.chemicalComponents.ContainsKey("HCl") ? data.chemicalComponents["HCl"] : 0f;
        float n_NaOH = data.chemicalComponents.ContainsKey("NaOH") ? data.chemicalComponents["NaOH"] : 0f;
        float n_NH3 = data.chemicalComponents.ContainsKey("NH3") ? data.chemicalComponents["NH3"] : 0f;
        float n_NH4Cl = data.chemicalComponents.ContainsKey("NH4Cl") ? data.chemicalComponents["NH4Cl"] : 0f;

        // 2. CAC PHUONG TRINH PHAN UNG

        // Phan ung Trung hoa (HCl + NaOH -> NaCl + H2O)
        // Hai chat se triet tieu nhau theo ty le 1:1, phan du se con lai
        float reactStrong = Mathf.Min(n_HCl, n_NaOH);
        n_HCl -= reactStrong;
        n_NaOH -= reactStrong;

        // He dem phan ung voi Axit (HCl + NH3 -> NH4Cl)
        float reactAcidBuffer = Mathf.Min(n_HCl, n_NH3);
        n_HCl -= reactAcidBuffer;
        n_NH3 -= reactAcidBuffer;
        n_NH4Cl += reactAcidBuffer;

        // He dem phan ung voi Bazo (NaOH + NH4Cl -> NH3 + NaCl + H2O)
        float reactBaseBuffer = Mathf.Min(n_NaOH, n_NH4Cl);
        n_NaOH -= reactBaseBuffer;
        n_NH4Cl -= reactBaseBuffer;
        n_NH3 += reactBaseBuffer;

        // Cap nhat lai so Mol vao bieu mau du lieu
        data.chemicalComponents["HCl"] = n_HCl;
        data.chemicalComponents["NaOH"] = n_NaOH;
        data.chemicalComponents["NH3"] = n_NH3;
        data.chemicalComponents["NH4Cl"] = n_NH4Cl;

        // 3. TINH TOAN pH SAU PHAN UNG
        float volumeLiters = data.volume / 1000f;
        if (volumeLiters <= 0f) return;

        float pH = 7.0f;
        float epsilon = 1e-6f; // Nguong cuc nho de bo qua cac sai so thap phan

        if (n_HCl > epsilon)
        {
            // Truong hop 1: Coc dang du Axit manh (Tinh theo HCl)
            float concentration = n_HCl / volumeLiters;
            pH = -Mathf.Log10(concentration);
        }
        else if (n_NaOH > epsilon)
        {
            // Truong hop 2: Coc dang du Bazo manh (Tinh theo NaOH)
            float concentration = n_NaOH / volumeLiters;
            pH = 14f + Mathf.Log10(concentration);
        }
        else if (n_NH3 > epsilon && n_NH4Cl > epsilon)
        {
            // Truong hop 3: Dung dich Dem (Ton tai ca NH3 va NH4Cl)
            // Ap dung phuong trinh Henderson-Hasselbalch voi pKa cua NH4+ la 9.25
            pH = 9.25f + Mathf.Log10(n_NH3 / n_NH4Cl);
        }
        else if (n_NH3 > epsilon)
        {
            // Truong hop 4: Chi co Bazo yeu (NH3)
            float concentration = n_NH3 / volumeLiters;
            pH = 14f - 0.5f * (4.75f - Mathf.Log10(concentration)); // pKb cua NH3 la 4.75
        }
        else if (n_NH4Cl > epsilon)
        {
            // Truong hop 5: Chi co Muoi mang tinh Axit (NH4Cl)
            float concentration = n_NH4Cl / volumeLiters;
            pH = 0.5f * (9.25f - Mathf.Log10(concentration));
        }

        // Gioi han pH an toan trong thang do thuc te
        data.phValue = Mathf.Clamp(pH, 0f, 14f);

        data.liquidColor = GetColorFromPH(data.phValue);
    }

    private Color GetColorFromPH(float ph)
    {
        if (ph < 3f) return new Color(1f, 0f, 0f, 0.5f); // Do (Axit manh)
        if (ph < 6f) return Color.Lerp(new Color(1f, 0f, 0f, 0.5f), new Color(1f, 1f, 0f, 0.5f), (ph - 3f) / 3f); // Do sang Vang
        if (ph < 8f) return new Color(1f, 1f, 1f, 0.1f); // Quanh muc 7 la Nuoc tinh khiet (Trang trong suot)
        if (ph < 11f) return Color.Lerp(new Color(1f, 1f, 1f, 0.1f), new Color(0f, 0f, 1f, 0.5f), (ph - 8f) / 3f); // Trang sang Xanh duong
        return new Color(0.5f, 0f, 0.5f, 0.5f); // Tim (Bazo manh)
    }
}