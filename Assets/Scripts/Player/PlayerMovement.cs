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
    public float groundDrag;
    public float jumpHeight;
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
    private bool isTogglePlayerList;
    private bool isToggleSprint;
    private bool isToggleCrouch;

    public InputActionMap playerActionMap;
    public InputActionMap uiActionMap;

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
    InputAction inventoryPrevious;
    InputAction inventoryNext;
    InputAction inventoryScroll;
    InputAction inventorySlotOneAction;
    InputAction inventorySlotTwoAction;
    InputAction inventorySlotThreeAction;
    InputAction inventorySlotFourAction;

    public void AssignInputActions()
    {
        playerActionMap = playerInput.actions.FindActionMap("Player");
        uiActionMap = playerInput.actions.FindActionMap("UI");

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
        inventoryPrevious = playerInput.actions.FindAction("InventoryPrevious");
        inventoryNext = playerInput.actions.FindAction("InventoryNext");
        inventoryScroll = playerInput.actions.FindAction("InventoryScroll");
        inventorySlotOneAction = playerInput.actions.FindAction("InventorySlot1");
        inventorySlotTwoAction = playerInput.actions.FindAction("InventorySlot2");
        inventorySlotThreeAction = playerInput.actions.FindAction("InventorySlot3");
        inventorySlotFourAction = playerInput.actions.FindAction("InventorySlot4");
    }

    public void SetPlayerInputMap(bool isPlayerInput)
    {
        Debug.Log("Current action map: " + playerInput.currentActionMap);

        if (isPlayerInput)
        {
            Debug.Log("Switching to player input map.");
            uiActionMap.Disable();
            playerActionMap.Enable();
            Debug.Log(moveAction + " is enabled? " + moveAction.enabled);
        }
        else
        {
            Debug.Log("Switching to UI input map.");
            uiActionMap.Enable();
            playerActionMap.Disable();
        }
    }

    public void ToggleActionEvents(bool enable)
    {
        if (enable)
        {
            pauseAction.performed += PauseEvent;
            playerListAction.performed += PlayerListEvent;
            jumpAction.performed += JumpEvent;
            inventoryScroll.performed += InventoryScrollEvent;
            sprintAction.performed += SprintEvent;
            crouchAction.performed += CrouchEvent;
        }
        else
        {
            pauseAction.performed -= PauseEvent;
            playerListAction.performed -= PlayerListEvent;
            jumpAction.performed -= JumpEvent;
            inventoryScroll.performed -= InventoryScrollEvent;
            sprintAction.performed -= SprintEvent;
            crouchAction.performed -= CrouchEvent;
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

    void JumpEvent(InputAction.CallbackContext context)
    {
        if (context.interaction is PressInteraction
            || context.interaction is HoldInteraction
            || context.interaction is TapInteraction
            && isGrounded)
        {
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
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

    public void PlayerInput()
    {
        Debug.Log(moveAction + " is enabled at beginning of player input? " + moveAction.enabled);
        Debug.Log(playerActionMap + " is enabled at beginning of player input? " + playerActionMap.enabled);
        Debug.Log(uiActionMap + " is enabled at beginning of player input? " + uiActionMap.enabled);
        if (isPaused)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (interactAction.IsPressed())
        {
            Interact();
        }

        if (inventorySlotOneAction.IsPressed())
        {
            inventorySlot = 0;
        }

        if (inventorySlotTwoAction.IsPressed())
        {
            inventorySlot = 1;
        }

        if (inventorySlotThreeAction.IsPressed())
        {
            inventorySlot = 2;
        }

        if (inventorySlotFourAction.IsPressed())
        {
            inventorySlot = 3;
        }

        if (dropAction.IsPressed())
        {
            DropItem();
        }
        
        if (attackAction.IsPressed())
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

        if (useAction.IsPressed())
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

        if (alternateAction.IsPressed())
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

        if (lightAction.IsPressed())
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

        if (inventoryPrevious.IsPressed())
        {
            inventorySlot = (inventorySlot - 1 + inventory.Length) % inventory.Length;
        }

        if (inventoryNext.IsPressed())
        {
            inventorySlot = (inventorySlot + 1) % inventory.Length;
        }

        Debug.Log(moveAction + " is enabled at end of playerinput? " + moveAction.enabled);
        Debug.Log(playerActionMap + " is enabled at end of player input? " + playerActionMap.enabled);
        Debug.Log(uiActionMap + " is enabled at end of player input? " + uiActionMap.enabled);
    }

    public void MovePlayer()
    {
        // calc move direction
        IsGroundedCheck();

        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        moveDirection = moveDirection.normalized;
        
        rb.AddForce(10f * baseMovementSpeed * moveDirection, ForceMode.Force);

        if (jumpInput && isGrounded)
        {
            rb.AddForce(0, jumpHeight, 0, ForceMode.Impulse);
            jumpInput = false;
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
