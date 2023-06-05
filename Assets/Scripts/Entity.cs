using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    NONE,
    BLUE,
    RED
}

public class Entity : MonoBehaviour
{

    public Team team;
    public Node node;
    public Race race;
    public int currentHp;
    public int currentPa;
    public int currentPm;

    private List<Node> path = new List<Node>();

    private void Update()
    {
        if (path.Count > 0)
        {
            Node target = path[0];
            var step = 4 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.worldPosition, step);
            if (Vector3.Distance(transform.position, target.worldPosition) < 0.001f)
            {
                path.RemoveAt(0);
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        if (IsDead())
        {
            GameManager.Instance.RemoveEntity(this);
        }
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    public void SetPath(List<Node> path)
    {
        this.path = path;
    }

}
