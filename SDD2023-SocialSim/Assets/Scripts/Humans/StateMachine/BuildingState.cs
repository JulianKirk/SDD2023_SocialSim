using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : State
{
    public override void OnStateEnter(StateMachine master) 
    {

    }

    public override void OnStateUpdate(StateMachine master) 
    {
        if (Input.GetButtonDown("Jump")) 
        {
            master.SwitchState(master.huntingState);
        }
    }

    public override void OnStateExit(StateMachine master) 
    {

    }
}
