using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform PlayerSpawnPosition;
    
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
    
    
}
