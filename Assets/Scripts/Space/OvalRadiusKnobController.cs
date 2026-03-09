using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using Unity.VRTemplate;

public class OvalRadiusKnobController : MonoBehaviour
{
    [Header("Knob Reference")]
    public XRKnob knob;

    [Header("Radius Range")]
    public float minRadius = 0.5f;
    public float maxRadius = 4f;

    private List<OvalRing> allRings = new List<OvalRing>();

    void Start()
    {
        
        // Find all OvalRing objects in scene
        allRings.AddRange(FindObjectsOfType<OvalRing>());

        knob.onValueChange.AddListener(UpdateRadius);
    }

    void UpdateRadius(float knobValue)
    {
        float mappedRadius = Mathf.Lerp(minRadius, maxRadius, knobValue);

        foreach (var ring in allRings)
        {
            ring.SetUniformRadius(mappedRadius);
        }
    }
}