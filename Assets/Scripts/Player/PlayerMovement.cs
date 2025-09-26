using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : NetworkBehaviour
{
    [Header("Key Inputs")]
    public Dictionary<string, KeyCode> keyDictionary;

    [Header("Movement Physics")]
    public float groundDrag;
    public float jumpHeight;
    float horizontalInput;
    float verticalInput;
    public bool allowedToMove;
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
    public float groundDistance;

    public bool isPaused = false;
    public bool isPlayerListOpen = false;
    private bool isTogglePlayerList;

    public void PlayerInput()
    {
        if (Input.GetKeyDown(keyDictionary["pauseKey"]))
        {
            UIManager.Singleton.TogglePause();
        }

        if (isPaused)
        {
            return;
        }

        if (isTogglePlayerList
            && Input.GetKey(keyDictionary["playerListKey"]))
        {
            isPlayerListOpen = !isPlayerListOpen;
            NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
        }

        if (!isTogglePlayerList
            && Input.GetKeyDown(keyDictionary["playerListKey"]))
        {
            isPlayerListOpen = true;
            NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
        }

        if (!isTogglePlayerList
            && Input.GetKeyUp(keyDictionary["playerListKey"]))
        {
            isPlayerListOpen = false;
            NetworkUIScript.Singleton.ToggleDisplayPlayerList(isPlayerListOpen);
        }

        verticalKeys = (Input.GetKey(keyDictionary["forwardKey"]) && Input.GetKey(keyDictionary["backwardKey"]));
        horizontalKeys = (Input.GetKey(keyDictionary["rightKey"]) && Input.GetKey(keyDictionary["leftKey"]));

        if (verticalKeys) 
        {
            verticalInput = 0;
        } 
        else if (Input.GetKey(keyDictionary["forwardKey"])) 
        {
            verticalInput = 1;
        } 
        else if (Input.GetKey(keyDictionary["backwardKey"])) 
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
        else if (Input.GetKey(keyDictionary["rightKey"])) 
        {
            horizontalInput = 1;
        } 
        else if (Input.GetKey(keyDictionary["leftKey"])) 
        {
            horizontalInput = -1;
        } 
        else 
        {
            horizontalInput = 0;
        }

        if (Input.GetKey(keyDictionary["jumpKey"]) && isGrounded) 
        {
            jumpInput = true;
        } 
        else
        {     
            jumpInput = false;
        }

        if (Input.GetKeyDown(keyDictionary["interactKey"]))
        {
            Interact();
        }

        if (Input.GetKeyDown(keyDictionary["inventorySlotOne"]))
        {
            inventorySlot = 0;
        }

        if (Input.GetKeyDown(keyDictionary["inventorySlotTwo"]))
        {
            inventorySlot = 1;
        }

        if (Input.GetKeyDown(keyDictionary["inventorySlotThree"]))
        {
            inventorySlot = 2;
        }

        if (Input.GetKeyDown(keyDictionary["inventorySlotFour"]))
        {
            inventorySlot = 3;
        }

        if (Input.GetKeyDown(keyDictionary["dropKey"]))
        {
            DropItem();
        }
        
        if (Input.GetKeyDown(keyDictionary["attackKey"]))
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

        if (Input.GetKeyDown(keyDictionary["useKey"]))
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

        if (Input.GetKeyDown(keyDictionary["alternateKey"]))
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

        if (Input.GetKeyDown(keyDictionary["lightKey"]))
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
