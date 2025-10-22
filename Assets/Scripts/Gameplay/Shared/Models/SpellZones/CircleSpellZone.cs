using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CircleSpellZone : SpellZone
{
    public bool fill = true;

    public override List<Vector2Int> GetZonePositions(Vector2Int launcherGridPosition, Vector2Int targetGridPosition)
    {
        return fill ? GetPositionCircleFill() : GetPositionCircleLine();
    }

    private List<Vector2Int> GetPositionCircleFill()
    {
        List<Vector2Int> positions = new();
        for (int x = -size + 1; x < size; x++)
        {
            for (int y = -size + 1; y < size; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) < size)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }

    private List<Vector2Int> GetPositionCircleLine()
    {
        List<Vector2Int> positions = new();
        for (int x = -size + 1; x < size; x++)
        {
            for (int y = -size + 1; y < size; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) == size - 1)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }
}