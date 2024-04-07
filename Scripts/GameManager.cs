using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public Transform PlayerSpawnPosition;
    public Transform[] ZombiSpawners;
    public GameObject ZombiPrefab;
    public float ZombiSpawnCoolDown;
    private float _timeElpasedSinceLastZombiSpawn;
    
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            ManageZombiSpawnRpc();
        }
    }
    [Rpc(SendTo.Server)]
    private void ManageZombiSpawnRpc()
    {
        _timeElpasedSinceLastZombiSpawn += Time.deltaTime;
        if (_timeElpasedSinceLastZombiSpawn >= ZombiSpawnCoolDown / NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            int spawnerIndex = Random.Range(0, ZombiSpawners.Length);
            var zombi = Instantiate(ZombiPrefab, ZombiSpawners[spawnerIndex]);
            zombi.GetComponent<NetworkObject>().Spawn();
            _timeElpasedSinceLastZombiSpawn = 0;
        }
    }
}
