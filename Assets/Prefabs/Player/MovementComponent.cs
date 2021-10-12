using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float turnSpeed = 5f;
    float EdgeTrackingDistance = 0.1f;
    float EdgeTrackingDepth = 10f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask GroundLayerMask;

    bool isClimbing;
    Vector3 LadderDir;
    Vector2 moveInput; //because the controls on the keyboard only work in 2 dimensions.
    Vector3 Velocity; //3d movement
    CharacterController characterController;
    float Gravity = -9.81f;


    public void SetMovementInput(Vector2 inputVal)
    {
        moveInput = inputVal;
    }
    public void ClearVerticalVelocity()
    {
        Velocity.y = 0;
    }

    public void SetClimbingInfo(Vector3 ladderDir, bool climbing)
    {

        LadderDir = ladderDir;
        isClimbing = climbing;
    }



    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isClimbing)
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


    bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, GroundLayerMask); //checking if we're touching something with ground layer
    }

    public IEnumerator MoveToTransform(Transform destiation, float transformTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = destiation.position;
        Quaternion StartRot = transform.rotation;
        Quaternion EndRot = destiation.rotation;

        float timer = 0f;
        while (timer < transformTime)
        {
            timer += Time.deltaTime;
            //movement, character controller requires the vector 3.
            Vector3 deltaMove = Vector3.Lerp(startPosition, endPosition, timer / transformTime) - transform.position;
            characterController.Move(deltaMove);
            //rotation, doesn't require, just rotates the transform
            transform.rotation = Quaternion.Lerp(StartRot, EndRot, timer / transformTime);
            yield return new WaitForEndOfFrame();

        }


      
    }

    void CalculateClimingVelocity()
    {
        if (moveInput.magnitude == 0)
        {
            Velocity = Vector3.zero;
            return;
        }
        Vector3 PlayerDesiredMoveDirection = GetPlayerDesiredMoveDirection();
        LadderDir = -transform.right;


        float Dot = Vector3.Dot(LadderDir, PlayerDesiredMoveDirection);

        Velocity = Vector3.zero;

        if (Dot < 0)
        {
            Velocity = GetPlayerDesiredMoveDirection() * movementSpeed;
            Velocity.y = movementSpeed;
        }
        else
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

    public Vector3 GetPlayerDesiredMoveDirection()
    {
        return new Vector3(-moveInput.y, 0f, moveInput.x).normalized; 
    }

    void UpdateRotation()
    {

        if (isClimbing)
        {
            return;
        }


        Vector3 PlayerDesiredDirection = GetPlayerDesiredMoveDirection(); 
        if (PlayerDesiredDirection.magnitude == 0)
        {
            PlayerDesiredDirection = transform.forward;
        }


        Quaternion DesiredRotation = Quaternion.LookRotation(PlayerDesiredDirection, Vector3.up); 
        transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, Time.deltaTime * turnSpeed);

    }
}
