using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "ScriptableObjects/Buff")]
public class Buff : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public string Description;
    public int TurnDuration;
    
    [SerializeReference] public List<Effect> Effects;
}
