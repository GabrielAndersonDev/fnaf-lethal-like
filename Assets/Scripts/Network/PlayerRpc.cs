using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRpc : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new();

    [SerializeField]
    Player player;

    public override void OnNetworkSpawn()
    {
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
        var newPos = player.MovePlayer().position;
        transform.position = newPos;
        Position.Value = newPos;
    }

    private void Update()
    {
        transform.position = Position.Value;
    }
}
