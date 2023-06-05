using UnityEngine;
using UnityEngine.UI;

public class TimelinePlayer : MonoBehaviour
{
    public Image playerImage;
    public Image colorImage;

    private Entity entity;

    public void SetEntity(Entity entity) {
        this.entity = entity;
        this.playerImage.sprite = entity.race.sprite;

        if(!entity.IsDead()) {
            this.colorImage.color = entity.team == Team.BLUE ? Color.blue : Color.red;
        }
        else {
            playerImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            colorImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }
    }
}
