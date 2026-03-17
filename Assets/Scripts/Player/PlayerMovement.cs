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
    float horizontalInput;
    float verticalInput;
    Vector2 moveInput;
    public bool allowedToMove;
    bool verticalKeys;
    bool horizontalKeys;
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

    InputAction pauseAction;
    InputAction playerListAction;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction interactAction;
    InputAction inventorySlotOneAction;
    InputAction inventorySlotTwoAction;
    InputAction inventorySlotThreeAction;
    InputAction inventorySlotFourAction;
    InputAction inventoryScrollAction;
    InputAction dropAction;
    InputAction attackAction;
    InputAction useAction;
    InputAction alternateAction;
    InputAction lightAction;

    public void AssignInputActions()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
        playerListAction = InputSystem.actions.FindAction("PlayerList");
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        interactAction = InputSystem.actions.FindAction("Interact");
        inventorySlotOneAction = InputSystem.actions.FindAction("InventorySlotOne");
        inventorySlotTwoAction = InputSystem.actions.FindAction("InventorySlotTwo");
        inventorySlotThreeAction = InputSystem.actions.FindAction("InventorySlotThree");
        inventorySlotFourAction = InputSystem.actions.FindAction("InventorySlotFour");
        inventoryScrollAction = InputSystem.actions.FindAction("InventoryScroll");
        dropAction = InputSystem.actions.FindAction("Drop");
        attackAction = InputSystem.actions.FindAction("Attack");
        useAction = InputSystem.actions.FindAction("Use");
        alternateAction = InputSystem.actions.FindAction("Alternate");
        lightAction = InputSystem.actions.FindAction("Light");
    }

    public void PlayerInput()
    {
        if (pauseAction.IsPressed())
        {
            UIManager.Singleton.TogglePause();
        }

        if (isPaused)
        {
            return;
        }

        if (isTogglePlayerList)
        {
            playerListAction.performed +=
                context =>
                {
                    if (context.interaction is TapInteraction
                    || context.interaction is PressInteraction)
                    {
                        isPlayerListOpen = !isPlayerListOpen;
                        NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
                    }
                };
        }

        if (!isTogglePlayerList)
        {
            playerListAction.performed +=
                context =>
                {
                    if (context.interaction is HoldInteraction)
                    {
                        isPlayerListOpen = true;
                        NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
                    }
                    else
                    {
                        isPlayerListOpen = false;
                        NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
                    }
                };
        }

        moveInput = moveAction.ReadValue<Vector2>();

        //verticalKeys = (moveAction && Input.GetKey(keyDictionary["backwardKey"]));
        //horizontalKeys = (Input.GetKey(keyDictionary["rightKey"]) && Input.GetKey(keyDictionary["leftKey"]));

        //if (verticalKeys) 
        //{
        //    verticalInput = 0;
        //} 
        //else if (Input.GetKey(keyDictionary["forwardKey"])) 
        //{
        //    verticalInput = 1;
        //} 
        //else if (Input.GetKey(keyDictionary["backwardKey"])) 
        //{
        //    verticalInput = -1;
        //} 
        //else 
        //{
        //    verticalInput = 0;
        //}

        //if (horizontalKeys) 
        //{
        //    horizontalInput = 0;
        //} 
        //else if (Input.GetKey(keyDictionary["rightKey"])) 
        //{
        //    horizontalInput = 1;
        //} 
        //else if (Input.GetKey(keyDictionary["leftKey"])) 
        //{
        //    horizontalInput = -1;
        //} 
        //else 
        //{
        //    horizontalInput = 0;
        //}

        jumpAction.performed +=
            context =>
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
            };

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

        inventoryScrollAction.performed +=
            context =>
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
            };

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
