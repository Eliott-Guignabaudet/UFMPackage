using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SERVER
        StartServer(); 
        NetworkManager.Singleton.OnClientConnectedCallback += obj =>
        {
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log(id);
            }
        };
#else
        LoadSceneClient();
#endif
        
    }

    private void StartServer()
    {
        NetworkManager.Singleton.OnServerStarted += () => Debug.Log("Server Started");
        NetworkManager.Singleton.StartServer();
        //NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.LoadScene("DedicatedServerScene", LoadSceneMode.Single);
    }

    private void LoadSceneClient()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
