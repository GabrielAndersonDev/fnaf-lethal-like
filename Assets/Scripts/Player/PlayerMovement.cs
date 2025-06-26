using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : NetworkBehaviour
{
    [Header("Key Inputs")]
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode useKey;
    public KeyCode attackKey;
    public KeyCode interactKey;
    public KeyCode dropKey;
    public KeyCode alternateKey;
    public KeyCode lightKey ;
    public KeyCode pauseKey;
    public KeyCode inventorySlotOne;
    public KeyCode inventorySlotTwo;
    public KeyCode inventorySlotThree;
    public KeyCode inventorySlotFour;
    // add toggle option in settings for sprinting

    [Header("Movement Physics")]
    public float groundDrag;
    public float jumpHeight;
    float horizontalInput;
    float verticalInput;
    public bool allowed_to_move;
    bool verticalKeys;
    bool horizontalKeys;
    bool jumpInput;
    Vector3 moveDirection;
    [SerializeField]
    Rigidbody rb;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public bool isPaused = false;

    public void PlayerInput()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            isPaused = UIManager.Singleton.TogglePause();

            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (isPaused)
        {
            return;
        }

        verticalKeys = (Input.GetKey(forwardKey) && Input.GetKey(backwardKey));
        horizontalKeys = (Input.GetKey(rightKey) && Input.GetKey(leftKey));

        if (verticalKeys) 
        {
            verticalInput = 0;
        } 
        else if (Input.GetKey(forwardKey)) 
        {
            verticalInput = 1;
        } 
        else if (Input.GetKey(backwardKey)) 
        {
            verticalInput = -1;
        } 
        else 
        {
            verticalInput = 0;
        }

        if (horizontalKeys) 
        {
            horizontalInput = 0;
        } 
        else if (Input.GetKey(rightKey)) 
        {
            horizontalInput = 1;
        } 
        else if (Input.GetKey(leftKey)) 
        {
            horizontalInput = -1;
        } 
        else 
        {
            horizontalInput = 0;
        }

        if (Input.GetKey(jumpKey) && isGrounded) 
        {
            jumpInput = true;
        } 
        else
        {     
            jumpInput = false;
        }

        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }

        if (Input.GetKeyDown(inventorySlotOne))
        {
            inventorySlot = 0;
        }

        if (Input.GetKeyDown(inventorySlotTwo))
        {
            inventorySlot = 1;
        }

        if (Input.GetKeyDown(inventorySlotThree))
        {
            inventorySlot = 2;
        }

        if (Input.GetKeyDown(inventorySlotFour))
        {
            inventorySlot = 3;
        }

        if (Input.GetKeyDown(dropKey))
        {
            RemoveItem();
        }
        
        if (Input.GetKeyDown(attackKey))
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

        if (Input.GetKeyDown(useKey))
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

        if (Input.GetKeyDown(alternateKey))
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

        if (Input.GetKeyDown(lightKey))
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].UseLight();
            } else
            {
                Debug.Log("Empty inventory slot.");
            }
        }
    }

    public void MovePlayer()
    {
        // calc move direction
        IsGroundedCheck();

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection = moveDirection.normalized;
        
        rb.AddForce(10f * baseMovementSpeed * moveDirection, ForceMode.Force);

        if (jumpInput && isGrounded)
        {
            rb.AddForce(0, jumpHeight, 0, ForceMode.Impulse);
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
