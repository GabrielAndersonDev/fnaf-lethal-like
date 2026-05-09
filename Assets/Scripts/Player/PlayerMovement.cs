using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public partial class Player : NetworkBehaviour
{
    [Header("Key Inputs")]
    public Dictionary<string, KeyCode> keyDictionary;

    [Header("Movement Physics")]
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float jumpHeight;
    Vector2 moveInput;
    public bool allowedToMove;
    bool jumpInput;
    Vector3 moveDirection;

    public Vector3 MoveDirection
    {
        get { return moveDirection; }
    }

    [SerializeField]
    Rigidbody rb;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance;

    public bool isPaused = false;
    public bool isPlayerListOpen = false;
    private bool isJumpKeyHeld;
    private bool isJumpQueued;
    private bool isTogglePlayerList;
    private bool isToggleSprint;
    private bool isToggleCrouch;

    public PlayerInput playerInput;

    InputAction moveAction;
    InputAction attackAction;
    InputAction interactAction;
    InputAction sprintAction;
    InputAction crouchAction;
    InputAction jumpAction;
    InputAction dropAction;
    InputAction useAction;
    InputAction alternateAction;
    InputAction lightAction;
    InputAction playerListAction;
    InputAction pauseAction;
    InputAction unpauseAction;
    InputAction inventoryPrevious;
    InputAction inventoryNext;
    InputAction inventoryScroll;
    InputAction inventorySlotOneAction;
    InputAction inventorySlotTwoAction;
    InputAction inventorySlotThreeAction;
    InputAction inventorySlotFourAction;

    public void AssignInputActions()
    {
        moveAction = playerInput.actions.FindAction("Move");
        attackAction = playerInput.actions.FindAction("Attack");
        interactAction = playerInput.actions.FindAction("Interact");
        sprintAction = playerInput.actions.FindAction("Sprint");
        crouchAction = playerInput.actions.FindAction("Crouch");
        jumpAction = playerInput.actions.FindAction("Jump");
        dropAction = playerInput.actions.FindAction("Drop");
        useAction = playerInput.actions.FindAction("Use");
        alternateAction = playerInput.actions.FindAction("Alternate");
        lightAction = playerInput.actions.FindAction("Light");
        playerListAction = playerInput.actions.FindAction("PlayerList");
        pauseAction = playerInput.actions.FindAction("Pause");
        unpauseAction = playerInput.actions.FindAction("Unpause");
        inventoryPrevious = playerInput.actions.FindAction("InventoryPrevious");
        inventoryNext = playerInput.actions.FindAction("InventoryNext");
        inventoryScroll = playerInput.actions.FindAction("InventoryScroll");
        inventorySlotOneAction = playerInput.actions.FindAction("InventorySlot1");
        inventorySlotTwoAction = playerInput.actions.FindAction("InventorySlot2");
        inventorySlotThreeAction = playerInput.actions.FindAction("InventorySlot3");
        inventorySlotFourAction = playerInput.actions.FindAction("InventorySlot4");
    }

    public void ToggleActionEvents(bool enable)
    {
        if (enable)
        {
            attackAction.performed += AttackEvent;
            interactAction.performed += InteractEvent;
            dropAction.performed += DropEvent;
            useAction.performed += UseEvent;
            alternateAction.performed += AlternateEvent;
            lightAction.performed += LightEvent;
            pauseAction.performed += PauseEvent;
            unpauseAction.performed += PauseEvent;
            playerListAction.performed += PlayerListEvent;
            jumpAction.started += JumpStartEvent;
            jumpAction.canceled += JumpCancelEvent;
            inventoryScroll.performed += InventoryScrollEvent;
            sprintAction.performed += SprintEvent;
            crouchAction.performed += CrouchEvent;
            inventoryPrevious.performed += InventoryPreviousAction;
            inventoryNext.performed += InventoryNextAction;
            inventorySlotOneAction.performed += InventorySlotOneAction;
            inventorySlotTwoAction.performed += InventorySlotTwoAction;
            inventorySlotThreeAction.performed += InventorySlotThreeAction;
            inventorySlotFourAction.performed += InventorySlotFourAction;
        }
        else
        {
            attackAction.performed -= AttackEvent;
            interactAction.performed -= InteractEvent;
            dropAction.performed -= DropEvent;
            useAction.performed -= UseEvent;
            alternateAction.performed -= AlternateEvent;
            lightAction.performed -= LightEvent;
            pauseAction.performed -= PauseEvent;
            unpauseAction.performed -= PauseEvent;
            playerListAction.performed -= PlayerListEvent;
            jumpAction.started -= JumpStartEvent;
            jumpAction.canceled -= JumpCancelEvent;
            inventoryScroll.performed -= InventoryScrollEvent;
            sprintAction.performed -= SprintEvent;
            crouchAction.performed -= CrouchEvent;
            inventoryPrevious.performed -= InventoryPreviousAction;
            inventoryNext.performed -= InventoryNextAction;
            inventorySlotOneAction.performed -= InventorySlotOneAction;
            inventorySlotTwoAction.performed -= InventorySlotTwoAction;
            inventorySlotThreeAction.performed -= InventorySlotThreeAction;
            inventorySlotFourAction.performed -= InventorySlotFourAction;
        }
    }

    void AttackEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].ItemAttack();

            }
            else
            {
                Debug.Log("Empty inventory slot.");
            }
        }
    }

    void UseEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].UseItem();

            }
            else
            {
                Debug.Log("Empty inventory slot.");
            }
        }
    }

    void AlternateEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].UseAlt();
            }
            else
            {
                Debug.Log("Empty inventory slot.");
            }
        }
    }

    void LightEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].UseLight();
            }
            else
            {
                Debug.Log("Empty inventory slot.");
            }
        }
    }

    void InteractEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            Interact();
        }
    }

    void DropEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            DropItem();
        }
    }

    void PauseEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            UIManager.Singleton.TogglePause();
        }
    }

    void PlayerListEvent(InputAction.CallbackContext context)
    {
        if (isTogglePlayerList)
        {
            if (context.interaction is TapInteraction)
            {
                isPlayerListOpen = !isPlayerListOpen;
                NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
            }
        }
        else
        {
            if (context.interaction is PressInteraction)
            {
                isPlayerListOpen = !isPlayerListOpen;
                NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
            }
        }
    }

    void JumpStartEvent(InputAction.CallbackContext context)
    {
        if (context.phase is InputActionPhase.Started)
        {
            isJumpKeyHeld = true;
            isJumpQueued = true;
        }
    }

    void JumpCancelEvent(InputAction.CallbackContext context)
    {
        if (context.phase is InputActionPhase.Canceled)
        {
            isJumpKeyHeld = false;
        }
    }

    void InventoryScrollEvent(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        if (scrollValue > 0)
        {
            inventorySlot = (inventorySlot + 1) % inventory.Length;
        }
        else if (scrollValue < 0)
        {
            inventorySlot = (inventorySlot - 1 + inventory.Length) % inventory.Length;
        }
    }

    void SprintEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            Debug.LogWarning("Sprint is currently unusable. Sprint is toggled " + (isToggleSprint ? "on." : "off."));
        }
    }

    void CrouchEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            Debug.LogWarning("Crouch is currently unusable. Crouch is toggled " + (isToggleCrouch ? "on." : "off."));
        }
    }

    void InventorySlotOneAction(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            inventorySlot = 0;
        }
    }

    void InventorySlotTwoAction(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            inventorySlot = 1;
        }
    }

    void InventorySlotThreeAction(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            inventorySlot = 2;
        }
    }

    void InventorySlotFourAction(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction
            || context.interaction is PressInteraction)
        {
            inventorySlot = 3;
        }
    }

    void InventoryPreviousAction(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            inventorySlot = (inventorySlot - 1 + inventory.Length) % inventory.Length;
        }
    }

    void InventoryNextAction(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction)
        {
            inventorySlot = (inventorySlot + 1) % inventory.Length;
        }
    }

    public void PlayerInput()
    {
        if (isPaused)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();
    }

    public void MovePlayer()
    {
        // calc move direction
        IsGroundedCheck();

        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        moveDirection = moveDirection.normalized;
        
        rb.AddForce(10f * baseMovementSpeed * moveDirection, ForceMode.Force);

        if (isGrounded
            && isJumpKeyHeld)
        {
            isJumpQueued = true;
        }
        else if (!isJumpKeyHeld)
        {
            isJumpQueued = false;
        }

        if (isGrounded 
            && isJumpQueued)
        {
            isJumpQueued = false;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(
                Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y),
                ForceMode.VelocityChange
            );
        }
    }

    public void IsGroundedCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = groundDrag;
        }
    }
}
