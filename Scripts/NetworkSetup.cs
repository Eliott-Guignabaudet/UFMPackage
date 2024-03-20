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

#else
        LoadSceneClient();
#endif
        
    }

    private void StartServer()
    {
        NetworkManager.Singleton.OnServerStarted += () => Debug.Log("Server Started");
        NetworkManager.Singleton.StartServer();
    }

    private void LoadSceneClient()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
