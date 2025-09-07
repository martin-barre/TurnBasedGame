using UnityEngine;

public enum NodeType : byte
{
    Empty,
    Ground,
    Wall
}

public class Node
{
    public Vector2Int GridPosition { get; set; }
    public Vector3 WorldPosition { get; set; }
    public NodeType NodeType { get; set; }
}