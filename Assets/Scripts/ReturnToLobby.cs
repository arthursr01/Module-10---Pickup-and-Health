using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class ReturnToLobby : NetworkBehaviour
{

    public Button btnLobby;
    // Start is called before the first frame update
    void Start()
    {
        
            btnLobby.onClick.AddListener(Return);
        

        
    }

    private void Return()
    {
        ReturnToRoom();
    }

    private void ReturnToRoom(string sceneName = "Lobby")
    {
        NetworkManager.SceneManager.LoadScene(sceneName,
        UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
