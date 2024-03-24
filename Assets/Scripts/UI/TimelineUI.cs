using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private GameObject timelinePanel;
    [SerializeField] private TimelineEntityUI timelineEntityUI;
    [SerializeField] private Image currentPlayerImage;

    private CanvasGroup _canvasGroup;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        GameManager.OnEntitiesChanged += UpdateTimeline;
        GameStateMachine.OnStateChanged += OnGameStateChanged;
        BattleState.OnPlayerIndexChanged += OnPlayerIndexChanged;
    }

    private void OnDisable()
    {
        GameManager.OnEntitiesChanged -= UpdateTimeline;
        GameStateMachine.OnStateChanged -= OnGameStateChanged;
        BattleState.OnPlayerIndexChanged -= OnPlayerIndexChanged;
    }

    private void UpdateTimeline()
    {
        // CLEAR ALL TIMELINE
        for (int i = 0; i < timelinePanel.transform.childCount; i++)
        {
            Destroy(timelinePanel.transform.GetChild(i).gameObject);
        }

        // CREATE ALL TIMELINE PLAYER
        foreach (var entity in GameManager.Instance.GetEntities())
        {
            var obj = Instantiate(timelineEntityUI, timelinePanel.transform);
            obj.SetEntity(entity);
        }
    }

    private void OnGameStateChanged(GameStateMachine.GameState state)
    {
        if (state != GameStateMachine.GameState.Battle)
        {
            _canvasGroup.alpha = 0;
        }
        else
        {
            _canvasGroup.alpha = 1;
        }
    }

    private void OnPlayerIndexChanged(int index)
    {
        Vector2 position = timelinePanel.transform.GetChild(index).position;
        currentPlayerImage.rectTransform.DOMove(position, 0.2f).SetEase(Ease.InOutCubic);
    }
}
