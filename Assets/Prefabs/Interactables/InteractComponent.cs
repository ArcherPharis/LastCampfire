using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractComponent : MonoBehaviour
{
    List<Interactable> interactables = new List<Interactable>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) //i'm adding to the list
    {
        Interactable otherAsInteractable = other.GetComponent<Interactable>(); //if something triggers, get its Interactable script
        if (otherAsInteractable) //if you do get it, add 
        {
            if (!interactables.Contains(otherAsInteractable)) //if it already contains it, do not add it again.
            {
                interactables.Add(otherAsInteractable); //add to the list
            }
            
        }
    }

    private void OnTriggerExit(Collider other) //i'm removing from the list
    {
        Interactable otherAsInteractable = other.GetComponent<Interactable>(); 
        if (otherAsInteractable)
        {
            if (interactables.Contains(otherAsInteractable)) //if it does contain it and it leaves, remove it.
            {
                interactables.Remove(otherAsInteractable); //remove from list
            }

        }
    }

    public void Interact()
    {
        Interactable closestInteractable = GetClosestInteractable();
        if (closestInteractable != null) //that it isn't null
        {
            closestInteractable.Interact(transform.parent.gameObject);
        }
    }

    Interactable GetClosestInteractable() //finding which in the list is the closest.
    {
        Interactable closestInteractable = null; //sets it to null by default.
        if(interactables.Count == 0) //if there's nothing in the list do nothing.
        {
            return closestInteractable;
        }

        float ClosestDist = float.MaxValue;
        foreach(var itemInteractable in interactables)
        {
            float Dist = Vector3.Distance(transform.position, itemInteractable.transform.position);
            if(Dist < ClosestDist)
            {
                closestInteractable = itemInteractable;
                ClosestDist = Dist;
            }
        }
        return closestInteractable;
    }
}
