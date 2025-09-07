using UnityEngine;

public class EntityInfoWindow : WindowUI
{
    [SerializeField] private GameObject panelBuffsContent;
    [SerializeField] private BuffUI buffsUI;

    private EntityViewModel _entityViewModel;

    private void OnDestroy()
    {
        Unbind();
    }

    public void Bind(Entity entity)
    {
        Unbind();
        
        _entityViewModel = ViewModelFactory.Entity.GetOrCreate(entity);
        _entityViewModel.Buffs.OnListChanged += RefreshBuffsUI;
        
        txtTitle.text = entity.Race.Name;
        RefreshBuffsUI();
    }

    private void Unbind()
    {
        if (_entityViewModel == null) return;
        _entityViewModel.Buffs.OnListChanged -= RefreshBuffsUI;
        _entityViewModel = null;
    }

    private void RefreshBuffsUI()
    {
        foreach (Transform child in panelBuffsContent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (ActiveBuff buff in _entityViewModel.Buffs)
        {
            BuffUI buffUI = Instantiate(buffsUI, panelBuffsContent.transform);
            buffUI.SetUI(buff);
        }
    }
}