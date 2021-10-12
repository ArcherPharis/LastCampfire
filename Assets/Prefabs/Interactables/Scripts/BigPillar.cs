using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPillar : Interactable
{

    [SerializeField] Transform newTransform;
    [SerializeField] GameObject platformToMove;
    [SerializeField] float transitionSpeed = 2.5f;



    [SerializeField] Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(MoveToTransform(transitionSpeed));
    }

    IEnumerator MoveToTransform(float TransitionTime)
    {
        float timer = 0f;
        while (timer < TransitionTime)
        {
            timer += Time.deltaTime;
            platformToMove.transform.position = Vector3.Lerp(platformToMove.transform.position, newTransform.position, timer / TransitionTime);
            platformToMove.transform.rotation = Quaternion.Lerp(platformToMove.transform.rotation, newTransform.rotation, timer / TransitionTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void Interact(GameObject InteractingObject = null)
    {
        Move();
        Debug.Log("I am here");
        //platformToMove.transform.position = newTransform.transform.position;
    }
}
