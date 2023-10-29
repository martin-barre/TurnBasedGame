using System;
using UnityEngine;

public class BtnNextUI : MonoBehaviour
{
    public static event Action OnBtnNextClick;

    public void OnClickBtnNext()
    {
        OnBtnNextClick?.Invoke();
    }
}
