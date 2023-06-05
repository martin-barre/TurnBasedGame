using UnityEngine;

public enum NodeType {
    GROUND,
    WALL,
    EMPTY
}

public class Node {

    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public NodeType type;
    public Entity entity;
    public Team spawnTeam;

    public Node(Vector2Int gridPosition, Vector3 worldPosition, NodeType type) {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
        this.type = type;
        this.entity = null;
        this.spawnTeam = Team.NONE;
    }

}
