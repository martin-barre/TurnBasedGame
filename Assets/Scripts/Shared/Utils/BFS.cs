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
        List<Node> reachableTiles = new();
        Queue<(int position, int remainingPm)> queue = new();
        bool[] visited = new bool[map.Width * map.Height];
        int startPositionIndex = startPos.x + startPos.y * map.Width;
        int[] directions = { -1, 1, -map.Width, map.Width };

        queue.Enqueue((startPositionIndex, pm));
        visited[startPositionIndex] = true;
        
        while (queue.Count > 0)
        {
            (int position, int remainingPm) = queue.Dequeue();
            
            if (position != startPositionIndex)
            {
                reachableTiles.Add(map.GetNode(position));
            }

            if (remainingPm <= 0) continue;

            for (int i = 0; i < directions.Length; i++)
            {
                int dir = directions[i];
                
                if((dir == -1 && position % map.Width == 0) // Border left
                   || (dir == 1 && position % map.Width == map.Width - 1)) continue; // // Border right
                
                int newPosition = position + dir;

                if (!map.IsWalkable(newPosition)) continue;
                if (gameState.GetEntityByGridPosition(new Vector2Int(newPosition % map.Width, newPosition / map.Width)) != null) continue;
                if (visited[newPosition]) continue;
                
                queue.Enqueue((newPosition, remainingPm - 1));
                visited[newPosition] = true;
            }
        }
        return reachableTiles;
    }
}
