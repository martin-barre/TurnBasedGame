using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtInfo;
    private void OnEnable()
    {
        GameStateMachine.OnStateChanged += OnStateChanged;
        BattleState.OnPlayerIndexChanged += OnPlayerIndexChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.OnStateChanged -= OnStateChanged;
        BattleState.OnPlayerIndexChanged -= OnPlayerIndexChanged;
    }

    private void OnStateChanged(GameStateMachine.GameState state)
    {
        if (state == GameStateMachine.GameState.Start)
        {
            txtInfo.SetText("Start phase");
        }
    }

    private void OnPlayerIndexChanged(int playerIndex)
    {
        txtInfo.SetText("Player : " + (playerIndex + 1));
    }
}
