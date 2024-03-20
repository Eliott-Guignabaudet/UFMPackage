using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [ReadOnly]
    public Vector2 InputDirection = new Vector2();
    public InputActionAsset InputAssets;

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            MoveRpc(InputDirection);
        }

        if (IsServer)
        {
            transform.position = Position.Value;
        }
    }

    [Rpc(SendTo.Server)]
    private void OnClientSpawnRpc()
    {
        Debug.Log("Client Spawned");
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc(Vector2 inputDirection)
    {
        var position = transform.position;
        inputDirection.Normalize();
        position.x += inputDirection.x * 10 * NetworkManager.ServerTime.FixedDeltaTime;
        position.z += inputDirection.y * 10 * NetworkManager.ServerTime.FixedDeltaTime;
        Position.Value = position;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            OnClientSpawnRpc();
            InputAssets.Enable();
            var moveAction = InputAssets.FindAction("Move");
            Debug.Log(moveAction);
            moveAction.started += context =>InputDirection = context.ReadValue<Vector2>();
            moveAction.performed += context => InputDirection = context.ReadValue<Vector2>();
            moveAction.canceled += context => InputDirection = context.ReadValue<Vector2>();
            
        }
    }
}
