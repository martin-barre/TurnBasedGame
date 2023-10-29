using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtPa;
    [SerializeField] private TMP_Text txtPm;
    [SerializeField] private List<GameObject> spellButtons;
    [SerializeField] private Sprite emptySpellSprite;

    private Entity _entity;

    private void OnEnable()
    {
        GameStateBattle.OnPlayerChanged += OnPlayerChanged;
        if (_entity != null)
        {
            _entity.OnPaChange += RefreshPa;
            _entity.OnPmChange += RefreshPm;
        }
    }

    private void OnDisable()
    {
        GameStateBattle.OnPlayerChanged -= OnPlayerChanged;
        if (_entity != null)
        {
            _entity.OnPaChange -= RefreshPa;
            _entity.OnPmChange -= RefreshPm;
        }
    }

    private void OnPlayerChanged(Entity entity)
    {
        if (_entity != null)
        {
            _entity.OnPaChange -= RefreshPa;
            _entity.OnPmChange -= RefreshPm;
        }
        _entity = entity;
        _entity.OnPaChange += RefreshPa;
        _entity.OnPmChange += RefreshPm;

        RefreshPa(_entity.CurrentPa);
    }

    private void RefreshPa(int pa)
    {
        txtPa.SetText(pa.ToString());

        // REFRESH SPELL BAR
        var spells = _entity.race.spells;

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
