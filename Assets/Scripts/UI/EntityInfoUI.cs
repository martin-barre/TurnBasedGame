using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfoUI : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    [SerializeField] private Image entityInfoImage;
    [SerializeField] private TMP_Text entityInfoName;
    [SerializeField] private TMP_Text entityInfoHp;
    [SerializeField] private TMP_Text entityInfoPa;
    [SerializeField] private TMP_Text entityInfoPm;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Entity _entity;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
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
        if (_entity != null)
        {
            _entity.OnHpChange -= UpdateHp;
            _entity.OnPaChange -= UpdatePa;
            _entity.OnPmChange -= UpdatePm;
        }

        _entity = entity;

        if (_entity == null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        }
        else
        {
            entityInfoImage.sprite = entity.race.sprite;
            entityInfoName.SetText(entity.race.raceName);

            //var pos = Input.mousePosition / _canvas.scaleFactor;
            //_rectTransform.anchoredPosition = pos;
            _rectTransform.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, entity.node.worldPosition + (Vector3)offset);
            _rectTransform.transform.localScale = Vector3.zero;
            _rectTransform.DOScale(1, 0.2f).SetEase(Ease.OutBack);

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1, 0.2f).SetEase(Ease.InOutSine);

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
