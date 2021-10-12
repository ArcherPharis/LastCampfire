using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : Interactable
{

    [SerializeField] bool hasItem;

    // Start is called before the first frame update
    void Start()
    {
        
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
            
        }

        //if (other.gameObject.CompareTag("Plate") && hasItem == true)
        //{
            //PutDown();
        //}

    }

    private void OnTriggerExit(Collider other)
    {
        
    }


    public virtual void PickUp(GameObject PickerGameObject)
    {
        Transform pickUpSocketTransform = PickerGameObject.transform;

        Player PickerAsPlayer = PickerGameObject.GetComponent<Player>();
        if (PickerAsPlayer != null)
        {
            pickUpSocketTransform = PickerAsPlayer.GetPickupSocketTransform();
        }

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        transform.rotation = pickUpSocketTransform.rotation;
        transform.parent = pickUpSocketTransform;
        transform.localPosition = Vector3.zero;




        //old code
        //if (canPickup == true)
        //{
            //transform.position = playerItemTransform.transform.position;
            //transform.transform.parent = playerItemTransform.transform;
           // hasItem = true;
        //}
    }
    
    public virtual void PutDown()
    {

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.localPosition = Vector3.forward;
        transform.parent = null;
        hasItem = false;



        //old code
        //transform.position = plateItemTransform.transform.position;
        //transform.transform.parent = plateItemTransform.transform;
        //hasItem = false;
    }

    public override void Interact(GameObject InteractingGameObject)
    {
     Vector3 DirFromInteractingGameObj = (transform.position - InteractingGameObject.transform.position);
        Vector3 DirOfInteractingGameObj = InteractingGameObject.transform.forward;
        float Dot = Vector3.Dot(DirOfInteractingGameObj, DirFromInteractingGameObj);
        if(Dot > 0.5f)
        {
            PickUp(InteractingGameObject);
            hasItem = true;
        }

        if(Dot == 0 && hasItem == true)
        {
            PutDown();
        }

    }
}
