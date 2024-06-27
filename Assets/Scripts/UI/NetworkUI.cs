using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private Button _btnHost;
    [SerializeField] private Button _btnJoin;
    [SerializeField] private TMP_InputField _inputJoinCode;

    [Header("Game Menu")]
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private TMP_Text _txtJoinCode;

    private CanvasGroup _mainMenuCanvasGroup;
    private CanvasGroup _gameMenuCanvasGroup;

    private void Awake()
    {
        _mainMenuCanvasGroup = _mainMenu.GetComponent<CanvasGroup>();
        _gameMenuCanvasGroup = _gameMenu.GetComponent<CanvasGroup>();
        _btnHost.onClick.AddListener(OnClickHost);
        _btnJoin.onClick.AddListener(OnClickJoin);

        ActiveMainMenu(true);
        ActiveGameMenu(false);
    }

    private async void OnClickHost()
    {
        var joinCode = await StartHostWithRelay();
        _txtJoinCode.text = joinCode;
        ActiveMainMenu(false);
        ActiveGameMenu(true);
    }

    private async void OnClickJoin()
    {
        var joinCode = _inputJoinCode.text;
        if (await StartClientWithRelay(joinCode))
        {
            _txtJoinCode.text = joinCode;
            ActiveMainMenu(false);
            ActiveGameMenu(true);
        }
    }

    private void ActiveMainMenu(bool active)
    {
        _mainMenuCanvasGroup.alpha = active ? 1 : 0;
        _mainMenuCanvasGroup.interactable = active;
        _mainMenuCanvasGroup.blocksRaycasts = active;
    }

    private void ActiveGameMenu(bool active)
    {
        _gameMenuCanvasGroup.alpha = active ? 1 : 0;
        _gameMenuCanvasGroup.interactable = active;
        _gameMenuCanvasGroup.blocksRaycasts = active;
    }

    private async Task<string> StartHostWithRelay()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
