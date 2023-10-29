using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    POSITON,
    BATTLE,
    END
}

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private List<Race> blueEntities;
    [SerializeField] private List<Race> redEntities;

    private GameStatePosition gameStatePosition;
    private GameStateBattle gameStateBattle;
    private GameState gameState;
    private List<Entity> entities;

    private void OnEnable()
    {
        BtnNextUI.OnBtnNextClick += OnClickBtnNext;
    }

    private void OnDisable()
    {
        BtnNextUI.OnBtnNextClick -= OnClickBtnNext;
    }

    private void Start()
    {
        entities = new List<Entity>();
        gameStatePosition = new GameStatePosition();
        gameStateBattle = new GameStateBattle();

        float random = UnityEngine.Random.Range(0, 1);
        var startTeam = random < .5 ? Team.BLUE : Team.RED;

        for (int i = 0; i < blueEntities.Count; i++)
        {
            Node blueNode = MapManager.Instance.GetRandomSpawns(Team.BLUE);
            Node redNode = MapManager.Instance.GetRandomSpawns(Team.RED);

            if (startTeam == Team.BLUE)
            {
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
                AddEntity(redEntities[i], Team.RED, redNode);
            }
            else
            {
                AddEntity(redEntities[i], Team.RED, redNode);
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
            }
        }

        UpdateGameState(GameState.POSITON);
    }

    private void Update()
    {
        if (gameState == GameState.POSITON) gameStatePosition.Update();
        else if (gameState == GameState.BATTLE) gameStateBattle.Update();
        else if (gameState == GameState.END) gameStateBattle.Update();
    }

    private void OnClickBtnNext()
    {
        if (gameState == GameState.POSITON) gameStatePosition.OnClickBtnNext();
        else if (gameState == GameState.BATTLE) gameStateBattle.OnClickBtnNext();
        else if (gameState == GameState.END) gameStateBattle.OnClickBtnNext();
    }

    public void OnClickBtnSpell(int spellIndex)
    {
        if (gameState == GameState.POSITON) return;
        else if (gameState == GameState.BATTLE) gameStateBattle.OnClickBtnSpell(spellIndex);
        else if (gameState == GameState.END) return;
    }

    public void AddEntity(Race race, Team team, Node spawnNode)
    {
        GameObject newPlayer = Instantiate(race.prefab, spawnNode.worldPosition, Quaternion.identity);
        Entity entity = newPlayer.GetComponent<Entity>();
        entity.team = team;
        entity.node = spawnNode;
        entity.CurrentHp = entity.race.hp;
        entity.CurrentPa = entity.race.pa;
        entity.CurrentPm = entity.race.pm;
        spawnNode.entity = entity;
        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entity.node.entity = null;
        Destroy(entity.gameObject);
    }

    // GETTERS
    public List<Entity> GetEntities()
    {
        return entities;
    }

    public void UpdateGameState(GameState gameState)
    {
        this.gameState = gameState;

        switch (gameState)
        {
            case GameState.POSITON:
                gameStatePosition.Start();
                break;
            case GameState.BATTLE:
                gameStateBattle.Start();
                break;
            case GameState.END:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }

        OnGameStateChanged?.Invoke(gameState);
    }
}
