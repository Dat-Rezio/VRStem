using UnityEngine;

public class PlanetVisual : MonoBehaviour
{
    public GameObject marker;
    public GameObject model;

    public void ShowMarker()
    {
        Debug.Log(gameObject.name + "show marker");
        marker.SetActive(true);
        model.SetActive(false);
    }

    public void ShowModel()
    {
        Debug.Log(gameObject.name + "show model");
        marker.SetActive(false);
        model.SetActive(true);
        model.transform.position = marker.transform.position;
    }
    
    
}