using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRpc : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> CamRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> PlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    Player player;
    [SerializeField]
    PlayerCam playerCam;
    [SerializeField]
    Camera cam;

    public override void OnNetworkSpawn()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rb.isKinematic = false;
            Debug.Log("isowner");
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var newPos = player.MovePlayer().position;
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
        if (!IsOwner) return;

        Transform newTrans = player.MovePlayer();
        Position.Value = newTrans.position;
        CamRotation.Value = playerCam.CameraInput().rotation;
        PlayerRotation.Value = playerCam.orientation.rotation;
    }

    private void Update()
    {
        if (IsOwner)
        {
            player.PlayerInput();
        }
        
        transform.position = Position.Value;
        cam.transform.rotation = CamRotation.Value;
        transform.rotation = PlayerRotation.Value;
    }
}
