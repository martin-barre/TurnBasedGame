using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class WindowUI : MonoBehaviour
{
    [SerializeField] protected Button btnClose;
    [SerializeField] protected TMP_Text txtTitle;

    private void Start()
    {
        btnClose.onClick.AddListener(() => Destroy(gameObject));
    }
}