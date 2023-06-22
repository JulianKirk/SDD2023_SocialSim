using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSeekPartnerState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    float searchInterval;
    float timeSinceLastSearch;
    float totalTimeSearching;
    float partnerSearchLimit = 30f;

    public override void EnterState(HumanStateManager master) 
    {
        searchInterval = Random.Range(3f, 9f);
        timeSinceLastSearch = 0f;
        totalTimeSearching = 0f;

        master.seekingPartner = true;

        if (!master.hasPartner) 
        {
            if (SeekPartner(master)) 
            {
                if (master.m_sex == Sex.male) //Men will move into their partner's house for this simulation
                {
                    //Walk to their partner's house
                    master.GeneratePath(master.Partner.homePosition.x, master.Partner.homePosition.y);

                    //Transfer all the stuff form their house to their partner's house
                    master.Partner.houseInventory.Add(master.houseInventory);

                    master.LeaveHouse();

                    //Move into their partner's house
                    master.houseObject = master.Partner.houseObject;
                    master.houseInventory = master.Partner.houseInventory;
                    master.homePosition = master.Partner.homePosition;
                } 
                else 
                {
                    master.GeneratePath(master.homePosition.x, master.homePosition.y);
                }
            }
            else 
            {
                master.Wander((int)master.m_vision);

                timeSinceLastSearch = 0f;
            }
        } else 
        {
            if (master.m_sex == Sex.male) //In this simulation, men will go to their partner's house to breed
                {
                    master.GeneratePath(master.Partner.homePosition.x, master.Partner.homePosition.y);
                } 
                else 
                {
                    master.GeneratePath(master.homePosition.x, master.homePosition.y);
                }
        }
    }

    public override void UpdateState(HumanStateManager master) 
    {
        totalTimeSearching += Time.deltaTime;

        if (!master.hasPartner) //Will run at intervals until they stop bothering to seek partners or 
        {
            timeSinceLastSearch += Time.deltaTime;
            totalTimeSearching += Time.deltaTime;

            if(timeSinceLastSearch > searchInterval) 
            {
                if (SeekPartner(master)) 
                {
                    if (master.m_sex == Sex.male) //Men will move into their partner's house 
                    {
                        master.GeneratePath(master.Partner.homePosition.x, master.Partner.homePosition.y);
                    } 
                    else 
                    {
                        master.GeneratePath(master.homePosition.x, master.homePosition.y);
                    }
                }

                timeSinceLastSearch = 0f;
                searchInterval = Random.Range(3f, 9f);
            }

            if (totalTimeSearching > partnerSearchLimit) //Searching for too long - need to go about daily activites
            {
                totalTimeSearching = 0f;
                master.SwitchState(master.decisiveState);
            }
        }
        else if (Mathf.Abs(master.Partner.homePosition.x - master.transform.position.x) <= 1f 
            && Mathf.Abs(master.Partner.homePosition.y - master.transform.position.y) <= 1f) 
        {
            master.SwitchState(master.breedingState);
        }
    }

    public override void ExitState(HumanStateManager master)
    {
        master.seekingPartner = false;
    }

    bool SeekPartner(HumanStateManager master) 
    {
        //Locate Partner
        foreach(HumanStateManager human in WorldManager.HumanCollective) 
        {
            //Check if they're taken and the opposite sex
            if (!human.hasPartner && (human.m_sex != master.m_sex) && human.seekingPartner) 
            {
                    human.Partner = master;
                    master.Partner = human; //The setter here should set hasPartner to true
                    return true; //No need to search anymore
            }
        }

        return false;
    }
}