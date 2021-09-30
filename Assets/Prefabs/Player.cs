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
    float EdgeTrackingDepth = 10f;
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
        inputActions.Gameplay.Interact.performed += Interact;
        
    }

    void Interact(InputAction.CallbackContext context)
    {
        InteractComponent interactComponent = GetComponentInChildren<InteractComponent>();
        if (interactComponent != null)
        {
            interactComponent.Interact(); //this is talking about the Interact in INteractComponant
        }
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
            CurrentClimbingLadder = ladderToHopOn;
            StartCoroutine(MoveToTransform(snapToTransform, 0.2f)); //the method requires a transform and a float

            Debug.Log("ladder hopped on");
        }
    }

    IEnumerator MoveToTransform(Transform destiation, float transformTime)
    {
        inputActions.Gameplay.Move.Disable();
        Vector3 startPosition = transform.position;
        Vector3 endPosition = destiation.position;
        Quaternion StartRot = transform.rotation;
        Quaternion EndRot = destiation.rotation;

        float timer = 0f;
        while(timer < transformTime)
        {
            timer += Time.deltaTime;
            //movement, character controller requires the vector 3.
            Vector3 deltaMove = Vector3.Lerp(startPosition, endPosition, timer/transformTime) - transform.position;
            characterController.Move(deltaMove);
            //rotation, doesn't require, just rotates the transform
            transform.rotation = Quaternion.Lerp(StartRot, EndRot, timer/transformTime);
            yield return new WaitForEndOfFrame();

        }


        inputActions.Gameplay.Move.Enable();
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
            return;
        }

        Vector3 LadderDirection = -CurrentClimbingLadder.transform.right;
        Vector3 PlayerDesiredMoveDirection = GetPlayerDesiredMoveDirection();

        float Dot = Vector3.Dot(LadderDirection, PlayerDesiredMoveDirection);

        Velocity = Vector3.zero;

        if (Dot < 0)
        {
            Velocity = GetPlayerDesiredMoveDirection() * movementSpeed;
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
