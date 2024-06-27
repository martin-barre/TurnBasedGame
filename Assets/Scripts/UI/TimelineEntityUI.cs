using UnityEngine;
using UnityEngine.UI;

public class TimelineEntityUI : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Image colorImage;

    private Entity entity;

    private void OnEnable()
    {
        if (entity != null) {
            entity.OnHpChange += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (entity != null)
        {
            entity.OnHpChange -= UpdateUI;
        }
    }

    public void SetEntity(Entity entity)
    {
        if (this.entity != null)
        {
            this.entity.OnHpChange -= UpdateUI;
        }

        this.entity = entity;
        this.entity.OnHpChange += UpdateUI;

        if (entity.data.ParentId >= 0) transform.localScale = Vector3.one * 0.8f;

        playerImage.sprite = entity.Race.Sprite;
        colorImage.color = entity.data.Team == Team.BLUE ? Color.blue : Color.red;
    }

    private void UpdateUI(int hp)
    {
        var ratio = (float)hp / entity.Race.Hp;
        colorImage.transform.localScale = new Vector3(ratio, 1, 1);

        if (entity.IsDead())
        {
            playerImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            colorImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
    }
}
