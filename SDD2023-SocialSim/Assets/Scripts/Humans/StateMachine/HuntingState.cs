using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingState : State
{
    public override void OnStateEnter(StateMachine master) 
    {
        Debug.Log("Hunting");
    }

    public override void OnStateUpdate(StateMachine master) 
    {
        Debug.Log("Update :)");
    }

    public override void OnStateExit(StateMachine master) 
    {

    }
}
