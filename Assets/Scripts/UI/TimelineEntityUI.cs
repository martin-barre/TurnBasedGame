using UnityEngine;
using UnityEngine.UI;

public class TimelineEntityUI : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Image colorImage;

    private Entity entity;

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
        this.entity.OnHpChange += UpdateUI;

        playerImage.sprite = entity.race.sprite;
        colorImage.color = entity.team == Team.BLUE ? Color.blue : Color.red;
    }

    private void UpdateUI(int hp)
    {
        var ratio = (float)hp / (float)entity.race.hp;
        colorImage.transform.localScale = new Vector3(ratio, 1, 1);

        if (entity.IsDead())
        {
            playerImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            colorImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
    }
}
