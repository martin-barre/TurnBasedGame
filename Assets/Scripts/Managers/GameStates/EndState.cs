public class EndState : BaseState<GameStateMachine.GameState>
{
    public EndState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState() { }

    public override void ExitState() { }

    public override GameStateMachine.GameState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState() { }
}