using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TooltipPosition
{
    Top,
    Bottom,
    Left,
    Right
}

public class ContextTooltip : MonoBehaviour {}

public class TooltipUI : MonoSingleton<TooltipUI>
{
    private Dictionary<Type, ContextTooltip> _tooltips = new();
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _tooltips = GetComponentsInChildren<ContextTooltip>(true).ToDictionary(o => o.GetType(), o => o);
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    public void Show<T>(Vector2 position, TooltipPosition tooltipPosition, Action<T> setup = null) where T : ContextTooltip
    {
        if(_tooltips.TryGetValue(typeof(T), out ContextTooltip content))
        {
            if (setup != null && content is T typedContent)
            {
                setup.Invoke(typedContent); // setup personnalisé ici
            }
            
            content.gameObject.SetActive(true);
            //content.SetData(tooltipData); // ex: méthode de l’interface pour injecter les infos
            
            SetPivotFromPosition(tooltipPosition);
        
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                position,
                _canvas.worldCamera,
                out pos
            );
            _rectTransform.anchoredPosition = ClampToScreen(pos);
        
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.LogWarning($"Tooltip of type {typeof(T).Name} not found in tooltip registry.");
        }
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void SetPivotFromPosition(TooltipPosition position)
    {
        _rectTransform.pivot = position switch
        {
            TooltipPosition.Top => new Vector2(0.5f, 0f),
            TooltipPosition.Bottom => new Vector2(0.5f, 0f),
            TooltipPosition.Left => new Vector2(1f, 0.5f),
            TooltipPosition.Right => new Vector2(0f, 0.5f),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
    }

    private Vector2 ClampToScreen(Vector2 pos)
    {
        Vector2 size = _rectTransform.sizeDelta;
        Vector2 canvasSize = (_canvas.transform as RectTransform).sizeDelta;

        pos.x = Mathf.Clamp(pos.x, -canvasSize.x / 2 + size.x / 2, canvasSize.x / 2 - size.x / 2);
        pos.y = Mathf.Clamp(pos.y, -canvasSize.y / 2 + size.y / 2, canvasSize.y / 2 - size.y / 2);

        return pos;
    }
}
