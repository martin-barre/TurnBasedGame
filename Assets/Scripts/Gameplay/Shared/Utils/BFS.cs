using System;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    public static List<Node> GetPath(Vector2Int startPos, Vector2Int endPos, GameState gameState, Map map, bool adjacent = false)
    {
        Queue<int> queue = new();
        int[] cameFrom = new int[map.Width * map.Height];
        bool[] visited = new bool[map.Width * map.Height];
        int startPositionIndex = startPos.x + startPos.y * map.Width;
        int endPositionIndex = endPos.x + endPos.y * map.Width;
        int[] directions = { -1, 1, -map.Width, map.Width };
        
        queue.Enqueue(startPositionIndex);
        visited[startPositionIndex] = true;
        
        while (queue.Count > 0)
        {
            int position = queue.Dequeue();

            if (adjacent)
            {
                if (position + 1 == endPositionIndex && position % map.Width != map.Width - 1) endPositionIndex -= 1;
                else if (position - 1 == endPositionIndex && position % map.Width != 0) endPositionIndex += 1;
                else if (position + map.Width == endPositionIndex) endPositionIndex -= map.Width;
                else if (position - map.Width == endPositionIndex) endPositionIndex += map.Width;
            }
            
            if (position == endPositionIndex)
            {
                List<Node> path = new();
                int current = endPositionIndex;

                while (current != startPositionIndex)
                {
                    path.Add(map.GetNode(current));
                    current = cameFrom[current];
                }

                path.Reverse();
                return path;
            }

            for (int i = 0; i < directions.Length; i++)
            {
                int dir = directions[i];
                
                if((dir == -1 && position % map.Width == 0) // Border left
                   || (dir == 1 && position % map.Width == map.Width - 1)) continue; // // Border right
                
                int newPosition = position + dir;

                if (!map.IsWalkable(newPosition)) continue;
                if (gameState.GetEntityByGridPosition(new Vector2Int(newPosition % map.Width, newPosition / map.Width)) != null) continue;
                if (visited[newPosition]) continue;
                
                queue.Enqueue(newPosition);
                visited[newPosition] = true;
                cameFrom[newPosition] = position;
            }
        }
        return null;
    }
    
    public static List<Node> GetDisplacement(Vector2Int startPos, int pm, GameState gameState, Map map)
    {
        int width = map.Width;
        int height = map.Height;
        int minX = Mathf.Max(0, startPos.x - pm);
        int minY = Mathf.Max(0, startPos.y - pm);
        int maxX = Mathf.Min(width - 1, startPos.x + pm);
        int maxY = Mathf.Min(height - 1, startPos.y + pm);
        int regionWidth = maxX - minX + 1;
        int regionHeight = maxY - minY + 1;
        int regionTotal = regionWidth * regionHeight;

        // Pré-allouer la liste avec une capacité estimée
        List<Node> reachableTiles = new(Mathf.Min(regionTotal, pm * pm * 3));

        Span<int> queue = regionTotal <= 4096 ? stackalloc int[regionTotal] : new int[regionTotal];
        int head = 0;
        int tail = 0;

        // Utiliser un seul byte pour visited + distance combinés
        Span<byte> visited = regionTotal <= 4096 ? stackalloc byte[regionTotal] : new byte[regionTotal];

        int startLocalX = startPos.x - minX;
        int startLocalY = startPos.y - minY;
        int startLocalPosition = startLocalX + startLocalY * regionWidth;

        queue[tail++] = startLocalPosition;
        visited[startLocalPosition] = 1; // distance 0 + 1 pour distinguer de non-visité

        // Précalculer les offsets globaux
        int globalOffsetX = minX;
        int globalOffsetY = minY * width;

        while (head < tail)
        {
            int localPosition = queue[head++];
            byte distancePlus1 = visited[localPosition];
            byte distance = (byte)(distancePlus1 - 1);

            if (distance > pm) continue;

            int localX = localPosition % regionWidth;
            int localY = localPosition / regionWidth;

            // Calcul direct de la position globale
            int globalPosition = globalOffsetX + localX + globalOffsetY + localY * width;

            if (localPosition != startLocalPosition)
            {
                reachableTiles.Add(map.GetNode(globalPosition));
            }

            byte newDistancePlus1 = (byte)(distancePlus1 + 1);

            // Gauche
            if (localX > 0)
            {
                int newPos = localPosition - 1;
                Node node = map.GetNode(globalPosition - 1);
                if (visited[newPos] == 0 && node is { NodeType: NodeType.Ground } && gameState.GetEntityByGridPosition(node.GridPosition) == null)
                {
                    visited[newPos] = newDistancePlus1;
                    queue[tail++] = newPos;
                }
            }

            // Droite
            if (localX < regionWidth - 1)
            {
                int newPos = localPosition + 1;
                Node node = map.GetNode(globalPosition + 1);
                if (visited[newPos] == 0 && node is { NodeType: NodeType.Ground } && gameState.GetEntityByGridPosition(node.GridPosition) == null)
                {
                    visited[newPos] = newDistancePlus1;
                    queue[tail++] = newPos;
                }
            }

            // Haut
            if (localY > 0)
            {
                int newPos = localPosition - regionWidth;
                Node node = map.GetNode(globalPosition - width);
                if (visited[newPos] == 0 && node is { NodeType: NodeType.Ground } && gameState.GetEntityByGridPosition(node.GridPosition) == null)
                {
                    visited[newPos] = newDistancePlus1;
                    queue[tail++] = newPos;
                }
            }

            // Bas
            if (localY < regionHeight - 1)
            {
                int newPos = localPosition + regionWidth;
                Node node = map.GetNode(globalPosition + width);
                if (visited[newPos] == 0 && node is { NodeType: NodeType.Ground } && gameState.GetEntityByGridPosition(node.GridPosition) == null)
                {
                    visited[newPos] = newDistancePlus1;
                    queue[tail++] = newPos;
                }
            }
        }
        return reachableTiles;
    }
}
