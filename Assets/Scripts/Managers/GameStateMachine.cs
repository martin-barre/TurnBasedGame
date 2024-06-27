public class GameStateMachine : StateMachine<GameStateMachine.GameState>
{
    public enum GameState
    {
        Start,
        Battle,
        End
    }

    protected override void Awake()
    {
        States.Add(GameState.Start, new StartState(GameState.Start));
        States.Add(GameState.Battle, new BattleState(GameState.Battle));
        States.Add(GameState.End, new EndState(GameState.End));
    }
}