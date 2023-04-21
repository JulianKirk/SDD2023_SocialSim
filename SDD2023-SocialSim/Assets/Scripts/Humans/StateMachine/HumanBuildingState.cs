using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBuildingState : HumanBaseState
{
    public override void UpdateState(HumanStateManager master) 
    {
        Debug.Log("Buildling upd");

        if (Input.GetKeyDown("j")) 
        {
            master.SwitchState(master.huntingState);
        }
    }
}
