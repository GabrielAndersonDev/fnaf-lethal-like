using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
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
    public float jumpHeight = 0.8f;
    float horizontalInput;
    float verticalInput;
    public bool allowed_to_move;
    bool verticalKeys;
    bool horizontalKeys;
    bool jumpInput;
    public Transform orientation;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    bool grounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    private void PlayerInput()
    {
        verticalKeys = (Input.GetKey(forwardKey) && Input.GetKey(backwardKey));
        horizontalKeys = (Input.GetKey(rightKey) && Input.GetKey(leftKey));

        if (verticalKeys) {
            verticalInput = 0;
        } else if (Input.GetKey(forwardKey)) {
            verticalInput = 1;
        } else if (Input.GetKey(backwardKey)) {
            verticalInput = -1;
        } else {
            verticalInput = 0;
        }

        if (horizontalKeys) {
            horizontalInput = 0;
        } else if (Input.GetKey(rightKey)) {
            horizontalInput = 1;
        } else if (Input.GetKey(leftKey)) {
            horizontalInput = -1;
        } else {
            horizontalInput = 0;
        }

        if (Input.GetKey(jumpKey) && grounded) {
            jumpInput = true;
        } else
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
                Debug.LogError("Empty inventory slot.");
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
                Debug.LogError("Empty inventory slot.");
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
                Debug.LogError("Empty inventory slot.");
            }
        }

        if (Input.GetKeyDown(lightKey))
        {
            if (inventory[inventorySlot] != null)
            {
                inventory[inventorySlot].UseLight();
            } else
            {
                Debug.LogError("Empty inventory slot.");
            }
        }
    }

    private void MovePlayer()
    {
        // calc move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        rb.AddForce(10f * BaseMovementSpeed * moveDirection.normalized, ForceMode.Force);

        if (jumpInput)
        {
            rb.AddForce(0, jumpHeight, 0, ForceMode.Impulse);
        }
    }
}
