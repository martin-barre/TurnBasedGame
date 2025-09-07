using System;
using UnityEngine;
using UnityEngine.UI;

public class TimelineEntityUI : MonoBehaviour
{
    [SerializeField] private Image imgEntity;
    [SerializeField] private Slider sliderHealth;

    private EntityViewModel _entityViewModel;

    private void OnDisable()
    {
        Unbind();
    }

    public void Bind(Entity entity)
    {
        Unbind();
        
        // BIND
        _entityViewModel = ViewModelFactory.Entity.GetOrCreate(entity);
        _entityViewModel.Hp.OnValueChanged += UpdateHp;
        
        // UPDATE UI
        imgEntity.sprite = entity.Race.IconSprite;
        imgEntity.enabled = entity.Race != null;
        sliderHealth.maxValue = entity.Race.Hp;
        UpdateHp(entity.Hp);
    }
    
    private void Unbind()
    {
        if (_entityViewModel == null) return;
        
        _entityViewModel.Hp.OnValueChanged -= UpdateHp;
        _entityViewModel = null;
    }
    
    private void UpdateHp(int hp) => sliderHealth.value = hp;
}
