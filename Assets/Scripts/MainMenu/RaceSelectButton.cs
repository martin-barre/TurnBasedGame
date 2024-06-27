using UnityEngine;
using UnityEngine.UI;

public class RaceSelectButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;

    private TeamSelectDisplay _teamSelectDisplay;
    private Race _race;

    public void SetRace(TeamSelectDisplay teamSelectDisplay, Race race)
    {
        _teamSelectDisplay = teamSelectDisplay;
        _race = race;
        _iconImage.sprite = race.Sprite;
    }

    public void SelectCharacter()
    {
        _teamSelectDisplay.SelectRace(_race);
    }
}