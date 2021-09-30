using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] Transform objectToMove;
    [SerializeField] float transitionTime;

    public Transform StartTrans;
    public Transform EndTrans;

    Coroutine movingCouroutine;
  public void MoveTo(Transform Destination)
    {
        if(movingCouroutine != null)
        {
            StopCoroutine(movingCouroutine);
            movingCouroutine = null;
        }


        movingCouroutine = StartCoroutine(MoveToTransform(Destination, transitionTime));
    }

    IEnumerator MoveToTransform(Transform Destination, float TransitionTime)
    {
        float timer = 0f;
        while(timer > TransitionTime)
        {
            timer += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(objectToMove.position, Destination.position, timer / TransitionTime);
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, Destination.rotation, timer / TransitionTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
