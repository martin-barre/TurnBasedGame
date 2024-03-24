using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    public static event Action<EState> OnStateChanged;

    protected Dictionary<EState, BaseState<EState>> States = new();
    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;

    public void Start()
    {
        CurrentState.EnterState();
    }

    private void Update()
    {
        EState newStateKey = CurrentState.GetNextState();
        if (!IsTransitioningState && newStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else if (!IsTransitioningState)
        {
            TransitionToState(newStateKey);
        }
    }

    private void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
        OnStateChanged?.Invoke(stateKey);
    }
}