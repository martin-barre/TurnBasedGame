using System.Collections.Generic;
using UnityEngine;

public enum FovMode {
    NORMAL,
    SQUARE,
    LINE,
    DIAGONAL,
    LINE_DIAGONAL
}

public abstract class FOV
{

    public static List<Node> GetDisplacement(Entity entity, Spell spell) {
        List<Node> nodes = new List<Node>();
        if(spell.poMin == 0) {
            nodes.Add(entity.node);
        }

        if(spell.fovMode == FovMode.NORMAL) {
            if(spell.xRay) nodes.AddRange(DoXRay(entity, spell));
            else nodes.AddRange(DoFOV(entity, spell));
        }
        else if(spell.fovMode == FovMode.SQUARE) {
            if(spell.xRay) nodes.AddRange(DoXRay(entity, spell));
            else nodes.AddRange(DoFOV(entity, spell));
        }
        else if(spell.fovMode == FovMode.LINE) {
            nodes.AddRange(DoLineOnly(entity, spell));
        }
        else if(spell.fovMode == FovMode.DIAGONAL) {
            nodes.AddRange(DoDiagonalOnly(entity, spell));
        }
        else if(spell.fovMode == FovMode.LINE_DIAGONAL) {
            nodes.AddRange(DoLineOnly(entity, spell));
            nodes.AddRange(DoDiagonalOnly(entity, spell));
        }
        

        return nodes;
    }


    private static List<Node> DoFOV(Entity entity, Spell spell) {
        Vector3Int[] directions = {
            new Vector3Int(1, 1, 0),   // UP UP RIGHT
            new Vector3Int(1, 1, 1),   // UP UP LEFT
            new Vector3Int(1, -1, 0),  // RIGHT RIGHT UP
            new Vector3Int(1, -1, 1),  // RIGHT RIGHT DOWN
            new Vector3Int(-1, -1, 1),  // DOWN DOWN RIGHT
            new Vector3Int(-1, -1, 0),  // DOWN DOWN LEFT
            new Vector3Int(-1, 1, 0),  // LEFT LEFT DOWN
            new Vector3Int(-1, 1, 1),  // LEFT LEFT UP
        };

        List<Node> nodes = new List<Node>();
        foreach(Vector3Int direction in directions) {
            nodes.AddRange(DoFOVRecursive(entity, spell, 0, 0.0f, 1.0f, direction));
        }
        return nodes;
    }


    private static List<Node> DoFOVRecursive(Entity entity, Spell spell, int startLigne, float angleMin, float angleMax, Vector3Int direction) {
        List<Node> nodes = new List<Node>();

        for(int x = startLigne; x <= spell.poMax; x++) {
            bool blocked = false;
            for(int y = 0; y <= spell.poMax; y++) {
                if(angleMin > angleMax) return nodes;
                if(x == 0 && y == 0) continue;
                if(spell.fovMode != FovMode.SQUARE && x + y > spell.poMax) continue;

                // RECUPERATION DU NODE
                int realX = direction.z == 0 ? x : y;
                int realY = direction.z == 0 ? y : x;
                realX = entity.node.gridPosition.x + realX * direction.x;
                realY = entity.node.gridPosition.y + realY * direction.y;
                Node node = MapManager.Instance.GetNode(Vector2Int.CeilToInt(new Vector2(realX, realY)));

                float angle = (float)y / x;

                // DESACTIVE LA CASE CAR ELLE N'EST PAS VISIBLE
                bool active = true;
                if(angle < angleMin || angle > angleMax) {
                    active = false;
                }

                if(node == null || node.type == NodeType.WALL || node.entity != null) {
                    if(!blocked) {
                        float newAngleMax = (y - .5f) / (x + .5f);
                        nodes.AddRange(DoFOVRecursive(entity, spell, x + 1, angleMin, newAngleMax, direction));
                        blocked = true;
                    }
                    float newAngleMin = (y + .5f) / (x - .5f);
                    angleMin = Mathf.Max(angleMin, newAngleMin);
                } else {
                    blocked = false;
                }

                if(angle > angleMax) break;

                if(active && node != null && node.type == NodeType.GROUND) {
                    if(spell.fovMode == FovMode.SQUARE && Mathf.Abs(x) < spell.poMin && Mathf.Abs(y) < spell.poMin) continue;
                    if(spell.fovMode != FovMode.SQUARE && x + y < spell.poMin) continue;
                    if(direction.z == 0 && x == y) continue;
                    if(!spell.canLaunchOnEntity && node.entity != null) continue;
                    nodes.Add(node);
                }
            }
        }

        return nodes;
    }


    public static List<Node> DoXRay(Entity entity, Spell spell) {
        List<Node> nodes = new List<Node>();

        for(int x = -spell.poMax; x <= spell.poMax; x++) {
            for(int y = -spell.poMax; y <= spell.poMax; y++) {
                if(spell.fovMode == FovMode.SQUARE && Mathf.Abs(x) < spell.poMin && Mathf.Abs(y) < spell.poMin) continue;
                if(spell.fovMode != FovMode.SQUARE && ( Mathf.Abs(x) + Mathf.Abs(y) < spell.poMin || Mathf.Abs(x) + Mathf.Abs(y) > spell.poMax )) continue;
                int realX = entity.node.gridPosition.x + x;
                int realY = entity.node.gridPosition.y + y;
                Node node = MapManager.Instance.GetNode(Vector2Int.CeilToInt(new Vector2(realX, realY)));
                if(node != null && node.type == NodeType.GROUND) {
                    if(!spell.canLaunchOnEntity && node.entity != null) continue;
                    nodes.Add(node);
                }
            }
        }

        return nodes;
    }


    public static List<Node> DoLineOnly(Entity entity, Spell spell) {
        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        List<Node> nodes = new List<Node>();
        foreach(Vector2Int direction in directions) {
            for(int i = 1; i <= spell.poMax; i++) {
                int realX = entity.node.gridPosition.x + i * direction.x;
                int realY = entity.node.gridPosition.y + i * direction.y;
                Node node = MapManager.Instance.GetNode(Vector2Int.CeilToInt(new Vector2(realX, realY)));

                if(node != null && node.type == NodeType.GROUND && i >= spell.poMin && i <= spell.poMax) {
                    if(!spell.canLaunchOnEntity && node.entity != null) continue;
                    nodes.Add(node);
                }

                if(!spell.xRay && ( node == null || node.type == NodeType.WALL || node.entity != null ) ) {
                    break;
                }
            }
        }
        return nodes;
    }

    public static List<Node> DoDiagonalOnly(Entity entity, Spell spell) {
        Vector2Int[] directions = {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1)
        };

        List<Node> nodes = new List<Node>();
        foreach(Vector2Int direction in directions) {
            for(int i = 1; i <= spell.poMax; i++) {
                int realX = entity.node.gridPosition.x + i * direction.x;
                int realY = entity.node.gridPosition.y + i * direction.y;
                Node node = MapManager.Instance.GetNode(Vector2Int.CeilToInt(new Vector2(realX, realY)));

                if(node != null && node.type == NodeType.GROUND && i >= spell.poMin && i <= spell.poMax) {
                    if(!spell.canLaunchOnEntity && node.entity != null) continue;
                    nodes.Add(node);
                }

                if(!spell.xRay && ( node == null || node.type == NodeType.WALL || node.entity != null ) ) {
                    break;
                }
            }
        }
        return nodes;
    }

}
