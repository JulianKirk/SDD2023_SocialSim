using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public virtual void OnStateEnter(HumanStateManager master) { 
        Debug.Log("Entering new state");
    }

    public abstract void OnStateUpdate(HumanStateManager master);

    public virtual void OnStateExit(HumanStateManager master, State newState) 
    {
        master.SwitchState(newState);
    }
}
