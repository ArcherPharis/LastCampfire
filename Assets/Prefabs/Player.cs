using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask GroundLayerMask;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float LadderClimbAngleDegrees = 20f;
    InputActions inputActions;
    Vector2 moveInput; //because the controls on the keyboard only work in 2 dimensions.
    Vector3 Velocity; //3d movement
    CharacterController characterController;
    float EdgeTrackingDistance = 0.1f;
    float EdgeTrackingDepth = 1f;
    float Gravity = -9.81f;
    Ladder CurrentClimbingLadder;
    List<Ladder> LaddersNearby = new List<Ladder>();



    public void NotifyLadderNearby(Ladder ladderNearby)
    {
        LaddersNearby.Add(ladderNearby);
    }

    public void NotifyLadderExit(Ladder ladderExit)
    {
        if (ladderExit == CurrentClimbingLadder)
        {
            CurrentClimbingLadder = null;
            Velocity.y = 0;
        }
        LaddersNearby.Remove(ladderExit);
    }

    Ladder FindPlayerClimbingLadder()
    {
        Vector3 PlayerDesiredMoveDirection = GetPlayerDesiredMoveDirection();
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

    bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, GroundLayerMask); //checking if we're touching something with ground layer
    }

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputActions.Gameplay.Move.performed += MoveInputsUpdated;
        inputActions.Gameplay.Move.canceled += MoveInputsUpdated;
    }

    void MoveInputsUpdated(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // what is the value?


    }

    void HopOnLadder(Ladder ladderToHopOn) //ladderToHopOn is literally just saying I'm the script.
    {
        if (ladderToHopOn == null) return;

        if (ladderToHopOn != CurrentClimbingLadder)
        {
            Transform snapToTransform = ladderToHopOn.GetClosestSnapTransform(transform.position);
            characterController.Move(snapToTransform.position - transform.position);
            transform.rotation = snapToTransform.rotation;
            CurrentClimbingLadder = ladderToHopOn;

            Debug.Log("ladder hopped on");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentClimbingLadder == null)
        {
            HopOnLadder(FindPlayerClimbingLadder());
        }

        if (CurrentClimbingLadder)
        {
            CalculateClimingVelocity();
        }
        else
        {
            CalculateWalkingVelocity();
        }

        characterController.Move(Velocity * Time.deltaTime);
        UpdateRotation();

    }

    void CalculateClimingVelocity()
    {
        if (moveInput.magnitude == 0)
        {
            Velocity = Vector3.zero;
        }

        Vector3 LadderDirection = CurrentClimbingLadder.transform.forward;
        Vector3 PlayerDesiredMoveDirection = GetPlayerDesiredMoveDirection();

        float Dot = Vector3.Dot(LadderDirection, PlayerDesiredMoveDirection);

        if (Dot < 0)
        {
            //Velocity = GetPlayerDesiredMoveDirection() * movementSpeed;
            Velocity.y = movementSpeed;
        }else
        {
            if (IsOnGround())
            {
                Velocity = GetPlayerDesiredMoveDirection() * movementSpeed;
            }

            Velocity.y = -movementSpeed;
        }
    }

    private void CalculateWalkingVelocity()
    {
        if (IsOnGround())
        {
            Velocity.y = -0.2f;
        }



        Velocity.x = GetPlayerDesiredMoveDirection().x * movementSpeed;
        Velocity.z = GetPlayerDesiredMoveDirection().z * movementSpeed;
        Velocity.y += Gravity * Time.deltaTime;

        Vector3 PosXTracePos = transform.position + new Vector3(EdgeTrackingDistance, 0.5f, 0f);
        Vector3 NegXTracePos = transform.position + new Vector3(-EdgeTrackingDistance, 0.5f, 0f);
        Vector3 PosZTracePos = transform.position + new Vector3(0f, 0.5f, EdgeTrackingDistance);
        Vector3 NegZTracePos = transform.position + new Vector3(0f, 0.5f, -EdgeTrackingDistance);

        bool CanGoPosX = Physics.Raycast(PosXTracePos, Vector3.down, EdgeTrackingDepth, GroundLayerMask);
        bool CanGoNegX = Physics.Raycast(NegXTracePos, Vector3.down, EdgeTrackingDepth, GroundLayerMask);
        bool CanGoPosZ = Physics.Raycast(PosZTracePos, Vector3.down, EdgeTrackingDepth, GroundLayerMask);
        bool CanGoNegZ = Physics.Raycast(NegZTracePos, Vector3.down, EdgeTrackingDepth, GroundLayerMask);

        float xMin = CanGoNegX ? float.MinValue : 0f;
        float xMax = CanGoPosX ? float.MaxValue : 0f;
        float zMin = CanGoNegZ ? float.MinValue : 0f;
        float zMax = CanGoPosZ ? float.MaxValue : 0f;

        Velocity.x = Mathf.Clamp(Velocity.x, xMin, xMax);
        Velocity.z = Mathf.Clamp(Velocity.z, zMin, zMax);



    }

    Vector3 GetPlayerDesiredMoveDirection()
    {
        return new Vector3(-moveInput.y, 0f, moveInput.x).normalized; //normalized keeps it to one button press
    }

    void UpdateRotation()
    {

        if (CurrentClimbingLadder != null)
        {
            return;
        }


        Vector3 PlayerDesiredDirection = GetPlayerDesiredMoveDirection(); //desired direction is direction pressed.
        if (PlayerDesiredDirection.magnitude == 0)
        {
            PlayerDesiredDirection = transform.forward;
        }


        Quaternion DesiredRotation = Quaternion.LookRotation(PlayerDesiredDirection, Vector3.up); //look based on that direction.
        transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, Time.deltaTime * turnSpeed);

    }

}
