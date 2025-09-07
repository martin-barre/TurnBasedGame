using System;

[Serializable]
public abstract class Effect {}

[Serializable]
public class AddStatsEffect : Effect
{
    public Stats Stats;
    public int Value;
}
