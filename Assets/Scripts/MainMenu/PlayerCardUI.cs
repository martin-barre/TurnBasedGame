using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardUI : MonoBehaviour
{
    [SerializeField] private RaceDatabase _raceDatabase;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _lockStatus;
    [SerializeField] private Button _race1;
    [SerializeField] private Button _race2;
    [SerializeField] private Button _race3;
    [SerializeField] private Sprite _defaultImage;

    private TeamSelectDisplay _teamSelectDisplay;

    public void UpdateUI(TeamSelectDisplay teamSelectDisplay, TeamSelectState teamSelectState, bool isOwner)
    {
        _teamSelectDisplay = teamSelectDisplay;
        _playerName.text = $"Player {teamSelectState.ClientId}";
        _lockStatus.text = teamSelectState.Locked ? "Ready" : "Creating team";
        if (isOwner)
        {
            _race1.onClick.RemoveAllListeners();
            _race2.onClick.RemoveAllListeners();
            _race3.onClick.RemoveAllListeners();
            _race1.onClick.AddListener(() => ButtonClicked(0));
            _race2.onClick.AddListener(() => ButtonClicked(1));
            _race3.onClick.AddListener(() => ButtonClicked(2));
        }
        if (teamSelectState.RaceId1 != ERace.NONE)
        {
            _race1.image.sprite = _raceDatabase.GetRace(teamSelectState.RaceId1).Sprite;
        }
        if (teamSelectState.RaceId2 != ERace.NONE)
        {
            _race2.image.sprite = _raceDatabase.GetRace(teamSelectState.RaceId2).Sprite;
        }
        if (teamSelectState.RaceId3 != ERace.NONE)
        {
            _race3.image.sprite = _raceDatabase.GetRace(teamSelectState.RaceId3).Sprite;
        }
    }

    public void DisableUI()
    {
        _teamSelectDisplay = null;
        _playerName.text = "Waiting...";
        _lockStatus.text = " ";
        _race1.onClick.RemoveAllListeners();
        _race2.onClick.RemoveAllListeners();
        _race3.onClick.RemoveAllListeners();
        _race1.image.sprite = _defaultImage;
        _race2.image.sprite = _defaultImage;
        _race3.image.sprite = _defaultImage;
    }

    private void ButtonClicked(int index)
    {
        _teamSelectDisplay.SelectRaceIndex(index);
    }
}