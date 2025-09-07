using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjectClient;
    [SerializeField] private List<GameObject> gameObjectServer;

    private async void Start()
    {
        string[] tags = CurrentPlayer.ReadOnlyTags();
        NetworkManager networkManager = NetworkManager.Singleton;
        if (tags.Contains("Server"))
        {
            networkManager.StartServer();
            gameObjectServer.ForEach(g => Instantiate(g).GetComponent<NetworkObject>()?.Spawn());
        }
        else if (tags.Contains("Host"))
        {
            networkManager.StartHost();
            gameObjectServer.ForEach(g => Instantiate(g).GetComponent<NetworkObject>()?.Spawn());
            gameObjectClient.ForEach(g => Instantiate(g).GetComponent<NetworkObject>()?.Spawn());
        }
        else if (tags.Contains("Client"))
        {
            await UnityServices.InitializeAsync();
            await SignInAnonymouslyAsync();
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(AuthenticationService.Instance.PlayerId);
            networkManager.StartClient();
            gameObjectClient.ForEach(g => Instantiate(g).GetComponent<NetworkObject>()?.Spawn());
        }
    }
    
    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"[ConnectionManager] Sign in anonymously succesfully.");
            Debug.Log($"PlayerID : {AuthenticationService.Instance.PlayerId}.");
            Debug.Log($"PlayerName : {await AuthenticationService.Instance.GetPlayerNameAsync()}.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
