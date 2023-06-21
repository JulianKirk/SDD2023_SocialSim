using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSeekPartnerState : State<HumanStateManager> //Both humans and animal wandering states can inherit from this
{
    float searchInterval = 5f;
    float timeSinceLastSearch;
    float totalTimeSearching;
    float partnerSearchLimit = 30f;

    public override void EnterState(HumanStateManager master) 
    {
        timeSinceLastSearch = 0f;
        totalTimeSearching = 0f;

        master.seekingPartner = true;

        if (!master.hasPartner) 
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
            else 
            {
                master.Wander((int)master.m_vision);

                timeSinceLastSearch = 0f;
            }
        } else 
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
    }

    public override void UpdateState(HumanStateManager master) 
    {
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
            }

            if (totalTimeSearching > partnerSearchLimit) //Searching for too long - need to go about daily activites
            {
                // master.SwitchState(master.)
            }
        }
        else if (Mathf.Abs(master.Partner.homePosition.x - master.transform.position.x) <= 1.15f 
            && Mathf.Abs(master.Partner.homePosition.y - master.transform.position.y) <= 1.15f) 
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
                    master.Partner = human; //Should set hasPartner to true
                    return true; //No need to search anymore
            }
        }

        return false;
    }
}