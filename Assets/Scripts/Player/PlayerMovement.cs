using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : NetworkBehaviour
{
    [Header("Key Inputs")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode useKey = KeyCode.Mouse1;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode alternateKey = KeyCode.R;
    public KeyCode lightKey = KeyCode.F;
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode inventorySlotOne = KeyCode.Alpha1;
    public KeyCode inventorySlotTwo = KeyCode.Alpha2;
    public KeyCode inventorySlotThree = KeyCode.Alpha3;
    public KeyCode inventorySlotFour = KeyCode.Alpha4;

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

    public void PlayerInput()
    {
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
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = groundDrag;
        }
    }
}
