using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverInteractable : Interactable
{
    


    public override void Interact(GameObject InteractingObject = null)
    {
      
        GetComponentInChildren<Platform>().MoveTo(true);
    }
}
