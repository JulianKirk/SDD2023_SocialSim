using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGatheringState : State<HumanStateManager>
{
    Collider2D resourceSense;
    LayerMask currentTargetedResource;

    float searchInterval = 5f;
    float timeSinceLastSearch;

    float gatherInteval = 1f;
    float timeSinceLastGather;

    bool walkingTowardsResource;

    Inventory resourceInventory; //Stores the inventory of the resouce that the human is taking from

    enum GatheringStates //It would probably be better to use a hierarchical state machine instead
    {
        Searching,
        Gathering
    }

    GatheringStates currentState;

    public override void EnterState(HumanStateManager master) 
    {
        SetResourceTarget(master.currentResourceTarget);

        timeSinceLastGather = 0f;

        timeSinceLastSearch = 0f; //So that it can wander straight away
        currentState = GatheringStates.Searching;        

        resourceSense = Physics2D.OverlapCircle(master.transform.position, master.m_vision, master.tempLayerMask);

        if (resourceSense != null) 
        {
            master.GeneratePath((int)resourceSense.transform.position.x, (int)resourceSense.transform.position.y);
        }
        else
        {
            master.Wander(25);
        }
    }

    public override void UpdateState(HumanStateManager master) 
    {        
        resourceSense = Physics2D.OverlapCircle(master.transform.position, master.m_vision, master.tempLayerMask);

        switch(currentState) 
        {
            case GatheringStates.Searching: 
            {

                Debug.Log("Current targeted resource is: " + (int)currentTargetedResource);

                timeSinceLastSearch -= Time.deltaTime;

                if (resourceSense != null) 
                {
                    if (Mathf.Abs(resourceSense.transform.position.x - master.transform.position.x) < 1.1f 
                        && Mathf.Abs(resourceSense.transform.position.y - master.transform.position.y) < 1.1f) //Check if the human is already at the resource
                    {
                        master.ClearPath(); //Stop walking

                        resourceInventory = resourceSense.GetComponent<Resource>().m_inventory;

                        currentState = GatheringStates.Gathering;

                        walkingTowardsResource = false;

                        timeSinceLastSearch = 0f;

                        break;
                    }

                    //The walking towards resource check is needed because otherwise it spams the path generation (choppy movement and more lag)
                    if (!walkingTowardsResource && !master.GeneratePath((int)resourceSense.transform.position.x, (int)resourceSense.transform.position.y))
                    {
                        resourceSense.enabled = false; //Disables the collider so that it is no longer a target for gatherers - unreachable
                        walkingTowardsResource = true;
                    }
                } 
                else if(timeSinceLastSearch <= 0) 
                {
                    master.Wander(25);

                    Debug.Log("Started a new search");

                    timeSinceLastSearch = searchInterval;
                }

                

                
                break;
            }
            case GatheringStates.Gathering: 
            {

                timeSinceLastGather -= Time.deltaTime;

                // Inventory resourceInventory = resourceSense.GetComponent<Resource>().m_inventory;

                /*
                    Insert gathering animation based on the resource gathering type.
                    E.g. axe cutting, pickaxe mining, grass picking
                */

                if (timeSinceLastGather <= 0) 
                {
                    if (resourceInventory.Remove(master.currentResourceTarget, 10)) 
                    {
                        master.m_inventory.Add(master.currentResourceTarget, 10);
                    } 
                    else if (resourceSense != null) //If it fails then there is no resource left at all or a partial amount, check if it is still there
                    {
                        float weight = resourceInventory.GetWeight(master.currentResourceTarget);

                        resourceInventory.Remove(master.currentResourceTarget, weight);
                        master.m_inventory.Add(master.currentResourceTarget, weight);
                    }

                    timeSinceLastGather = gatherInteval;
                }

                Debug.Log("Resource inventory: " + resourceInventory);

                //The resource inventory might delete later in the frame. Short circuits if it does, checks weight if it doesn't.
                if (resourceInventory == null || resourceInventory.m_currentWeight == 0f) 
                {
                    currentState = GatheringStates.Searching;
                    timeSinceLastGather = 0f;
                }

                break;
            }
            default:
                currentState = GatheringStates.Searching;
            break;
        }
    }

    void SetResourceTarget(Item resourceTarget) 
    {
        switch(resourceTarget) 
        {
            case Item.Wood:
                currentTargetedResource = 8;
                break;
            case Item.Fruit:
                currentTargetedResource = 9;
                break;
            case Item.Stone:
                currentTargetedResource = 10;
                break;
            default:
               currentTargetedResource = 8;
               break; 
        }
    }

    // Collider2D SenseResource(HumanStateManager master) 
    // {
    //     return Physics2D.OverlapCircle(master.transform.position, master.m_vision, currentTargetedResource);
    // }
}
