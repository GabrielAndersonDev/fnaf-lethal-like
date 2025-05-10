using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    //public NetworkVariable<Quaternion> CamRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    //public NetworkVariable<Quaternion> PlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    public Vector3 Position = new();
    public Quaternion CamRotation = new();
    public Quaternion PlayerRotation = new();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rb.freezeRotation = true;
            rb.isKinematic = false;

            Position = transform.position;
            CamRotation = playerCam.transform.rotation;
            PlayerRotation = playerCam.orientation.rotation;

            itemManager = FindObjectOfType<ItemManager>();
            InventoryInit();
            PlayerInit(playerData, 1);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            rb.isKinematic = true;

            Position = transform.position;
            CamRotation = playerCam.transform.rotation;
            PlayerRotation = playerCam.orientation.rotation;
            //transform.position = Position;
            //playerCam.transform.rotation = CamRotation;
            //playerCam.orientation.rotation = PlayerRotation;
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var newPos = MovePlayer().position;
        var newCamRot = playerCam.CameraInput().rotation;
        var newPlayerRot = playerCam.orientation.rotation;

        transform.position = newPos;
        Position = newPos;

        playerCam.transform.rotation = newCamRot;
        CamRotation = newCamRot;

        transform.rotation = newPlayerRot;
        PlayerRotation = newPlayerRot;
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            PlayerInput();
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            SubmitPositionRequestRpc();
        }
        
        transform.position = Position;
        playerCam.transform.rotation = CamRotation;
        transform.rotation = PlayerRotation;
    }
}
