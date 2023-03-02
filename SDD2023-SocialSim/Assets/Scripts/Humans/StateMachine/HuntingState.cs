using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingState : State
{
    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Hunting");
    }

    public override void UpdateState(HumanStateManager master) 
    {
        Debug.Log("Update :)");

        if (Input.GetButtonDown("Jump")) 
        {
            ExitState(master, master.buildingState);
        }
    }
}
