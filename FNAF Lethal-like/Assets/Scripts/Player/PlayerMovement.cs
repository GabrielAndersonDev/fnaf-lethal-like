using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{

    [Header("Movement")]
    public bool allowed_to_move;
    public Transform orientation;
    public KeyCode forward_key = KeyCode.W;
    public KeyCode backward_key = KeyCode.S;
    public KeyCode left_key = KeyCode.A;
    public KeyCode right_key = KeyCode.D;
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode use_key = KeyCode.Mouse1;
    public KeyCode attack_key = KeyCode.Mouse0;
    public KeyCode interact_key = KeyCode.E;
    public KeyCode drop_key = KeyCode.Q;
    public KeyCode alternate_key = KeyCode.R;
    public KeyCode pause_key = KeyCode.Escape;

    public float groundDrag;
    public float jumpHeight = 0.8f;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    bool grounded;  
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    bool verticalKeys;
    bool horizontalKeys;
    bool jumpInput;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        PlayerInput();

        // handle drag

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        verticalKeys = (Input.GetKey(forward_key) && Input.GetKey(backward_key));
        horizontalKeys = (Input.GetKey(right_key) && Input.GetKey(left_key));

        if (verticalKeys) {
            verticalInput = 0;
        } else if (Input.GetKey(forward_key)) {
            verticalInput = 1;
        } else if (Input.GetKey(backward_key)) {
            verticalInput = -1;
        } else {
            verticalInput = 0;
        }

        if (horizontalKeys) {
            horizontalInput = 0;
        } else if (Input.GetKey(right_key)) {
            horizontalInput = 1;
        } else if (Input.GetKey(left_key)) {
            horizontalInput = -1;
        } else {
            horizontalInput = 0;
        }

        if (Input.GetKey(jump_key) && grounded) {
            jumpInput = true;
        } else
        {     
            jumpInput = false;
        }

        if (Input.GetKeyDown(interact_key))
        {

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
