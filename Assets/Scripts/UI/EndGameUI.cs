using System.Linq;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subtitle;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        endGamePanel.SetActive(state == GameState.END);

        if (state == GameState.END)
        {
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
        }
    }
}
