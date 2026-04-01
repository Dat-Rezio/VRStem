using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PlanetHoverEffect : MonoBehaviour
{
    [Header("Highlight Ring")]
    public GameObject highlightRing;

    private XRSimpleInteractable interactable;

    private void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(_ => ShowRing());
        interactable.hoverExited.AddListener(_ => HideRing());

        if (highlightRing != null)
            highlightRing.SetActive(false);
    }

    private void ShowRing()
    {
        if (highlightRing != null)
            highlightRing.SetActive(true);
    }

    private void HideRing()
    {
        if (highlightRing != null)
            highlightRing.SetActive(false);
    }
}