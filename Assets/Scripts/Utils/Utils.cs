using UnityEngine;

public abstract class Utils
{
    /*
        7     8     1
          \       /
            \   /
        6     X     2
            /   \
          /       \
        5     4     3
    */
    public static Vector2Int GridDirection(Vector2Int positionA, Vector2Int positionB)
    {
        if (positionA == positionB) return new Vector2Int(0, 0);

        Vector2Int direction = positionB - positionA;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        Vector2Int realDirection = new Vector2Int(0, 1);
        if (angle == 45) realDirection = new Vector2Int(1, 1);
        else if (angle == 135) realDirection = new Vector2Int(1, -1);
        else if (angle == 225) realDirection = new Vector2Int(-1, -1);
        else if (angle == 315) realDirection = new Vector2Int(-1, 1);

        else if (angle > 45 && angle < 135) realDirection = new Vector2Int(1, 0);
        else if (angle > 135 && angle < 225) realDirection = new Vector2Int(0, -1);
        else if (angle > 225 && angle < 315) realDirection = new Vector2Int(-1, 0);

        return realDirection;
    }
}
