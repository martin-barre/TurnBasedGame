using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpellZoneType {
    CIRCLE_FILL,
    CIRCLE_LINE,
    LINE_VERTICAL,
    LINE_HORIZONTAL,
    ARC,
    CROSS
}

[Serializable]
public class SpellZone {

    public SpellZoneType zoneType;
    public int size = 1;

    public List<Vector2Int> GetPositions(Entity launcher, Node targetNode) {
        switch(zoneType) {
            case SpellZoneType.CIRCLE_FILL:
                return GetPositionCircleFill();
            case SpellZoneType.CIRCLE_LINE:
                return GetPositionCircleLine();
            case SpellZoneType.LINE_VERTICAL:
                return GetPositionLineVertical(launcher, targetNode);
            case SpellZoneType.LINE_HORIZONTAL:
                return GetPositionLineHorizontal(launcher, targetNode);
            case SpellZoneType.ARC:
                return GetPositionArc(launcher, targetNode);
            case SpellZoneType.CROSS:
                return GetPositionCross();
        }
        return new List<Vector2Int>();
    }

    private List<Vector2Int> GetPositionCircleFill() {
        List<Vector2Int> positions = new List<Vector2Int>();

        for(int x = -size + 1; x < size; x ++) {
            for(int y = -size + 1; y < size; y ++) {
                if(Mathf.Abs(x) + Mathf.Abs(y) < size) positions.Add(new Vector2Int(x, y));
            }
        }

        return positions;
    }

    private List<Vector2Int> GetPositionCircleLine() {
        List<Vector2Int> positions = new List<Vector2Int>();
        
        for(int x = -size + 1; x < size; x ++) {
            for(int y = -size + 1; y < size; y ++) {
                if(Mathf.Abs(x) + Mathf.Abs(y) == size - 1) positions.Add(new Vector2Int(x, y));
            }
        }

        return positions;
    }

    private List<Vector2Int> GetPositionLineVertical(Entity launcher, Node targetNode) {
        Vector2Int direction = Utils.GridDirection(launcher.node.gridPosition, targetNode.gridPosition);
        List<Vector2Int> positions = new List<Vector2Int>();

        for(int i = 0; i < size; i++) {
            positions.Add(direction * i);
        }

        return positions;
    }

    private List<Vector2Int> GetPositionLineHorizontal(Entity launcher, Node targetNode) {
        Vector2Int direction = Utils.GridDirection(launcher.node.gridPosition, targetNode.gridPosition);
        List<Vector2Int> positions = new List<Vector2Int>();

        for(int i = -size + 1; i < size; i++) {
            positions.Add(new Vector2Int(-direction.y * i, direction.x * i));
        }

        return positions;
    }

    private List<Vector2Int> GetPositionArc(Entity launcher, Node targetNode) {
        Vector2Int direction = Utils.GridDirection(launcher.node.gridPosition, targetNode.gridPosition);
        List<Vector2Int> positions = new List<Vector2Int>();

        positions.Add(new Vector2Int(0, 0));
        for(int i = 1; i < size; i++) {
            if(Mathf.Abs(direction.x) == Mathf.Abs(direction.y)) {
                positions.Add(new Vector2Int(direction.x - (int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y) * -i));
                positions.Add(new Vector2Int((int)Mathf.Sign(direction.x) * -i, direction.y - (int)Mathf.Sign(direction.y)));
            } else {
                positions.Add(new Vector2Int(direction.x * -i + direction.y * i, direction.y * -i + direction.x * i));
                positions.Add(new Vector2Int(direction.x * -i - direction.y * i, direction.y * -i - direction.x * i));
            }
        }

        return positions;
    }

    private List<Vector2Int> GetPositionCross() {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.Add(new Vector2Int(0, 0));
        for(int i = 1; i < size; i++) {
            positions.Add(new Vector2Int(i, 0));
            positions.Add(new Vector2Int(-i, 0));
            positions.Add(new Vector2Int(0, i));
            positions.Add(new Vector2Int(0, -i));
        }

        return positions;
    }

}
