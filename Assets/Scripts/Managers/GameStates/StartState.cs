using UnityEngine;

public class StartState : BaseState<GameStateMachine.GameState>
{
    private Entity _selectedEntity;
    private bool _teamBlueReady;
    private bool _teamRedReady;

    public StartState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState()
    {
        MapManager.Instance.ActiveTilemapSpawns(true);
        BtnNextUI.OnBtnNextClick += OnBtnNextClick;
        GameCommand.OnTeamReady += OnTeamReady;
        GameCommand.OnSelectSpawn += OnSelectSpawn;

        _selectedEntity = null;
        _teamBlueReady = false;
        _teamRedReady = false;
    }

    public override void ExitState()
    {
        MapManager.Instance.ActiveTilemapSpawns(false);
        BtnNextUI.OnBtnNextClick -= OnBtnNextClick;
        GameCommand.OnTeamReady -= OnTeamReady;
        GameCommand.OnSelectSpawn -= OnSelectSpawn;
    }

    public override GameStateMachine.GameState GetNextState()
    {
        if (_teamBlueReady && _teamRedReady)
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

            if (entity != null && entity.team == GameManager.Instance.CurrentTeam)
            {
                _selectedEntity = entity;
            }
            else
            {
                if (_selectedEntity != null && node.spawnTeam == GameManager.Instance.CurrentTeam && node.entity == null)
                {
                    GameCommand.Instance.SendSelectSpawnEvent(_selectedEntity.node, node);
                }
            }
        }
    }

    private void OnBtnNextClick()
    {
        GameCommand.Instance.SendReadyEvent(GameManager.Instance.CurrentTeam);
    }

    private void OnTeamReady(Team team)
    {
        //if (team == Team.RED)
        {
            _teamRedReady = true;
        }
        //if (team == Team.BLUE)
        {
            _teamBlueReady = true;
        }
    }

    private void OnSelectSpawn(Node node1, Node node2)
    {
        var entity = node1.entity;
        if (entity != null)
        {
            MapManager.Instance.MoveEntity(entity, node2, true);
        }
    }
}