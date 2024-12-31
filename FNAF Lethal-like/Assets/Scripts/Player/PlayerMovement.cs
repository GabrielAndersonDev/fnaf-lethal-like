using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

partial class Player : Entity
{
    public Team team = Team.PLAYER;

    [Header("Movement")]
    public bool allowed_to_move;
    public Transform orientation;
    public KeyCode forward_key;
    public KeyCode backward_key;
    public KeyCode left_key;
    public KeyCode right_key;
    public KeyCode jump_key;
    public KeyCode use_key;
    public KeyCode interact_key;
    public KeyCode drop_key;
    public KeyCode alternate_key;
    public KeyCode pause_key;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        myInput();

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

    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // calc move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(10f * BaseMovementSpeed * moveDirection.normalized, ForceMode.Force);
    }
}
