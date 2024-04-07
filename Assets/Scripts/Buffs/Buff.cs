using UnityEngine;

public enum Caracteristic
{
    PA,
    PM,
    PO,
    HP,
    DAMAGE,
}

[CreateAssetMenu(fileName = "New Buff", menuName = "ScriptableObjects/Buff")]
public class Buff : ScriptableObject
{
    public Caracteristic Caracteristic;
    public int Value;
    public int NbTurn;
}