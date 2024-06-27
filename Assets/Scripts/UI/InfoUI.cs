using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtInfo;
    private void OnEnable()
    {
        GameStateMachine.Instance.StateEnum.OnValueChanged += OnStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged += OnPlayerIndexChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.Instance.StateEnum.OnValueChanged -= OnStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged -= OnPlayerIndexChanged;
    }

    private void OnStateChanged(GameStateMachine.GameState oldState, GameStateMachine.GameState newState)
    {
        if (newState == GameStateMachine.GameState.Start)
        {
            txtInfo.SetText("Start phase");
        }
    }

    private void OnPlayerIndexChanged(int oldIndex, int newIndex)
    {
        txtInfo.SetText("Player : " + (newIndex + 1));
    }
}
