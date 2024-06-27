using Unity.Netcode;
using UnityEngine;

public class StartState : BaseState<GameStateMachine.GameState>
{
    private Entity _selectedEntity;

    public StartState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState()
    {
        MapManager.Instance.ActiveTilemapSpawns(true);
        BtnNextUI.OnBtnNextClick += OnBtnNextClick;
        GameCommand.OnTeamReady += OnTeamReady;
        GameCommand.OnSelectSpawn += OnSelectSpawn;

        _selectedEntity = null;

        if (NetworkManager.Singleton.IsServer)
        {
            GameManager.Instance.RedTeamReady.Value = false;
            GameManager.Instance.BlueTeamReady.Value = false;
        }
    }

    public override void ExitState()
    {
        MapManager.Instance.ActiveTilemapSpawns(false);
        BtnNextUI.OnBtnNextClick -= OnBtnNextClick;
        GameCommand.OnTeamReady -= OnTeamReady;
        GameCommand.OnSelectSpawn -= OnSelectSpawn;
    }

    public override void UpdateState()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);
            Entity entity = node?.entity;

            if (entity != null && entity.data.Team == GameManager.Instance.CurrentTeam)
            {
                _selectedEntity = entity;
            }
            else
            {
                if (_selectedEntity != null && node.spawnTeam == GameManager.Instance.CurrentTeam && node.entity == null)
                {
                    GameCommand.Instance.SendSelectSpawnEvent(_selectedEntity.Node, node);
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
        if (NetworkManager.Singleton.IsServer)
        {
            if (team == Team.RED)
            {
                GameManager.Instance.RedTeamReady.Value = true;
            }
            if (team == Team.BLUE)
            {
                GameManager.Instance.BlueTeamReady.Value = true;
            }

            if (GameManager.Instance.RedTeamReady.Value && GameManager.Instance.BlueTeamReady.Value) {
                GameStateMachine.Instance.StateEnum.Value = GameStateMachine.GameState.Battle;
            }
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