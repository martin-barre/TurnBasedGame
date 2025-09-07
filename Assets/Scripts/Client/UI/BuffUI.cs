using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtTurnDuration;

    private ActiveBuffViewModel activeBuffViewModel;
    public void SetUI(ActiveBuff buff)
    {
        imgIcon.sprite = buff.Buff.Icon;
        txtName.text = buff.Buff.Name;

        activeBuffViewModel = ViewModelFactory.ActiveBuff.GetOrCreate(buff);
        activeBuffViewModel.TurnDuration.OnValueChanged += UpdateTurnDuration;
        UpdateTurnDuration(activeBuffViewModel.TurnDuration.Value);
    }
    
    private void UpdateTurnDuration(int turnDuration) => txtTurnDuration.text = turnDuration.ToString();
}
