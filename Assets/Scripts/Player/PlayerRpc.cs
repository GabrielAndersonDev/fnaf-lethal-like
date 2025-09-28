using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<Quaternion> CamRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    NetworkTransform networkTransform;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rb.freezeRotation = true;
            rb.isKinematic = false;

            InventoryInit();
            PlayerInit();
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            rb.isKinematic = true;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            CamRotation.Value = playerCam.transform.rotation;
            MovePlayer();
        }
        else
        {
            playerCam.transform.rotation = CamRotation.Value;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            playerCam.CameraInput();
            PlayerInput();
        }
    }
}
