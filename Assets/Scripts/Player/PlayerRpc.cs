using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    public NetworkVariable<Quaternion> CamRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rb.freezeRotation = true;
            rb.isKinematic = false;

            itemManager = FindObjectOfType<ItemManager>();
            InventoryInit();
            PlayerInit(playerData, 1);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            rb.isKinematic = true;
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
