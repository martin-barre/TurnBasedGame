using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SpellZone
{
    public int size = 1;
    
    public abstract List<Vector2Int> GetZonePositions(Vector2Int launcherGridPosition, Vector2Int targetGridPosition);
}
