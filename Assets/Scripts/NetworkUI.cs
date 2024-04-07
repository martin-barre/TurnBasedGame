using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button _btnHost;
    [SerializeField] private Button _btnServer;
    [SerializeField] private Button _btnClient;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _btnHost.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        _btnServer.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        _btnClient.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        else
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
