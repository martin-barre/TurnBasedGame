using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRace", menuName = "ScriptableObjects/Race")]
public class Race : ScriptableObject {

    public string raceName;
    public Sprite sprite;
    public GameObject prefab;
    public int hp;
    public int pa;
    public int pm;
    public List<Spell> spells;

}