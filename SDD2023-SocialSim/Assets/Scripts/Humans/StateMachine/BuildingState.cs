using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : State
{
    public override void UpdateState(HumanStateManager master) 
    {
        Debug.Log("Buildling upd");

        if (Input.GetButtonDown("Jump")) 
        {
            ExitState(master, master.huntingState);
        }
    }
}
