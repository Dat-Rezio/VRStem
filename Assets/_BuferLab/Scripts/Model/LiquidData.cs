using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LiquidData
{
    [Tooltip("The tich hien tai (ml)")]
    public float volume;

    [Tooltip("Do pH hien tai")]
    public float phValue;

    [Tooltip("Mau sac hien tai cua dung dich")]
    public Color liquidColor;

    [Tooltip("Ten dung dich de hien thi len UI")]
    public string liquidName;

    [Tooltip("Cuon so cai luu tru ten chat va so mol tuong ung")]
    public Dictionary<string, float> chemicalComponents;

    // Day la ham Constructor (Ham khoi tao)
    public LiquidData(float initialVolume, float initialPh, Color initialColor, string name)
    {
        volume = initialVolume;
        phValue = initialPh;
        liquidColor = initialColor;
        liquidName = name;
        chemicalComponents = new Dictionary<string, float>();
    }

    // Ham ho tro de them hoac cong don so mol cua mot chat
    public void AddChemical(string chemicalName, float moles)
    {
        if (chemicalComponents.ContainsKey(chemicalName))
        {
            chemicalComponents[chemicalName] += moles;
        }
        else
        {
            chemicalComponents.Add(chemicalName, moles);
        }
    }
}