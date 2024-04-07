using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ZombiController : NetworkBehaviour
{
    public NetworkVariable<float> HP;

    [SerializeField] private NetworkObject _networkObjectRef;
    

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(float damage)
    {
        HP.Value -= damage;
        if (HP.Value <= 0)
        {
            _networkObjectRef.Despawn();
        }
    }

    [Rpc(SendTo.Server)]
    private void InitRpc()
    {
        HP.Value = 10;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            InitRpc();
        }
    }
}
