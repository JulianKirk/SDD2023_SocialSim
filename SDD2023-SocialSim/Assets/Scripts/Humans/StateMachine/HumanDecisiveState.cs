using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanDecisiveState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    //The purpose of this state is to decide what the daily activities of the Human should be

    public override void EnterState(HumanStateManager master) 
    {
        if (master.m_inventory.m_currentWeight == master.m_inventory.m_maxWeight) 
        {
            if (master.homeOwner && master.m_inventory.GetFoodWeight() > 50f /*&& master.houseInventory.m_currentWeight > (30 * (master.NumberOfChildren + 1))*/)
            //They will try to have new children if they are able to support them
            {
                master.SwitchState(master.seekPartnerState);
            } 
            else 
            {
                master.SwitchState(master.wanderingState); //Spend time exploring for the next month
            }
        }
        else if (master.homeOwner) //Prioritise creating a house
        {
            if (master.m_inventory.GetFoodWeight() > 50f && master.houseInventory.m_currentWeight > (30 * (master.NumberOfChildren + 1)))
                //They will try to have new children if they are able to support them
            {
                master.SwitchState(master.seekPartnerState);
            }
            else
            {
                float foodMaterialDifferential = master.m_inventory.GetMaterialWeight() - master.m_inventory.GetFoodWeight();

                if (foodMaterialDifferential >= 0) 
                {
                    master.currentResourceTargets.Clear();

                    master.currentResourceTargets.Add(Item.Fruit);
                    master.currentResourceTargets.Add(Item.Meat);
                }
                else 
                {
                    master.currentResourceTargets.Clear();

                    master.currentResourceTargets.Add(Item.Stone);
                    master.currentResourceTargets.Add(Item.Wood);
                }

                master.SwitchState(master.gatheringState);
            }
        }
        else
        {
            if (master.m_inventory.GetFoodWeight() > 40) //2.5 months worth of food (for a human in their prime)
            {
                master.currentResourceTargets.Clear();

                master.currentResourceTargets.Add(Item.Stone);
                master.currentResourceTargets.Add(Item.Wood);
            } 
            else 
            {
                master.currentResourceTargets.Clear();

                master.currentResourceTargets.Add(Item.Fruit);
                master.currentResourceTargets.Add(Item.Meat);
            }

            master.SwitchState(master.gatheringState);
        }
    }

    public override void UpdateState(HumanStateManager master) 
    {
        //
    }

    public override void ExitState(HumanStateManager master)
    {
        
    }
}