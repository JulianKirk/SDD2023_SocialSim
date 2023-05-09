using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<TStateManager> where TStateManager : EntityStateManager
{
    public virtual void EnterState(TStateManager master) { 
        Debug.Log("Entering state.");
    }

    public abstract void UpdateState(TStateManager master);

    public virtual void ExitState(TStateManager master) 
    {
        Debug.Log("Exiting state.");
    }
}