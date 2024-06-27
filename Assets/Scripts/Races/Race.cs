using System.Collections.Generic;
using UnityEngine;

public enum ERace
{
    NONE,
    ARCHER,
    GOBLIN,
    WARRIOR
}

[CreateAssetMenu(fileName = "New Race", menuName = "ScriptableObjects/Race")]
public class Race : ScriptableObject
{
    [SerializeField] private ERace _enum;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Entity _prefab;
    [SerializeField] private int _hp;
    [SerializeField] private int _pa;
    [SerializeField] private int _pm;
    [SerializeField] private List<Spell> _spells;

    public ERace Enum => _enum;
    public string Name => _name;
    public Sprite Sprite => _sprite;
    public Entity Prefab => _prefab;
    public int Hp => _hp;
    public int Pa => _pa;
    public int Pm => _pm;
    public List<Spell> Spells => _spells;
}
