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
    public Vector2 MousePosition = new Vector2();

    public GameObject AimingSphere;
    
    public InputActionAsset InputAssets;

    public GameObject PlayerCamera;
    public GameObject VCam;
    public GameObject ProjectilePrefab;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> LookAtPosition = new NetworkVariable<Vector3>();

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            MoveServerRpc(InputDirection);
            ManageAim(MousePosition);
        }

        if (IsServer)
        {
            transform.position = Position.Value;
            transform.LookAt(LookAtPosition.Value);
        }
    }

    [Rpc(SendTo.Server)]
    private void OnClientSpawnRpc()
    {
        Position.Value = GameManager.Instance.PlayerSpawnPosition.position;
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRpc(Vector2 inputDirection)
    {
        var position = transform.position;
        inputDirection.Normalize();
        position.x += inputDirection.x * 40 * NetworkManager.ServerTime.FixedDeltaTime;
        position.z += inputDirection.y * 40 * NetworkManager.ServerTime.FixedDeltaTime;
        Position.Value = position;
    }

    [Rpc(SendTo.Server)]
    private void SetAimDirectionRpc(Vector3 inputPosition)
    {
        var targetPosition = inputPosition;
        targetPosition.y = transform.position.y;
        LookAtPosition.Value = inputPosition;
    }

    [Rpc(SendTo.Server)]
    private void FireRpc(bool isShooting)
    {
        if (isShooting)
        {
            var instance = Instantiate(ProjectilePrefab, transform.position + transform.forward , transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
        }
    }

    private void ManageAim(Vector2 inputDirection)
    {
        Vector3 screenPosition3 = new Vector3(inputDirection.x, inputDirection.y, PlayerCamera.GetComponent<Camera>().nearClipPlane + 15) ;
        
        Vector3 worldPosition = PlayerCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPosition3);
        var targetPosition = worldPosition;
        targetPosition.y = transform.position.y;
        AimingSphere.transform.position = targetPosition;

        SetAimDirectionRpc(worldPosition);
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            OnClientSpawnRpc();
            InputAssets.Enable();
            var moveAction = InputAssets.FindAction("Move");
            var aimAction = InputAssets.FindAction("Aiming");
            var fireAction = InputAssets.FindAction("Fire");
            Debug.Log(moveAction);
            moveAction.started += context =>InputDirection = context.ReadValue<Vector2>();
            moveAction.performed += context => InputDirection = context.ReadValue<Vector2>();
            moveAction.canceled += context => InputDirection = context.ReadValue<Vector2>();
            aimAction.started += context => MousePosition = context.ReadValue<Vector2>();
            aimAction.performed += context => MousePosition = context.ReadValue<Vector2>();
            aimAction.canceled += context => MousePosition = context.ReadValue<Vector2>();
            fireAction.started += context => FireRpc(true);
            fireAction.performed += context => FireRpc(false);
            fireAction.canceled += context => FireRpc(false);
            PlayerCamera.SetActive(true);
            VCam.SetActive(true);
            VCam.transform.SetParent(null);
        }
    }
}
