using System.Collections;
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
    void Start()
    {
        ConnectButton.onClick.AddListener(ClientConnection);

        NetworkManager.Singleton.OnClientStarted += () =>
        {
            Debug.Log("Client started");
        };
        
        NetworkManager.Singleton.OnClientConnectedCallback += obj =>
        {
            Debug.Log("Client Connected \nLoad scene Gameplay");
            LoadSceneRpc();
        };
    }

    [Rpc(SendTo.Server)]
    private void LoadSceneRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }

    private void ClientConnection()
    {
        string address = AddressInput.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = address;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = 7777;
        Debug.Log("Try launch connection on address: " + address +":" + 7777);
        
        NetworkManager.Singleton.StartClient();
    }
}
