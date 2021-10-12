using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour, Toggable
{
    [SerializeField] Transform objectToMove;
    [SerializeField] float transitionTime;

    public Transform StartTrans;
    public Transform EndTrans;

    public void ToggleOn()
    {
        MoveTo(true);
    }

    public void ToggleOff()
    {
        MoveTo(false);
    }

    public void MoveTo(bool ToEnd)
    {
        if (ToEnd)
        {
            MoveTo(EndTrans);
        }
        else
        {
            MoveTo(StartTrans);
        }
    }

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
        while(timer < TransitionTime)
        {
            
            objectToMove.position = Vector3.Lerp(objectToMove.position, Destination.position, timer / TransitionTime);
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, Destination.rotation, timer / TransitionTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
