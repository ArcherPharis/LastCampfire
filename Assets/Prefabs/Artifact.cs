using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : Weight
{
    [SerializeField] float DropDownSlotSearchRadius = 0.2f;
    ArtifactSlot CurrentSlot = null;


    private void Start()
    {
        PutDown();
    }
    public override void PickUp(GameObject PickerGameObject)
    {
        base.PickUp(PickerGameObject);
        if(CurrentSlot)
        {
            CurrentSlot.OnArtifactLeft();
            CurrentSlot = null;

        }
    }

    public override void PutDown()
    {
        ArtifactSlot slot = GetArtifactSlotNearBy();
        if (slot != null)
        {
            slot.OnArtifactPlaced();
            transform.parent = null;
            transform.rotation = slot.GetSlotsTrans().rotation;
;           transform.position = slot.GetSlotsTrans().position;
            CurrentSlot = slot;
        }
        else
        {
            base.PutDown();
        }
    }

    ArtifactSlot GetArtifactSlotNearBy()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, DropDownSlotSearchRadius);
        foreach(Collider col in Cols)
        {
            ArtifactSlot slot = col.GetComponent<ArtifactSlot>();
            if(slot != null)
            {
                return slot;
            }

        }
        return null;
    }
}
