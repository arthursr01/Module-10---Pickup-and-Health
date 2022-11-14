using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public Player playerPrefab;
    public GameObject spawnPoints;
    private List<Player> players = new List<Player>();
    private int spawnIndex = 0;
    private List<Vector3> availableSpawnPositions = new List<Vector3>();
    private bool isGameOver = false;
    private int maxScore = 100;


    public void Start()
    {
        GameData.dbgRun.StartGameWithSceneIfNotChosen();
    }

    //public void Awake()
    //{
    //    RefreshSpawnPoints();
    //}

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            RefreshSpawnPoints();
            SpawnPlayers();
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
        }
    }



    private void RefreshSpawnPoints()
    {
        Transform[] allPoints = spawnPoints.GetComponentsInChildren<Transform>();
        availableSpawnPositions.Clear();
        foreach (Transform point in allPoints)
        {
            if (point != spawnPoints.transform)
            {
                availableSpawnPositions.Add(point.localPosition);
            }
        }
    }

    public Vector3 GetnextSpawnLocation()
    {
        var newPosition = availableSpawnPositions[spawnIndex];
        newPosition.y = 1.5f;
        spawnIndex += 1;
        if (spawnIndex > availableSpawnPositions.Count - 1)
        {
            spawnIndex = 0;
        }

        return newPosition;
    }

    private void SpawnPlayers()
    {
        foreach (PlayerInfo info in GameData.Instance.allPlayers)
        {
            SpawnPlayer(info);
        }
    }

    private void SpawnPlayer(PlayerInfo info)
    {
        Player playerSpawn = Instantiate(playerPrefab, GetnextSpawnLocation(), Quaternion.identity);
        playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
        playerSpawn.PlayerColor.Value = info.color;
        players.Add(playerSpawn);
        playerSpawn.Score.OnValueChanged += HostOnPlayerScoreChanged;
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER");
        Player winner = null;


        foreach (Player player in players)
        {
            player.PauseGame();
            if (player.Score.Value >= maxScore)
            {
                winner = player;
            }
        }

        foreach (Player player in players)
        {
            if (player != winner)
            {
                player.gameObject.transform.LookAt(winner.transform);
            }

        }

        var bullets = GameObject.FindGameObjectsWithTag("Bullet");

        for (var i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
        }
    }

    private void HostOnPlayerScoreChanged(int previos, int current)
    {
        if (current >= maxScore && !isGameOver)
        {
            GameOver();
        }
    }

    private void HostOnClientDisconnect(ulong clientId)
    {
        NetworkObject nObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        Player pObject = nObject.GetComponent<Player>();
        players.Remove(pObject);
    }

    private void HostOnClientConnected(ulong clientId)
    {
        int playerIndex = GameData.Instance.FindPlayerIndex(clientId);
        if (playerIndex != -1)
        {
            PlayerInfo newPlayerInfo = GameData.Instance.allPlayers[playerIndex];
            SpawnPlayer(newPlayerInfo);
        }
    }
 }