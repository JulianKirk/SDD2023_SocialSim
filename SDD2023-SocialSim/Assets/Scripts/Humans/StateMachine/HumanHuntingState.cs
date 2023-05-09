using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHuntingState : State<HumanStateManager>
{
    float huntingTimeLimit = 3f;
    float followInterval= 1f;
    float timeSinceLastFollow;

    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Hunting Started.");

        timeSinceLastFollow = followInterval;

        master.GeneratePath((int)master.animalSense.transform.position.x, (int)master.animalSense.transform.position.y);
    }

    public override void UpdateState(HumanStateManager master) 
    {
        timeSinceLastFollow -= Time.deltaTime;

        if (master.animalSense == null) 
        {
            huntingTimeLimit -= Time.deltaTime;
            master.SwitchState(master.wanderingState);
        } 
        else if (timeSinceLastFollow <= 0) 
        {
            master.GeneratePath((int)master.animalSense.transform.position.x, (int)master.animalSense.transform.position.y);
            timeSinceLastFollow = followInterval;
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        Debug.Log("Hunting Finished.");
    }
}