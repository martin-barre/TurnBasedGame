using System;
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
    public event Action<int> OnHpChange;
    public event Action<int> OnPaChange;
    public event Action<int> OnPmChange;

    public Team team;
    public Node node;
    public Race race;

    private int _hp;
    private int _pa;
    private int _pm;

    public int CurrentHp
    {
        get
        {
            return _hp;
        }
        set
        {
            if (_hp == value) return;
            _hp = value;
            OnHpChange?.Invoke(_hp);
        }
    }

    public int CurrentPa
    {
        get
        {
            return _pa;
        }
        set
        {
            if (_pa == value) return;
            _pa = value;
            OnPaChange?.Invoke(_pa);
        }
    }

    public int CurrentPm
    {
        get {
            return _pm;
        }
        set {
            if (_pm == value) return;
            _pm = value;
            OnPmChange?.Invoke(_pm);
        }
    }

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
        CurrentHp = Mathf.Max(0, CurrentHp - damage);

        if (IsDead())
        {
            GameManager.Instance.RemoveEntity(this);
        }
    }

    public bool IsDead()
    {
        return CurrentHp <= 0;
    }

    public void SetPath(List<Node> path)
    {
        this.path = path;
    }

}
