using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CloseGame : NetworkBehaviour
{
    public Button btnGameOver;

    public void Start()
    {
        if(IsHost )
        {
            btnGameOver.onClick.AddListener(doExitGame);
        }
        if (IsClient)
        {
            btnGameOver.onClick.AddListener(doExitGame);
        }
        
    }
    void doExitGame()
    {
        Application.Quit();
    }
}
