using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : Interactable
{
    [SerializeField] GameObject playerItemTransform;
    [SerializeField] bool canPickup;
    [SerializeField] bool hasItem;

    // Start is called before the first frame update
    void Start()
    {
        canPickup = false;
        hasItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractComponent interactComponent = other.GetComponent<InteractComponent>();

        if (interactComponent)
        {
            canPickup = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPickup = false;
    }

    public override void Interact()
    {
        if (canPickup == true)
        {
            transform.position = playerItemTransform.transform.position;
            transform.transform.parent = playerItemTransform.transform;
            hasItem = true;
        }
    }
}
