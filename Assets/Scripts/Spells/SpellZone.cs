using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpellZoneType
{
    CIRCLE_FILL,
    CIRCLE_LINE,
    LINE_VERTICAL,
    LINE_HORIZONTAL,
    ARC,
    CROSS
}

[Serializable]
public class SpellZone
{
    public SpellZoneType zoneType;
    public int size = 1;

    public List<Vector2Int> GetPositions(Entity launcher, Node targetNode)
    {
        return zoneType switch
        {
            SpellZoneType.CIRCLE_FILL => GetPositionCircleFill(),
            SpellZoneType.CIRCLE_LINE => GetPositionCircleLine(),
            SpellZoneType.LINE_VERTICAL => GetPositionLineVertical(launcher, targetNode),
            SpellZoneType.LINE_HORIZONTAL => GetPositionLineHorizontal(launcher, targetNode),
            SpellZoneType.ARC => GetPositionArc(launcher, targetNode),
            SpellZoneType.CROSS => GetPositionCross(),
            _ => new()
        };
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

    private List<Vector2Int> GetPositionLineVertical(Entity launcher, Node targetNode)
    {
        Vector2Int direction = Utils.GridDirection(launcher.Node.gridPosition, targetNode.gridPosition);
        List<Vector2Int> positions = new();
        for (int i = 0; i < size; i++)
        {
            positions.Add(direction * i);
        }
        return positions;
    }

    private List<Vector2Int> GetPositionLineHorizontal(Entity launcher, Node targetNode)
    {
        Vector2Int direction = Utils.GridDirection(launcher.Node.gridPosition, targetNode.gridPosition);
        List<Vector2Int> positions = new();
        for (int i = -size + 1; i < size; i++)
        {
            positions.Add(new Vector2Int(-direction.y * i, direction.x * i));
        }
        return positions;
    }

    private List<Vector2Int> GetPositionArc(Entity launcher, Node targetNode)
    {
        Vector2Int direction = Utils.GridDirection(launcher.Node.gridPosition, targetNode.gridPosition);
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

    private List<Vector2Int> GetPositionCross()
    {
        List<Vector2Int> positions = new() { new Vector2Int(0, 0) };
        for (int i = 1; i < size; i++)
        {
            positions.Add(new Vector2Int(i, 0));
            positions.Add(new Vector2Int(-i, 0));
            positions.Add(new Vector2Int(0, i));
            positions.Add(new Vector2Int(0, -i));
        }
        return positions;
    }
}
