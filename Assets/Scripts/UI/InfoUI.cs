using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtInfo;

    private void OnEnable()
    {
        StartState.OnTeamTurnChanged += OnTeamTurnChanged;
        BattleState.OnPlayerIndexChanged += OnPlayerIndexChanged;
    }

    private void OnDisable()
    {
        StartState.OnTeamTurnChanged -= OnTeamTurnChanged;
        BattleState.OnPlayerIndexChanged -= OnPlayerIndexChanged;
    }

    private void OnTeamTurnChanged(Team team)
    {
        txtInfo.SetText("Team " + (team == Team.BLUE ? "blue" : "red"));
    }

    private void OnPlayerIndexChanged(int playerIndex)
    {
        txtInfo.SetText("Player : " + (playerIndex + 1));
    }
}
