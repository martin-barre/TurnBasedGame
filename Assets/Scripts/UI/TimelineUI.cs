using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private GameObject playerListPanel;
    [SerializeField] private TimelineEntityUI timelineEntityUI;
    [SerializeField] private Image currentPlayerImage;

    private CanvasGroup _canvasGroup;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        GameManager.OnEntitiesChanged += UpdateTimeline;
        GameStateMachine.Instance.StateEnum.OnValueChanged += OnGameStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged += OnPlayerIndexChanged;
    }

    private void OnDisable()
    {
        GameManager.OnEntitiesChanged -= UpdateTimeline;
        GameStateMachine.Instance.StateEnum.OnValueChanged -= OnGameStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged -= OnPlayerIndexChanged;
    }

    private void UpdateTimeline()
    {
        // CLEAR ALL TIMELINE
        for (int i = 0; i < playerListPanel.transform.childCount; i++)
        {
            Destroy(playerListPanel.transform.GetChild(i).gameObject);
        }

        // CREATE ALL TIMELINE PLAYER
        foreach (var entity in GameManager.Instance.GetEntities())
        {
            var obj = Instantiate(timelineEntityUI, playerListPanel.transform);
            obj.SetEntity(entity);
        }

        OnPlayerIndexChanged(0, GameManager.Instance.CurrentPlayerIndex.Value);
    }

    private void OnGameStateChanged(GameStateMachine.GameState olsState, GameStateMachine.GameState newState)
    {
        if (newState != GameStateMachine.GameState.Battle)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        else
        {
            OnPlayerIndexChanged(0, GameManager.Instance.CurrentPlayerIndex.Value);
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnPlayerIndexChanged(int oldIndex, int newIndex)
    {
        Vector2 position = playerListPanel.transform.GetChild(newIndex).position;
        currentPlayerImage.rectTransform.DOMove(position, 0.2f).SetEase(Ease.InOutCubic);
    }
}
