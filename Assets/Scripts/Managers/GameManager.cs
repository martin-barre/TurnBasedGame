using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    POSITON,
    BATTLE,
    END
}

public class GameManager : Singleton<GameManager> {

    [SerializeField] private List<Race> blueEntities;
    [SerializeField] private List<Race> redEntities;


    private GameStatePosition gameStatePosition;
    private GameStateBattle gameStateBattle;
    private GameStateEnd gameStateEnd;
    private GameState gameState;
    private List<Entity> entities;
    private Team startTeam;


    private void Start() {
        gameState = GameState.POSITON;
        entities = new List<Entity>();
        gameStatePosition = new GameStatePosition();
        gameStateBattle = new GameStateBattle();
        gameStateEnd = new GameStateEnd();

        float random = Random.Range(0, 1);
        startTeam = random < .5 ? Team.BLUE : Team.RED;

        for(int i = 0; i < blueEntities.Count; i++) {
            Node blueNode = MapManager.Instance.GetRandomSpawns(Team.BLUE);
            Node redNode = MapManager.Instance.GetRandomSpawns(Team.RED);

            if(startTeam == Team.BLUE) {
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
                AddEntity(redEntities[i], Team.RED, redNode);
            } else {
                AddEntity(redEntities[i], Team.RED, redNode);
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
            }
        }

        SetGameState(GameState.POSITON);
    }

    private void Update() {
        UIManager.Instance.SetTimeline(entities);
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Node hoverNode = MapManager.Instance.WorldPositionToMapNodes(mousePosition);
        Entity hoverEntity = hoverNode.entity;
        UIManager.Instance.SetEntityInfo(hoverEntity);

        if(gameState == GameState.POSITON) gameStatePosition.Update();
        else if(gameState == GameState.BATTLE) gameStateBattle.Update();
        else if(gameState == GameState.END) gameStateBattle.Update();
    }

    public void OnClickBtnNext() {
        if(gameState == GameState.POSITON) gameStatePosition.OnClickBtnNext();
        else if(gameState == GameState.BATTLE) gameStateBattle.OnClickBtnNext();
        else if(gameState == GameState.END) gameStateBattle.OnClickBtnNext();
    }

    public void OnClickBtnSpell(int spellIndex) {
        if(gameState == GameState.POSITON) gameStatePosition.OnClickBtnSpell(spellIndex);
        else if(gameState == GameState.BATTLE) gameStateBattle.OnClickBtnSpell(spellIndex);
        else if(gameState == GameState.END) gameStateBattle.OnClickBtnNext();
    }

    public void AddEntity(Race race, Team team, Node spawnNode) {
        GameObject newPlayer = Instantiate(race.prefab, spawnNode.worldPosition, Quaternion.identity);
        Entity entity = newPlayer.GetComponent<Entity>();
        entity.team = team;
        entity.node = spawnNode;
        entity.currentHp = entity.race.hp;
        entity.currentPa = entity.race.pa;
        entity.currentPm = entity.race.pm;
        spawnNode.entity = entity;
        entities.Add(entity);
    }
    
    public void RemoveEntity(Entity entity) {
        entity.node.entity = null;
        Destroy(entity.gameObject);
    }

    // GETTERS
    public List<Entity> GetEntities() {
        return entities;
    }

    // SETTERS
    public void SetGameState(GameState gameState) {
        this.gameState = gameState;
        if(gameState == GameState.POSITON) gameStatePosition.Start();
        else if(gameState == GameState.BATTLE) gameStateBattle.Start();
        else if(gameState == GameState.END) gameStateEnd.Start();
    }

}
