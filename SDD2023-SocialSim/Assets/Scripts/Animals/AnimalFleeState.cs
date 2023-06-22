using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFleeState : State<AnimalStateManager> //Both humans and animal wandering states can inherit from this
{
    int radius = 25; // Not an actual radius. It scans more of a square around the centre point
    float fleeTimer;

    public override void EnterState(AnimalStateManager master) 
    {
        fleeTimer = 5f;
    }

    public override void UpdateState(AnimalStateManager master) 
    {
        if (!master.threatIsSensed) 
        {
            fleeTimer -= Time.deltaTime;
        } else {
            fleeTimer = 5f;
        }

        if (fleeTimer <= 0) //If it reaches the destination quickly, it will stay there for a bit 
        {
            master.SwitchState(master.wanderingState);
        }
    }

    public override void ExitState(AnimalStateManager master)
    {
        
    }
}