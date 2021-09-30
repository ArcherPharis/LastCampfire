using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSlot : MonoBehaviour
{

    [SerializeField] Transform ArtifactSlotTrans;
    [SerializeField] Platform platformToMove;

    public void OnArtifactLeft()
    {
        Debug.Log("Artifact Left me");
        platformToMove.MoveTo(platformToMove.StartTrans);
    }

    public void OnArtifactPlaced()
    {
        platformToMove.MoveTo(platformToMove.EndTrans);
        Debug.Log("Artifact place on me");
    }

    public Transform GetSlotsTrans()
    {
        return ArtifactSlotTrans;
    }
}
