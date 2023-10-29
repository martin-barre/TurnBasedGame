using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject entityInfo;
    [SerializeField] private Image entityInfoImage;
    [SerializeField] private TMP_Text entityInfoName;
    [SerializeField] private TMP_Text entityInfoHp;
    [SerializeField] private TMP_Text entityInfoPa;
    [SerializeField] private TMP_Text entityInfoPm;

    private void Start()
    {
        OnOveredEntityChanged(null);
    }

    private void OnEnable()
    {
        MapManager.OnOveredEntityChanged += OnOveredEntityChanged;
    }

    private void OnDisable()
    {
        MapManager.OnOveredEntityChanged -= OnOveredEntityChanged;
    }

    private void OnOveredEntityChanged(Entity entity)
    {
        if (entity == null)
        {
            entityInfo.SetActive(false);
        }
        else
        {
            entityInfoImage.sprite = entity.race.sprite;
            entityInfoName.SetText(entity.race.raceName);
            entityInfoHp.SetText("HP : " + entity.CurrentHp);
            entityInfoPa.SetText("PA : " + entity.CurrentPa);
            entityInfoPm.SetText("PM : " + entity.CurrentPm);
            entityInfo.SetActive(true);
        }
    }
}
