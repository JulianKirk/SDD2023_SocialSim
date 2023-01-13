using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : State
{
    public override void OnStateUpdate(HumanStateManager master) 
    {
        Debug.Log("Buildling upd");

        if (Input.GetButtonDown("Jump")) 
        {
            OnStateExit(master, master.huntingState);
        }
    }
}
