using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionPanelController : MonoBehaviour
{
    public TMP_InputField AddressInput;

    public Button ConnectButton;
    // Start is called before the first frame update
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
    void Start()
    {
        ConnectButton.onClick.AddListener(ClientConnection);

        NetworkManager.Singleton.OnClientStarted += () =>
        {
            Debug.Log("Client started");
        };

        NetworkManager.Singleton.OnClientConnectedCallback += obj =>  OnCientConnected(obj);

    }

    private void OnCientConnected(ulong obj)
    {
        _actions.Enqueue(() =>
        {
            Debug.Log("Client Connected \nLoad scene Gameplay");
            NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
        });
    }

    private void ClientConnection()
    {
        string address = AddressInput.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = address;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = 7777;
        Debug.Log("Try launch connection on address: " + address +":" + 7777);
        
        NetworkManager.Singleton.StartClient();
    }

    private void Update()
    {
        while(_actions.Count > 0)
        {
            if(_actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}
