using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarUI : MonoBehaviour
{
    public static event Action<int> OnBtnSpellClick;

    [SerializeField] private TMP_Text txtPa;
    [SerializeField] private TMP_Text txtPm;
    [SerializeField] private List<GameObject> spellButtons;
    [SerializeField] private Sprite emptySpellSprite;

    private Entity _entity;

    private void OnEnable()
    {
        GameStateMachine.Instance.StateEnum.OnValueChanged += OnGameStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged += OnPlayerIndexChanged;
        if (_entity != null)
        {
            _entity.OnPaChange += RefreshPa;
            _entity.OnPmChange += RefreshPm;
        }
    }

    private void OnDisable()
    {
        GameStateMachine.Instance.StateEnum.OnValueChanged -= OnGameStateChanged;
        GameManager.Instance.CurrentPlayerIndex.OnValueChanged -= OnPlayerIndexChanged;
        if (_entity != null)
        {
            _entity.OnPaChange -= RefreshPa;
            _entity.OnPmChange -= RefreshPm;
        }
    }

    private void OnGameStateChanged(GameStateMachine.GameState oldState, GameStateMachine.GameState newState)
    {
        if (newState == GameStateMachine.GameState.Battle)
        {
            OnPlayerIndexChanged(0, GameManager.Instance.CurrentPlayerIndex.Value);
        }
    }

    public void OnClickBtnSpell(int spellIndex)
    {
        OnBtnSpellClick?.Invoke(spellIndex);
    }

    private void OnPlayerIndexChanged(int oldIndex, int newIndex)
    {
        if (_entity != null)
        {
            _entity.OnPaChange -= RefreshPa;
            _entity.OnPmChange -= RefreshPm;
        }
        _entity = GameManager.Instance.GetEntities()[newIndex];
        _entity.OnPaChange += RefreshPa;
        _entity.OnPmChange += RefreshPm;

        RefreshPa(_entity.CurrentPa);
    }

    private void RefreshPa(int pa)
    {
        txtPa.SetText(pa.ToString());

        // REFRESH SPELL BAR
        var spells = _entity.Race.Spells;

        for (var i = 0; i < spellButtons.Count; i++)
        {
            var spell = i < spells.Count ? spells[i] : null;
            var sprite = spell ? spells[i].iconSprite : emptySpellSprite;
            var button = spellButtons[i].GetComponent<Button>();
            var image = spellButtons[i].GetComponent<Image>();

            button.interactable = spell && spell.paCost <= _entity.CurrentPa;
            image.sprite = sprite;
        }
    }

    private void RefreshPm(int pm)
    {
        txtPm.SetText(pm.ToString());
    }
}
