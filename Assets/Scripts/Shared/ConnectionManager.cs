using System;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : NetworkManager
{
    private void Start()
    {
        Singleton.OnServerStarted += OnServerStartedCallback;
        Singleton.OnServerStopped += OnServerStoppedCallback;
        Singleton.OnClientStarted += OnClientStartedCallback;
        Singleton.OnClientStopped += OnClientStoppedCallback;
        Singleton.OnConnectionEvent += OnConnectionEventCallback;
        Singleton.ConnectionApprovalCallback += ApprovalCheckCallback;
    }

    private void OnDestroy()
    {
        if (Singleton == null) return;
        Singleton.OnServerStarted -= OnServerStartedCallback;
        Singleton.OnServerStopped -= OnServerStoppedCallback;
        Singleton.OnClientStarted -= OnClientStartedCallback;
        Singleton.OnClientStopped -= OnClientStoppedCallback;
        Singleton.OnConnectionEvent -= OnConnectionEventCallback;
        Singleton.ConnectionApprovalCallback -= ApprovalCheckCallback;
    }

    private static void OnServerStartedCallback() => Debug.Log("[ConnectionManager] Server started.");

    private static void OnServerStoppedCallback(bool isHost) => Debug.Log("[ConnectionManager] Server stopped.");

    private static void OnClientStartedCallback() => Debug.Log("[ConnectionManager] Client started.");

    private static void OnClientStoppedCallback(bool isHost) => Debug.Log("[ConnectionManager] Client stopped.");
    
    private void OnConnectionEventCallback(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        string message = connectionEventData.EventType switch
        {
            ConnectionEvent.ClientConnected => "Client connected",
            ConnectionEvent.ClientDisconnected => "Client disconnected",
            ConnectionEvent.PeerConnected => "Peer connected",
            ConnectionEvent.PeerDisconnected => "Peer connected",
            _ => throw new ArgumentOutOfRangeException()
        };
        Debug.Log($"[ConnectionManager] {message} with id {connectionEventData.ClientId}.");
    }
    
    private void ApprovalCheckCallback(ConnectionApprovalRequest request, ConnectionApprovalResponse response)
    {
        byte[] payload = request.Payload;
        string playerId = Encoding.UTF8.GetString(payload);
        
        Debug.Log($"[ConnectionApproval] Receive connection request with PlayerID {playerId}.");
        
        if (SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(playerId))
        {
            response.Approved = false;
            response.Reason = "This player is already connected.";
            return;
        }

        SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(request.ClientNetworkId, playerId, new SessionPlayerData
        {
            ClientID = request.ClientNetworkId,
            IsConnected = true
        });
        
        response.Approved = true;
    }
}
