using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanRecallState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    public override void EnterState(HumanStateManager master) 
    {
        Debug.Log("Going Home.");

        master.GeneratePath(master.homePosition.x, master.homePosition.y);
    }

    public override void UpdateState(HumanStateManager master) 
    {
        if (Mathf.Abs(master.homePosition.x - master.transform.position.x) <= 1.15f 
            && Mathf.Abs(master.homePosition.y - master.transform.position.y) <= 1.15f) 
        {
            if(master.homeOwner) 
            {
                float woodTransfer = master.m_inventory.RemoveAll(Item.Wood);

                float stoneTransfer = master.m_inventory.RemoveAll(Item.Wood);

                master.houseInventory.DumpMaterial(woodTransfer, stoneTransfer);

                float foodSurplus = master.m_inventory.m_currentWeight - (60f); //Half of the weight
                float foodDeficit = (30f)  - master.m_inventory.m_currentWeight; //A quarter of the weight

                if (foodSurplus > 0) 
                {
                    float meatRatio = master.m_inventory.RemoveFood(foodSurplus);

                    master.houseInventory.AddFood(foodSurplus, meatRatio);

                } 
                else if (foodDeficit > 0)
                {
                    float meatRatio = master.houseInventory.RemoveFood(foodDeficit);

                    master.m_inventory.AddFood(foodSurplus, meatRatio);
                }

                master.SwitchState(master.decisiveState); //Go wander again
            } 
            else
            {
                if ((master.m_inventory.GetWeight(Item.Stone) + master.m_inventory.GetWeight(Item.Wood))> 40f) 
                {
                    master.SwitchState(master.buildHouseState);
                } else 
                {
                    master.SwitchState(master.decisiveState); //Gather materials to build a home
                }
            }
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        //Exit the state
    }
}