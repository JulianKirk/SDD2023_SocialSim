using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanWanderingState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    int radius = 25; // Not an actual radius. It scans more of a square around the player
    float wanderDuration;
    float wanderTimer;

    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Started Wandering.");

        wanderDuration = Random.Range(5f, 15f);
        wanderTimer = wanderDuration;

        master.Wander(radius);
    }

    public override void UpdateState(HumanStateManager master) 
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0) //If it reaches the destination quickly, it will stay there for a bit 
        {
            Debug.Log("Start Wander");
            master.Wander(radius);

            wanderDuration = Random.Range(5f, 15f);
            wanderTimer = wanderDuration;
        }

    }

    public override void ExitState(HumanStateManager master)
    {
        Debug.Log("Stopped Wandering.");
    }
}