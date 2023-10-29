using System.Collections.Generic;
using UnityEngine;

public class TimelineUI : MonoBehaviour
{
    [SerializeField] private GameObject timelinePanel;
    [SerializeField] private TimelineEntityUI timelineEntityUI;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.BATTLE)
        {
            timelinePanel.SetActive(false);
        }
        else
        {
            timelinePanel.SetActive(true);

            // CLEAR ALL TIMELINE
            for (int i = 0; i < timelinePanel.transform.childCount; i++)
            {
                Destroy(timelinePanel.transform.GetChild(i).gameObject);
            }

            // CREATE ALL TIMELINE PLAYER
            foreach (var entity in GameManager.Instance.GetEntities())
            {
                var obj = Instantiate(timelineEntityUI, timelinePanel.transform);
                obj.SetEntity(entity);
            }
        }
    }
}
