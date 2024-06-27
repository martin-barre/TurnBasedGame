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

    public EntityData data;
    public Race Race => GameManager.Instance.GetRace(data.RaceEnum);
    public Node Node => MapManager.Instance.GetNode(data.Position);

    [SerializeField] private Animator _animator;

    public int CurrentHp
    {
        get => data.Hp;
        set
        {
            if (data.Hp == value) return;
            data.Hp = value;
            OnHpChange?.Invoke(data.Hp);
        }
    }

    public int CurrentPa
    {
        get => data.Pa;
        set
        {
            if (data.Pa == value) return;
            data.Pa = value;
            OnPaChange?.Invoke(data.Pa);
        }
    }

    public int CurrentPm
    {
        get => data.Pm;
        set
        {
            if (data.Pm == value) return;
            data.Pm = value;
            OnPmChange?.Invoke(data.Pm);
        }
    }

    private List<Node> path = new();

    private void Update()
    {
        if (path.Count > 0)
        {
            Node target = path[0];
            var step = 2 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.worldPosition, step);
            if (Vector3.Distance(transform.position, target.worldPosition) < 0.001f)
            {
                path.RemoveAt(0);
            }
        }

        // ANIMATOR
        _animator.SetBool("Idle", path.Count <= 0);
        _animator.SetBool("Walk", path.Count > 0);
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
