using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineSpellZone : SpellZone
{
    public bool horizontal = true;
    
    public override List<Vector2Int> GetZonePositions(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        return horizontal
            ? GetPositionLineHorizontal(launcherGridPosition, targetGridPosition)
            : GetPositionLineVertical(launcherGridPosition, targetGridPosition);
    }
    
    private List<Vector2Int> GetPositionLineVertical(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        Vector2Int direction = Utils.GridDirection(launcherGridPosition, targetGridPosition);
        List<Vector2Int> positions = new();
        for (int i = 0; i < size; i++)
        {
            positions.Add(direction * i);
        }
        return positions;
    }

    private List<Vector2Int> GetPositionLineHorizontal(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        Vector2Int direction = Utils.GridDirection(launcherGridPosition, targetGridPosition);
        List<Vector2Int> positions = new() { new Vector2Int(0, 0) };
        for (int i = 1; i < size; i++)
        {
            positions.Add(new Vector2Int(direction.y * i, -direction.x * i));
            positions.Add(new Vector2Int(-direction.y * i, direction.x * i));
        }
        return positions;
    }
}