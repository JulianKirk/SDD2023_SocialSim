using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBreedingState : State<HumanStateManager>
{
    float totalBreedTime;
    float remainingBreedTime;

    public override void EnterState(HumanStateManager master) 
    {
        totalBreedTime = (WorldManager.secondsPerYr)/24; //Takes half a month to produce a baby
        remainingBreedTime = totalBreedTime;
    }

    public override void UpdateState(HumanStateManager master) 
    {
        remainingBreedTime -= Time.deltaTime; //Decrement as time goes on

        if (remainingBreedTime <= 0) 
        {
            //10% Chance to have twins
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

            WorldManager.RESULTS.deathsByCauses["Childbirth"] += 1;

            master.Die();
        }
    }
}