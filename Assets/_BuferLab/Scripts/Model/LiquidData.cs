using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LiquidData
{
    public string liquidName = "Empty";
    public float volume = 0f; // ml
    public Color liquidColor = new Color(1f, 1f, 1f, 0f);
    public float phValue = 7.0f;
    
    // Tu dien luu tru so Mol cua tung chat
    public Dictionary<string, float> chemicalComponents = new Dictionary<string, float>();

    // 1. Constructor mac dinh (khong tham so) de cac file moi hoat dong
    public LiquidData() 
    {
    }

    // 2. Constructor 4 tham so de tuong thich voi file HoloButton.cs cu cua ban
    public LiquidData(string name, float vol, Color color, float ph)
    {
        liquidName = name;
        volume = vol;
        liquidColor = color;
        phValue = ph;
    }

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