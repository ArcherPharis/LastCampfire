using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimbing : MonoBehaviour
{
    [SerializeField] float LadderClimbAngleDegrees = 20f;
    [SerializeField] float LadderHopOnTime = 0.2f;

    public Ladder CurrentClimbingLadder;
    List<Ladder> LaddersNearby = new List<Ladder>();
    IInputActionCollection inputAction;

    MovementComponent movementComp;

    private void Start()
    {
        movementComp = GetComponent<MovementComponent>();

    }


    private void Update()
    {
        if (CurrentClimbingLadder == null)
        {
            HopOnLadder(FindPlayerClimbingLadder());
        }
    }

    public void SetInput(IInputActionCollection InputAction)
    {
        inputAction = InputAction;
    }

    public void NotifyLadderNearby(Ladder ladderNearby)
    {
        LaddersNearby.Add(ladderNearby);

    }

    public void NotifyLadderExit(Ladder ladderExit)
    {
        if (ladderExit == CurrentClimbingLadder)
        {
            CurrentClimbingLadder = null;
            movementComp.SetClimbingInfo(Vector3.zero, false);
            movementComp.ClearVerticalVelocity();
        }
        LaddersNearby.Remove(ladderExit);
    }

    Ladder FindPlayerClimbingLadder()
    {
        Vector3 PlayerDesiredMoveDirection = movementComp.GetPlayerDesiredMoveDirection();
        Ladder ChosenLadder = null;
        float ClosestAngle = 180.0f;
        foreach (Ladder ladder in LaddersNearby)
        {
            Vector3 LadderDirection = ladder.transform.position - transform.position;
            LadderDirection.y = 0;
            LadderDirection.Normalize();
            float Dot = Vector3.Dot(PlayerDesiredMoveDirection, LadderDirection);
            float AngleDegrees = Mathf.Acos(Dot) * Mathf.Rad2Deg;
            if (AngleDegrees < LadderClimbAngleDegrees && AngleDegrees < ClosestAngle)
            {
                ChosenLadder = ladder;
                ClosestAngle = AngleDegrees;
            }
        }
        return ChosenLadder;
    }

    void HopOnLadder(Ladder ladderToHopOn) 
    {
        if (ladderToHopOn == null) return;

        if (ladderToHopOn != CurrentClimbingLadder)
        {
            Transform snapToTransform = ladderToHopOn.GetClosestSnapTransform(transform.position);
            CurrentClimbingLadder = ladderToHopOn;
            movementComp.SetClimbingInfo(ladderToHopOn.transform.forward, true);
            DisableMovementInput();
            StartCoroutine(movementComp.MoveToTransform(snapToTransform, LadderHopOnTime)); //the method requires a transform and a float
            Invoke("EnableMovementInput", LadderHopOnTime);


            Debug.Log("ladder hopped on");
        }
    }

    public void EnableMovementInput()
    {
        inputAction.Enable();
    }

    public void DisableMovementInput()
    {
        inputAction.Disable();
    }

}
