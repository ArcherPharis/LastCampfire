using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCollider : Interactable
{
    //BoxCollider collider;


    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
