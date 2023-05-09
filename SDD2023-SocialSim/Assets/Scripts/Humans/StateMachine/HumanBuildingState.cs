using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBuildingState : State<HumanStateManager>
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
