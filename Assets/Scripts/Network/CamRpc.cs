using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CamRpc : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new();
    public NetworkVariable<Quaternion> Rotation = new();

    public Transform cameraPosition;

    [SerializeField]
    PlayerCam cam;

    public override void OnNetworkSpawn()
    {
        Debug.Log("cam is alive... for now");

        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        SubmitPositionRequestRpc();
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var newPos = cameraPosition.position;
        var newRot = cam.CameraInput().rotation;

        transform.SetPositionAndRotation(newPos, newRot);

        Position.Value = newPos;
        Rotation.Value = newRot;
    }

    void Update()
    {
        cameraPosition.position = Position.Value;
        transform.rotation = Rotation.Value;
    }
}
