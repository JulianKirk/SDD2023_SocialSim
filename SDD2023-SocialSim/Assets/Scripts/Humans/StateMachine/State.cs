using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public virtual void EnterState(HumanStateManager master) { 
        Debug.Log("Entering new state");
    }

    public abstract void UpdateState(HumanStateManager master);

    public virtual void ExitState(HumanStateManager master, State newState) 
    {
        master.SwitchState(newState);
    }
}
