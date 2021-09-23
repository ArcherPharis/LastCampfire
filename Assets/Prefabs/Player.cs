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
            
        }
        LaddersNearby.Remove(ladderExit);
    }

    Ladder FindPlayerClimbingLadder()
    {
        Vector3 PlayerDesiredMoveDirection = GetPlayerDesiredMoveDirection();
        Ladder ChosenLadder = null;
        float ClosestAngle = 180.0f;
        foreach(Ladder ladder in LaddersNearby)
        {
            Vector3 LadderDirection = ladder.transform.position - transform.position;
            LadderDirection.y = 0;
            LadderDirection.Normalize();
            float Dot = Vector3.Dot(PlayerDesiredMoveDirection, LadderDirection);
            float AngleDegrees = Mathf.Acos(Dot) * Mathf.Rad2Deg;
            if(AngleDegrees < LadderClimbAngleDegrees && AngleDegrees < ClosestAngle)
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

        if(ladderToHopOn != CurrentClimbingLadder)
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

        if(CurrentClimbingLadder == null)
        {
            HopOnLadder(FindPlayerClimbingLadder());
        }

        if (IsOnGround())
        {
            Velocity.y = -0.2f;
            HopOnLadder(CurrentClimbingLadder);
        }

        if (IsOnGround() == false)
        {
            characterController.Move(-Velocity * Time.deltaTime);
        }

        Velocity.x = GetPlayerDesiredMoveDirection().x * movementSpeed;
        Velocity.z = GetPlayerDesiredMoveDirection().z * movementSpeed;
        Velocity.y += Gravity * Time.deltaTime;
        characterController.Move(Velocity * Time.deltaTime);
        UpdateRotation();

    }

    Vector3 GetPlayerDesiredMoveDirection()
    {
        return new Vector3(-moveInput.y, 0f, moveInput.x).normalized; //normalized keeps it to one button press
    }

    void UpdateRotation()
    {
        Vector3 PlayerDesiredDirection = GetPlayerDesiredMoveDirection(); //desired direction is direction pressed.
        if(PlayerDesiredDirection.magnitude == 0)
        {
            PlayerDesiredDirection = transform.forward;
        }

        Quaternion DesiredRotation = Quaternion.LookRotation(PlayerDesiredDirection, Vector3.up); //look based on that direction.
        transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, Time.deltaTime * turnSpeed);

    }

}
