using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHuntingState : HumanBaseState
{
    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Hunting Started.");
    }

    public override void UpdateState(HumanStateManager master) 
    {

        if (Input.GetKeyDown("j"))
        {
            master.SwitchState(master.buildingState);
        }

        if (Input.GetKeyDown("return")) 
        {
            int x = Random.Range(1, 100);
            int y = Random.Range(1, 100);

            bool path = master.GeneratePath(x, y);

            while (!path)
            {
                x = Random.Range(1, 100);
                y = Random.Range(1, 100);
                path = master.GeneratePath(x, y);
            }
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        Debug.Log("Hunting Finished.");
    }
}