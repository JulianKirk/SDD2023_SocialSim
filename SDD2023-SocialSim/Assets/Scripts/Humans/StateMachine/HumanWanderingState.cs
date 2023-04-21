using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanWanderingState : HumanBaseState //Both humans and animal wandering states can inherit from this
{
    int radius = 20; // Not an actual radius. It scans more of a square around the player

    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Started Wandering.");
    }

    public override void UpdateState(HumanStateManager master) 
    {

        if (Input.GetKeyDown("return")) 
        {
            int x = (int)master.transform.position.x + Random.Range(-radius, radius);
            int y = (int)master.transform.position.y + Random.Range(-radius, radius);

            bool path = master.GeneratePath(x, y);

            while (!path)
            {
                x = (int)master.transform.position.x + Random.Range(-radius, radius);
                y = (int)master.transform.position.y + Random.Range(-radius, radius);
                path = master.GeneratePath(x, y);
            }
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        Debug.Log("Stopped Wandering.");
    }
}