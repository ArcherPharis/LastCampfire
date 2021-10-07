using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] Transform bottomTransform;
    [SerializeField] Transform topTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        LadderClimbing otherAsPlayer = other.GetComponent <LadderClimbing>();
        if (otherAsPlayer!=null)
        {
            otherAsPlayer.NotifyLadderNearby(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LadderClimbing otherAsPlayer = other.GetComponent<LadderClimbing>();
        if (otherAsPlayer != null)
        {
            otherAsPlayer.NotifyLadderExit(this);
        }
    }

    public Transform GetClosestSnapTransform(Vector3 Position)
    {
        float distanceToTop = Vector3.Distance(Position, topTransform.position); //distance to top?
        float distanceTopBottom = Vector3.Distance(Position, bottomTransform.position); //distance to bottom?
        return distanceToTop < distanceTopBottom ? topTransform : bottomTransform; //which is the closest? we use distancetox so we don't have to write the whole thing out each time.
    }

}
