using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public Team team = Team.Player;
    Vector3 playerLocation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerLocation = rb.transform.position;
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
}
