using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    public float speed;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            transform.position += transform.forward * speed * NetworkManager.Singleton.ServerTime.FixedDeltaTime;
        }
    }
}
