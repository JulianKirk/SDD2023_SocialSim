using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanBaseState
{
    public virtual void EnterState(HumanStateManager master) { 
        Debug.Log("Entering state.");
    }

    public abstract void UpdateState(HumanStateManager master);

    public virtual void ExitState(HumanStateManager master) 
    {
        Debug.Log("Exiting state.");
    }
}
