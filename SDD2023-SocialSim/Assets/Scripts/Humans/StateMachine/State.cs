using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void OnStateEnter(StateMachine master);

    public abstract void OnStateUpdate(StateMachine master);

    public abstract void OnStateExit(StateMachine master);
}
