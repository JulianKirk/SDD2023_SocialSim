using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanWanderingState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    float wanderDuration;
    float wanderTimer;

    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Food weight 0: " + master.m_inventory.GetFoodWeight());

        wanderDuration = Random.Range(5f, 15f);
        wanderTimer = wanderDuration;

        master.Wander((int)master.m_vision);
    }

    public override void UpdateState(HumanStateManager master) 
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0) //If it reaches the destination quickly, it will stay there for a bit 
        {
            master.Wander((int)master.m_vision);

            wanderDuration = Random.Range(5f, 15f);
            wanderTimer = wanderDuration;
        }

    }

    public override void ExitState(HumanStateManager master)
    {

    }
}