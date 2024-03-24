using System;
using UnityEngine;

public class StartState : BaseState<GameStateMachine.GameState>
{
    public static event Action<Team> OnTeamTurnChanged;

    private Entity _selectedEntity;
    private Team _currentTeam;
    private int _turnCount;

    public StartState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState()
    {
        MapManager.Instance.ActiveTilemapSpawns(true);
        BtnNextUI.OnBtnNextClick += OnClickBtnNext;

        _selectedEntity = null;
        _currentTeam = GameManager.Instance.GetEntities()[0].team;
        _turnCount = 0;

        OnTeamTurnChanged?.Invoke(_currentTeam);
    }

    public override void ExitState()
    {
        MapManager.Instance.ActiveTilemapSpawns(false);
        BtnNextUI.OnBtnNextClick -= OnClickBtnNext;
    }

    public override GameStateMachine.GameState GetNextState()
    {
        if (_turnCount >= 2)
        {
            return GameStateMachine.GameState.Battle;
        }
        return StateKey;
    }

    public override void UpdateState()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);
            Entity entity = node?.entity;

            if (_selectedEntity == null)
            {
                if (entity != null && entity.team == _currentTeam)
                {
                    _selectedEntity = entity;
                }
            }
            else
            {
                if (node.spawnTeam == _currentTeam && node.entity == null)
                {
                    Node oldNode = _selectedEntity.node;

                    _selectedEntity.node = node;
                    _selectedEntity.transform.position = node.worldPosition;

                    node.entity = _selectedEntity;
                    oldNode.entity = null;

                    _selectedEntity = null;
                }
            }
        }


    }

    private void OnClickBtnNext()
    {
        _currentTeam = _currentTeam == Team.BLUE ? Team.RED : Team.BLUE;
        _selectedEntity = null;
        _turnCount++;

        OnTeamTurnChanged?.Invoke(_currentTeam);
    }
}