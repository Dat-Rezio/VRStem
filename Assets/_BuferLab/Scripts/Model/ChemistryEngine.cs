using UnityEngine;
using System.Collections.Generic;

// 1. DINH NGHIA CAU TRUC DU LIEU
public enum ChemicalType { Acid, Base, Salt, Neutral }

[System.Serializable]
public class ChemicalProfile
{
    public string chemicalName;
    public ChemicalType type;
    public bool isStrong; // True neu la Axit manh/Bazo manh
    public float pKa_pKb; // Dung cho Axit yeu/Bazo yeu (Neu la chat manh thi de 0)
}

[System.Serializable]
public class ReactionRule
{
    public string reactionName; // VD: Trung hoa HCl va NaOH
    public string reactant1;    // VD: HCl
    public string reactant2;    // VD: NaOH
    public string product1;     // VD: NaCl
    // Trong phien ban nay chung ta ap dung ty le phan ung 1:1 cho don gian
}

public class ChemistryEngine : MonoBehaviour
{
    public static ChemistryEngine Instance { get; private set; }

    [Header("Tu Dien Hoa Chat")]
    public List<ChemicalProfile> chemicalDatabase = new List<ChemicalProfile>();

    [Header("So Tay Phan Ung")]
    public List<ReactionRule> reactionRules = new List<ReactionRule>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void MixLiquids(LiquidData targetData, LiquidData incomingData, float addedVolume)
    {
        if (addedVolume <= 0f) return;

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
        targetData.liquidColor = Color.Lerp(targetData.liquidColor, incomingData.liquidColor, incomingRatio);

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
        
        // Goi ham xu ly phan ung dong
        ProcessDynamicReactions(targetData);
    }

    private void ProcessDynamicReactions(LiquidData data)
    {
        // 1. DUYET QUA TAT CA CAC LUAT PHAN UNG
        foreach (ReactionRule rule in reactionRules)
        {
            // Kiem tra xem trong coc co du 2 chat tham gia khong
            if (data.chemicalComponents.ContainsKey(rule.reactant1) && data.chemicalComponents.ContainsKey(rule.reactant2))
            {
                float n1 = data.chemicalComponents[rule.reactant1];
                float n2 = data.chemicalComponents[rule.reactant2];

                if (n1 > 0 && n2 > 0)
                {
                    // Tinh toan luong chat phan ung (Chat nao it hon se bi phan ung het truoc)
                    float reactedMoles = Mathf.Min(n1, n2);

                    // Tru di luong chat da phan ung
                    data.chemicalComponents[rule.reactant1] -= reactedMoles;
                    data.chemicalComponents[rule.reactant2] -= reactedMoles;

                    // Sinh ra chat san pham
                    if (!string.IsNullOrEmpty(rule.product1))
                    {
                        data.AddChemical(rule.product1, reactedMoles);
                    }
                }
            }
        }

        // 2. TINH TOAN PH MOI DUA TREN TU DIEN
        CalculateDynamicPH(data);
    }

    private void CalculateDynamicPH(LiquidData data)
    {
        float volumeLiters = data.volume / 1000f;
        if (volumeLiters <= 0f) return;

        float finalPH = 7.0f;
        float epsilon = 1e-6f;

        // Tim kiem xem trong coc dang con lai chat nao noi troi nhat
        ChemicalProfile dominantAcid = null;
        float acidConcentration = 0f;
        ChemicalProfile dominantBase = null;
        float baseConcentration = 0f;

        foreach (var kvp in data.chemicalComponents)
        {
            if (kvp.Value > epsilon)
            {
                ChemicalProfile profile = chemicalDatabase.Find(x => x.chemicalName == kvp.Key);
                if (profile != null)
                {
                    float concentration = kvp.Value / volumeLiters;
                    if (profile.type == ChemicalType.Acid && concentration > acidConcentration)
                    {
                        dominantAcid = profile;
                        acidConcentration = concentration;
                    }
                    else if (profile.type == ChemicalType.Base && concentration > baseConcentration)
                    {
                        dominantBase = profile;
                        baseConcentration = concentration;
                    }
                }
            }
        }

        // Tinh pH dua vao chat noi troi
        if (dominantAcid != null)
        {
            if (dominantAcid.isStrong) finalPH = -Mathf.Log10(acidConcentration);
            else finalPH = 0.5f * (dominantAcid.pKa_pKb - Mathf.Log10(acidConcentration));
        }
        else if (dominantBase != null)
        {
            if (dominantBase.isStrong) finalPH = 14f + Mathf.Log10(baseConcentration);
            else finalPH = 14f - 0.5f * (dominantBase.pKa_pKb - Mathf.Log10(baseConcentration));
        }

        data.phValue = Mathf.Clamp(finalPH, 0f, 14f);
    }

    public Color GetColorFromPH(float ph)
    {
        if (ph < 3f) return Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), ph / 3f); 
        if (ph < 7f) return Color.Lerp(new Color(1f, 0.5f, 0f), Color.green, (ph - 3f) / 4f); 
        if (ph < 11f) return Color.Lerp(Color.green, Color.blue, (ph - 7f) / 4f); 
        return Color.Lerp(Color.blue, new Color(0.5f, 0f, 0.5f), (ph - 11f) / 3f); 
    }
}