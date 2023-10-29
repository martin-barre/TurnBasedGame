using System;
using UnityEngine;

public class GameStatePosition
{
    public static event Action<Team> OnTeamTurnChanged;

    private Entity selectedEntity;
    private Team currentTeam;
    private int turnCount;

    public void Start()
    {
        MapManager.Instance.ActiveTilemapSpawns(true);
        currentTeam = GameManager.Instance.GetEntities()[0].team;
        OnTeamTurnChanged?.Invoke(currentTeam);
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);
            if (node == null) return;

            Entity entity = node.entity;

            if (selectedEntity == null)
            {
                if (entity != null && entity.team == currentTeam)
                {
                    selectedEntity = entity;
                }
            }
            else
            {
                if (node.spawnTeam == currentTeam && node.entity == null)
                {
                    Node oldNode = selectedEntity.node;

                    selectedEntity.node = node;
                    selectedEntity.transform.position = node.worldPosition;

                    node.entity = selectedEntity;
                    oldNode.entity = null;

                    selectedEntity = null;
                }
            }
        }
    }

    public void OnClickBtnNext()
    {
        // NEXT TEAM
        currentTeam = currentTeam == Team.BLUE ? Team.RED : Team.BLUE;

        OnTeamTurnChanged?.Invoke(currentTeam);

        selectedEntity = null;
        turnCount++;
        if (turnCount >= 2)
        {
            GameManager.Instance.UpdateGameState(GameState.BATTLE);
        }
    }
}
