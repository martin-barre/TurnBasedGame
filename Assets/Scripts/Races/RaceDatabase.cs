using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race Database", menuName = "ScriptableObjects/Database/Race")]
public class RaceDatabase : ScriptableObject
{
    [SerializeField] private List<Race> _races;

    public List<Race> Races => _races;

    public Race GetRace(ERace raceEnum)
    {
        return _races.SingleOrDefault(r => r.Enum == raceEnum);
    }
}
