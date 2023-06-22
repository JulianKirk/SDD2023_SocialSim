using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHuntingState : State<HumanStateManager>
{
    //Set up the timers and their limits
    float huntingTimeLimit = 3f;
    float huntingTimeRemaining;
    float followInterval = 1f;
    float timeSinceLastFollow;

    public override void EnterState(HumanStateManager master) 
    {
        timeSinceLastFollow = followInterval;
        huntingTimeRemaining = huntingTimeLimit;

        master.GeneratePath((int)master.animalSense.transform.position.x, (int)master.animalSense.transform.position.y);
    }

    public override void UpdateState(HumanStateManager master) 
    {
        timeSinceLastFollow -= Time.deltaTime;

        if (master.animalSense == null) 
        {
            huntingTimeRemaining -= Time.deltaTime;
        } 
        else if (timeSinceLastFollow <= 0) //Recalculate the Hunter's path to ensure they are traveling in the right direction
        {
            master.GeneratePath((int)master.animalSense.transform.position.x, (int)master.animalSense.transform.position.y);
            timeSinceLastFollow = followInterval;
        }

        if(huntingTimeRemaining <= 0) //If an animal is getting away from the hunter too much, they give up
        {
            huntingTimeRemaining = huntingTimeLimit;
            master.SwitchState(master.decisiveState);
        }
    }

    public override void ExitState(HumanStateManager master)
    {

    }
}