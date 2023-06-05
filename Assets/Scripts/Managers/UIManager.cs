using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [Header("Entity Panel")]
    [SerializeField] private GameObject entityInfo;
    [SerializeField] private Image entityInfoImage;
    [SerializeField] private TMP_Text entityInfoName;
    [SerializeField] private TMP_Text entityInfoHp;
    [SerializeField] private TMP_Text entityInfoPa;
    [SerializeField] private TMP_Text entityInfoPm;

    [Header("Spell Bar")]
    [SerializeField] private TMP_Text txtPa;
    [SerializeField] private TMP_Text txtPm;
    [SerializeField] private List<GameObject> spellButtons;
    [SerializeField] private Sprite emptySpellSprite;

    [Header("Timeline")]
    [SerializeField] private GameObject timeline;
    [SerializeField] private GameObject timelinePlayer;

    [Header("Game End")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text endGameTitle;
    [SerializeField] private TMP_Text endGameSubtitle;

    [Header("Other")]
    [SerializeField] private TMP_Text txtInfo;

    public void SetInfoMessage(string message) {
        txtInfo.SetText(message);
    }

    public void SetEntityInfo(Entity entity) {
        if(entity == null) {
            entityInfo.SetActive(false);
        } else {
            entityInfoImage.sprite = entity.race.sprite;
            entityInfoName.SetText(entity.race.raceName);
            entityInfoHp.SetText("HP : " + entity.currentHp);
            entityInfoPa.SetText("PA : " + entity.currentPa);
            entityInfoPm.SetText("PM : " + entity.currentPm);
            entityInfo.SetActive(true);
        }
    }

    public void SetSpellBar(Entity entity) {
        // REFRESH SPELL BAR
        List<Spell> spells = entity.race.spells;

        for(int i = 0; i < spellButtons.Count; i++) {
            Spell spell = i < spells.Count ? spells[i] : null;
            Sprite sprite = spell != null ? spells[i].iconSprite : emptySpellSprite;
            Button button = spellButtons[i].GetComponent<Button>();
            button.interactable = !( spell == null || entity.currentPa < spell.paCost );

            Image image = spellButtons[i].GetComponent<Image>();
            image.sprite = sprite;
        }

        // REFRESH ALL TEXT
        txtPa.SetText(entity.currentPa.ToString());
        txtPm.SetText(entity.currentPm.ToString());
    }

    public void SetSpellInfo(Button button) {
        Debug.Log(button);
    }

    public void ClearSpellInfo() {
        Debug.Log("oui");
    }

    public void SetTimeline(List<Entity> entities) {
        for(int i = timeline.transform.childCount - 1; i >= 0; i--) {
            GameObject.DestroyImmediate(timeline.transform.GetChild(i).gameObject);
        }
        foreach(Entity entity in entities) {
            GameObject obj = Instantiate(timelinePlayer);
            obj.GetComponent<TimelinePlayer>().SetEntity(entity);
            obj.transform.SetParent(timeline.transform);
        }
    }

    public void SetEndGame(bool active, string title, string subtitle) {
        endGamePanel.SetActive(active);
        endGameTitle.SetText(title);
        endGameSubtitle.SetText(subtitle);
    }

}
