using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBreedingState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    float totalBreedTime;
    float remainingBreedTime;

    public override void EnterState(HumanStateManager master) 
    {
        totalBreedTime = (WorldManager.secondsPerYr)/12;
        remainingBreedTime = totalBreedTime;
    }

    public override void UpdateState(HumanStateManager master) 
    {
        remainingBreedTime -= Time.deltaTime;

        //Play some sort of building animation

        if (remainingBreedTime <= 0) 
        {
            //Chance to have twins
            int numOfKids = Random.Range(0, 100) < 10 ? 2 : 1;
            master.SpawnBabies(numOfKids);

            master.SwitchState(master.recallState);
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        Debug.Log("Breeding complete");

        if (master.m_sex == Sex.female && Random.Range(0, 100) < 10) 
        {
            Debug.Log("Death from breeding");
            master.Die(); 
        }
    }
}