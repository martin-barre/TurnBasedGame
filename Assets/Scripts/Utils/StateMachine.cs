using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class StateMachine<EState> : NetworkSingleton<StateMachine<EState>> where EState : Enum
{
    public NetworkVariable<EState> StateEnum = new();

    protected Dictionary<EState, BaseState<EState>> States = new();

    private bool started = false;

    private void OnEnable()
    {
        GameManager.OnDataInitialized += StartStateMachine;
    }

    private void OnDisable()
    {
        GameManager.OnDataInitialized -= StartStateMachine;
    }

    private void StartStateMachine()
    {
        StateEnum.OnValueChanged += OnStateEnumChanged;
        States[StateEnum.Value].EnterState();
        started = true;
    }

    private void Update()
    {
        if (!started) return;
        States[StateEnum.Value].UpdateState();
    }

    private void OnStateEnumChanged(EState oldState, EState newState)
    {
        States[oldState].ExitState();
        States[newState].EnterState();
    }
}