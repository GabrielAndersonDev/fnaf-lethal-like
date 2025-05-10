using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> CamRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> PlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            
            playerCam = GetComponentInChildren<PlayerCam>();
            cam = GetComponentInChildren<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rb.freezeRotation = true;
            rb.isKinematic = false;
            itemManager = FindObjectOfType<ItemManager>();
            InventoryInit();
            PlayerInit(playerData, 1);
            Debug.Log("isowner");
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            rb.isKinematic = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var newPos = MovePlayer().position;
        var newCamRot = playerCam.CameraInput().rotation;
        var newPlayerRot = playerCam.orientation.rotation;

        transform.position = newPos;
        Position.Value = newPos;

        cam.transform.rotation = newCamRot;
        CamRotation.Value = newCamRot;

        transform.rotation = newPlayerRot;
        PlayerRotation.Value = newPlayerRot;
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
        
        transform.position = Position.Value;
        cam.transform.rotation = CamRotation.Value;
        transform.rotation = PlayerRotation.Value;
    }
}
