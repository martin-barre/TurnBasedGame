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

    private Entity _entity;

    private void Start()
    {
        OnOveredEntityChanged(null);
    }

    private void OnEnable()
    {
        MapManager.OnOveredEntityChanged += OnOveredEntityChanged;

        if (_entity != null)
        {
            _entity.OnHpChange += UpdateHp;
            _entity.OnPaChange += UpdatePa;
            _entity.OnPmChange += UpdatePm;
        }
    }

    private void OnDisable()
    {
        MapManager.OnOveredEntityChanged -= OnOveredEntityChanged;

        if (_entity != null)
        {
            _entity.OnHpChange -= UpdateHp;
            _entity.OnPaChange -= UpdatePa;
            _entity.OnPmChange -= UpdatePm;
        }
    }

    private void OnOveredEntityChanged(Entity entity)
    {
        if (_entity != null)
        {
            _entity.OnHpChange -= UpdateHp;
            _entity.OnPaChange -= UpdatePa;
            _entity.OnPmChange -= UpdatePm;
        }

        _entity = entity;

        if (_entity == null)
        {
            entityInfo.SetActive(false);
        }
        else
        {
            entityInfoImage.sprite = entity.race.sprite;
            entityInfoName.SetText(entity.race.raceName);
            entityInfo.SetActive(true);

            UpdateHp(entity.CurrentHp);
            UpdatePa(entity.CurrentPa);
            UpdatePm(entity.CurrentPm);

            _entity.OnHpChange += UpdateHp;
            _entity.OnPaChange += UpdatePa;
            _entity.OnPmChange += UpdatePm;
        }
    }

    private void UpdateHp(int hp)
    {
        entityInfoHp.SetText("HP : " + hp);
    }

    private void UpdatePa(int pa)
    {
        entityInfoPa.SetText("PA : " + pa);
    }

    private void UpdatePm(int pm)
    {
        entityInfoPm.SetText("PM : " + pm);
    }
}
