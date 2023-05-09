using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWanderingState : State<AnimalStateManager> //Both humans and animal wandering states can inherit from this
{
    int radius = 25; // Not an actual radius. It scans more of a square around the player
    float wanderDuration;
    float wanderTimer;

    public override void EnterState(AnimalStateManager master) 
    {
        wanderDuration = Random.Range(5f, 15f);
        wanderTimer = wanderDuration;
    }

    public override void UpdateState(AnimalStateManager master) 
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0) //If it reaches the destination quickly, it will stay there for a bit 
        {
            master.Wander(radius);

            wanderDuration = Random.Range(5f, 15f);
            wanderTimer = wanderDuration;
        }
    }

    public override void ExitState(AnimalStateManager master)
    {
        
    }
}