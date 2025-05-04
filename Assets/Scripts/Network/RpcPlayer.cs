using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RpcPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerSpawn();
        }
    }

    public void PlayerSpawn()
    {
        SubmitPositionRequestRpc();
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {

    }
}
