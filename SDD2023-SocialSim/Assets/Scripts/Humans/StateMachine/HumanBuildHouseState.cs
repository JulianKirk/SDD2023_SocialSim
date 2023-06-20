using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBuildHouseState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    float totalBuildTime;
    float remainingBuildTime;

    public override void EnterState(HumanStateManager master) 
    {
        totalBuildTime = ((WorldManager.secondsPerYr)/12) /  Mathf.Clamp((0.1f * master.m_strength + 0.05f * master.m_intelligence), 1, 20);
        remainingBuildTime = totalBuildTime;

    }

    public override void UpdateState(HumanStateManager master) 
    {
        remainingBuildTime -= Time.deltaTime;

        Debug.Log("Remaining Build Time: " + remainingBuildTime);

        //Play some sort of building animation

        if (remainingBuildTime <= 0) 
        {
            master.SpawnHouse();

            master.SwitchState(master.recallState);
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        //Set new home position
        master.homePosition = Vector2Int.RoundToInt(master.transform.position);

        master.houseInventory = master.houseObject.GetComponent<House>().m_inventory;

        //Set flag to true so the human doesn't attempt to build any more homes
        master.homeOwner = true;
    }
}