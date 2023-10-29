using System.Collections.Generic;
using UnityEngine;

public abstract class Dijkstra
{
    private class NodeWeight
    {
        public Node node;
        public int weight;
        public NodeWeight parent;

        public NodeWeight(Node node, int weight, NodeWeight parent)
        {
            this.node = node;
            this.weight = weight;
            this.parent = parent;
        }
    }

    private static readonly Vector2Int[] directions = {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1)
    };

    public static List<Node> GetPath(Entity entity, int pm, Node node)
    {
        List<NodeWeight> openList = new();
        List<Node> closeList = new();

        openList.Add(new NodeWeight(node, 0, null));

        while (openList.Count > 0)
        {
            NodeWeight nodeWeight = openList[0];
            foreach (Vector2Int direction in directions)
            {
                Node nextNode = MapManager.Instance.GetNode(nodeWeight.node.gridPosition + direction);
                NodeWeight nextNodeWeight = new(nextNode, nodeWeight.weight + 1, nodeWeight);
                if (nextNode != null && nextNode.type == NodeType.GROUND && nextNodeWeight.weight <= pm)
                {
                    if (nextNode == entity.node)
                    {
                        List<Node> path = new();
                        while (nextNodeWeight.parent != null)
                        {
                            nextNodeWeight = nextNodeWeight.parent;
                            path.Add(nextNodeWeight.node);
                        }
                        return path;
                    }
                    if (nextNode.entity == null)
                    {
                        if (!InsideListNodeWeight(openList, nextNodeWeight) && !InsideListNode(closeList, nextNodeWeight.node))
                        {
                            openList.Add(nextNodeWeight);
                        }
                    }
                }
            }
            openList.Remove(nodeWeight);
            closeList.Add(nodeWeight.node);
        }
        return new List<Node>();
    }

    public static List<Node> GetDisplacement(Entity entity, int pm)
    {
        List<NodeWeight> openList = new();
        List<Node> closeList = new();

        openList.Add(new NodeWeight(entity.node, 0, null));

        while (openList.Count > 0)
        {
            NodeWeight nodeWeight = openList[0];
            foreach (Vector2Int direction in directions)
            {
                Node nextNode = MapManager.Instance.GetNode(nodeWeight.node.gridPosition + direction);
                if (nextNode != null && nextNode.type == NodeType.GROUND && nextNode.entity == null)
                {
                    NodeWeight nextNodeWeight = new(nextNode, nodeWeight.weight + 1, nodeWeight);
                    if (nextNodeWeight.weight <= pm)
                    {
                        if (!InsideListNodeWeight(openList, nextNodeWeight) && !InsideListNode(closeList, nextNodeWeight.node))
                        {
                            openList.Add(nextNodeWeight);
                        }
                    }
                }
            }
            openList.Remove(nodeWeight);
            closeList.Add(nodeWeight.node);
        }
        closeList.RemoveAt(0);
        return closeList;
    }

    private static bool InsideListNodeWeight(List<NodeWeight> list, NodeWeight nodeWeight)
    {
        foreach (NodeWeight tmpNodeWeight in list)
        {
            if (tmpNodeWeight.node == nodeWeight.node) return true;
            if (tmpNodeWeight.node.gridPosition.x == nodeWeight.node.gridPosition.x && tmpNodeWeight.node.gridPosition.y == nodeWeight.node.gridPosition.y) return true;
        }
        return false;
    }

    private static bool InsideListNode(List<Node> list, Node node)
    {
        foreach (Node tmpNode in list)
        {
            if (tmpNode == node) return true;
            if (tmpNode.gridPosition.x == node.gridPosition.x && tmpNode.gridPosition.y == node.gridPosition.y) return true;
        }
        return false;
    }
}
