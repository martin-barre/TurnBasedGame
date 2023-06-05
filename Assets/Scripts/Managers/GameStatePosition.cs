using UnityEngine;

public class GameStatePosition {

    private Entity selectedEntity;
    private Team currentTeam;
    private int turnCount;

    public void Start() {
        MapManager.Instance.ActiveTilemapSpawns(true);
        currentTeam = GameManager.Instance.GetEntities()[0].team;
        UIManager.Instance.SetInfoMessage("Team " + (currentTeam == Team.BLUE ? "blue" : "red"));
    }

    public void Update() {
        if(Input.GetButtonDown("Fire1")) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);
            if(node == null) return;

            Entity entity = node.entity;

            if(selectedEntity == null) {
                if(entity != null && entity.team == currentTeam) {
                    selectedEntity = entity;
                }
            } else {
                if(node.spawnTeam == currentTeam && node.entity == null) {
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

    public void OnClickBtnNext() {
        NextTeam();
    }

    public void OnClickBtnSpell(int spellIndex) {}

    private void NextTeam() {
        if(currentTeam == Team.BLUE) currentTeam = Team.RED;
        else currentTeam = Team.BLUE;

        UIManager.Instance.SetInfoMessage("Team " + (currentTeam == Team.BLUE ? "blue" : "red"));
        
        selectedEntity = null;
        turnCount++;
        if(turnCount >= 2) GameManager.Instance.SetGameState(GameState.BATTLE);
    }

}
