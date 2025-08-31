using UnityEngine;
using Mirror;
using System;

public class NetworkingManager : NetworkManager
{
    [Header("Custom Fields")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject hudCanvas;
    /// <summary>
    /// An event for host starting for invoking anything that needs to happen after that
    /// </summary>
    public event Action OnStartHostEvent;

    public static NetworkingManager Instance { get; internal set; }

    public override void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another NetworkingManager instance exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn == null) 
        {
            Debug.Log("[NWM] passed null conn, aborting OnServerAddPlayer...");
            return;
        }

        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.Log("[NWM] playerPrefab or spawnPoint not assigned, aborting OnServerAddPlayer...");
            return;
        }
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        OnStartHostEvent?.Invoke();
    }

    public void RespawnPlayer(NetworkConnectionToClient conn)
    {
        if (conn == null)
        {
            Debug.Log("[NWM] passed null conn, aborting RespawnPlayer...");
            return;
        }

        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.Log("[NWM] playerPrefab or spawnPoint not assigned, aborting RespawnPlayer...");
            return;
        }
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.ReplacePlayerForConnection(conn, player, ReplacePlayerOptions.Destroy);
    }
}
