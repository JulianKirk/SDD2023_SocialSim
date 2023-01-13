using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingState : State
{
    public override void OnStateEnter(HumanStateManager master) 
    {
        Debug.Log("Hunting");
    }

    public override void OnStateUpdate(HumanStateManager master) 
    {
        Debug.Log("Update :)");

        if (Input.GetButtonDown("Jump")) 
        {
            OnStateExit(master, master.buildingState);
        }
    }
}
