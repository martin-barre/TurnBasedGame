public class GameStateEnd {

    public void Start() {
        Team winTeam = GetTeamWin();

        if(winTeam == Team.NONE) UIManager.Instance.SetEndGame(true, "EGALITÉ", "C'était un combat épique !");
        else if(winTeam == Team.BLUE) UIManager.Instance.SetEndGame(true, "VICTOIRE", "Les bleus ont gagné");
        else if(winTeam == Team.RED) UIManager.Instance.SetEndGame(true, "VICTOIRE", "Les rouges ont gagné");
    }

    public void Update() {}

    public void OnClickBtnNext() {}

    public void OnClickBtnSpell(int spellIndex) {}

    private Team GetTeamWin() {
        foreach(Entity entity in GameManager.Instance.GetEntities()) {
            if(!entity.IsDead() && entity.team == Team.BLUE) return Team.BLUE;
            if(!entity.IsDead() && entity.team == Team.RED) return Team.RED;
        }

        return Team.NONE;
    }

}
