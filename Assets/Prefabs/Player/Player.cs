using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    
    
    [SerializeField] Transform PickupSocketInfo;
    InputActions inputActions;
    
    
    MovementComponent movementComp;
    LadderClimbing ladderClimbing;

    public Transform GetPickupSocketTransform()
    {
        return PickupSocketInfo;
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
        
        movementComp = GetComponent<MovementComponent>();
        ladderClimbing = GetComponent<LadderClimbing>();
        ladderClimbing.SetInput(inputActions);
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
        movementComp.SetMovementInput(context.ReadValue<Vector2>());  


    }

 

}
