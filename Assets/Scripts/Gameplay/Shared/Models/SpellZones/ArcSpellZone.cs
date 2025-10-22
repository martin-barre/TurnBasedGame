using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArcSpellZone : SpellZone
{
    public override List<Vector2Int> GetZonePositions(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        return GetPositionArc(launcherGridPosition, targetGridPosition);
    }
    
    private List<Vector2Int> GetPositionArc(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        Vector2Int direction = Utils.GridDirection(launcherGridPosition, targetGridPosition);
        List<Vector2Int> positions = new() { new Vector2Int(0, 0) };
        for (int i = 1; i < size; i++)
        {
            if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
            {
                positions.Add(new Vector2Int(direction.x - (int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y) * -i));
                positions.Add(new Vector2Int((int)Mathf.Sign(direction.x) * -i, direction.y - (int)Mathf.Sign(direction.y)));
            }
            else
            {
                positions.Add(new Vector2Int(direction.x * -i + direction.y * i, direction.y * -i + direction.x * i));
                positions.Add(new Vector2Int(direction.x * -i - direction.y * i, direction.y * -i - direction.x * i));
            }
        }
        return positions;
    }
}