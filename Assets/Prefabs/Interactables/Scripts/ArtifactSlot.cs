using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSlot : MonoBehaviour
{

    [SerializeField] Transform ArtifactSlotTrans;
    [SerializeField] GameObject TogglingObject;

    public void OnArtifactLeft()
    {
        TogglingObject.GetComponent<Toggable>().ToggleOff();
    }

    public void OnArtifactPlaced()
    {
        TogglingObject.GetComponent<Toggable>().ToggleOn();
    }

    public Transform GetSlotsTrans()
    {
        return ArtifactSlotTrans;
    }
}
