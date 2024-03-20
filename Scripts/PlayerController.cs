using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Rpc(SendTo.Server)]
    private void OnClientSpawnRpc()
    {
        Debug.Log("Client Spawned");
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            OnClientSpawnRpc();
        }
    }
}
