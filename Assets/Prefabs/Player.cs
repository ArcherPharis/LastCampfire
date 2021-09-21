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
    InputActions inputActions;
    Vector2 moveInput; //because the controls on the keyboard only work in 2 dimensions.
    Vector3 Velocity; //3d movement
    CharacterController characterController;
    float Gravity = -9.81f;

    bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, GroundLayerMask);
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

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Velocity);

        if (IsOnGround())
        {
            Velocity.y = -0.2f;
        }
        Velocity.x = GetPlayerDesiredMoveDirection().x * movementSpeed;
        Velocity.z = GetPlayerDesiredMoveDirection().z * movementSpeed;
        Velocity.y += Gravity * Time.deltaTime;
        characterController.Move(Velocity * Time.deltaTime);

    }

    Vector3 GetPlayerDesiredMoveDirection()
    {
        return new Vector3(-moveInput.y, 0f, moveInput.x).normalized; //normalized keeps it to one button press
    }
}
