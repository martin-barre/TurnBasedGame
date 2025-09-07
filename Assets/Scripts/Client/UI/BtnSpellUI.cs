using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnSpellUI : MonoBehaviour
{
    [SerializeField] private Button btnSpell;
    [SerializeField] private TMP_Text txtCooldown;

    private EntityViewModel _entityViewModel;
    private Spell _spell;

    private void OnDestroy()
    {
        Unbind();
    }

    public void Bind(Entity entity, Spell spell)
    {
        Unbind();
        
        _spell = spell;
        _entityViewModel = ViewModelFactory.Entity.GetOrCreate(entity);
        
        btnSpell.onClick.AddListener(OnButtonClick);
        btnSpell.image.sprite = _spell.iconSprite;
        
        // BIND
        _entityViewModel.Pa.OnValueChanged += OnPaChanged;
        
        // UPDATE UI
        RefreshUI();
    }
    
    private void Unbind()
    {
        if (_entityViewModel == null) return;
        btnSpell.onClick.RemoveAllListeners();
        _entityViewModel.Pa.OnValueChanged -= OnPaChanged;
        _entityViewModel = null;
    }
    
    private void RefreshUI()
    {
        btnSpell.interactable = _entityViewModel.Pa.Value >= _spell.paCost;
        txtCooldown.text = "";
    }

    private void OnButtonClick() => InteractionManager.Instance.DisplaySpellNode(_spell.id);
    private void OnPaChanged(int pa) => RefreshUI();
}
