using System.Linq;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subtitle;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GameStateMachine.OnStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateMachine.GameState state)
    {
        if (state != GameStateMachine.GameState.End) return;

        var winnerTeam = GameManager.Instance.GetEntities().FirstOrDefault(o => !o.IsDead())?.team ?? Team.NONE;

        if (winnerTeam == Team.NONE)
        {
            title.SetText("EGALITÉ");
            subtitle.SetText("C'était un combat épique !");
        }
        else if (winnerTeam == Team.NONE)
        {
            title.SetText("VICTOIRE");
            subtitle.SetText("Les bleus ont gagné");
        }
        else if (winnerTeam == Team.NONE)
        {
            title.SetText("VICTOIRE");
            subtitle.SetText("Les bleus ont gagné");
        }

        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}
