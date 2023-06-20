using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanGatheringState : State<HumanStateManager>
{
    Collider2D resourceSense;

    Collider2D  currentTargetedResourceInstance;
    Item currentTargetedResourceType;

    int currentTargetedResourceMask; //Layer masks are just bitmasks

    float searchInterval = 5f;
    float timeSinceLastSearch;

    float gatherInteval = 1f;
    float timeSinceLastGather;

    // float TempTimer = 25f;

    Inventory resourceInventory; //Stores the inventory of the resouce that the human is taking from

    enum GatheringStates //It would probably be better to use a hierarchical state machine instead 
    {
        Searching,
        Gathering
    }

    GatheringStates currentState;

    public override void EnterState(HumanStateManager master) 
    {
        SetResourceTarget(master.currentResourceTargets);

        timeSinceLastGather = 0f;

        timeSinceLastSearch = 0f; //So that it can wander straight away
        currentState = GatheringStates.Searching;        

        resourceSense = Physics2D.OverlapCircle(master.transform.position, master.m_vision, currentTargetedResourceMask);

        if (resourceSense != null) 
        {
            master.GeneratePath((int)resourceSense.transform.position.x, (int)resourceSense.transform.position.y);

            currentTargetedResourceInstance = resourceSense;
        }
        else
        {
            // currentTargetedResourceInstance = null;
            master.Wander(15);
        }
    }

    public override void UpdateState(HumanStateManager master) 
    {        
        switch(currentState) 
        {
            case GatheringStates.Searching: 
            {

                // Debug.Log("Current targeted resource is: " + (int)currentTargetedResourceMask);

                timeSinceLastSearch -= Time.deltaTime;

                if (currentTargetedResourceInstance == null) //If another NPC gets there first
                {
                    // master.ClearPath();

                    resourceSense = Physics2D.OverlapCircle(master.transform.position, master.m_vision, currentTargetedResourceMask);

                    if (resourceSense != null) 
                    {
                        if(!master.GeneratePath((int)resourceSense.transform.position.x, (int)resourceSense.transform.position.y)) 
                        {
                            resourceSense.enabled = false; //Disables the collider so that it is no longer a target for gatherers - unreachable
                            
                            // master.Wander(15);
                            // timeSinceLastSearch = searchInterval;
                        } 
                        else 
                        {
                            currentTargetedResourceInstance = resourceSense;
                        }
                    }
                    else if(timeSinceLastSearch <= 0) 
                    {
                        master.Wander(15);

                        timeSinceLastSearch = searchInterval;
                    }
                } 
                else if (Mathf.Abs(currentTargetedResourceInstance.transform.position.x - master.transform.position.x) <= 1.15f 
                    && Mathf.Abs(currentTargetedResourceInstance.transform.position.y - master.transform.position.y) <= 1.15f) //Check if the human is already at the resource
                {
                    master.ClearPath(); //Stop walking

                    resourceInventory = currentTargetedResourceInstance.GetComponent<Resource>().m_inventory;

                    currentTargetedResourceType = currentTargetedResourceInstance.GetComponent<Resource>().ResourceType;

                    timeSinceLastSearch = 0f;

                    currentState = GatheringStates.Gathering;

                    break;
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
                    if (resourceInventory.Remove(currentTargetedResourceType, 2)) 
                    {
                        master.m_inventory.Add(currentTargetedResourceType, 2);
                    } 
                    else if (resourceSense != null) //If it fails then there is no resource left at all or a partial amount, check if it is still there
                    {
                        float weight = resourceInventory.GetWeight(currentTargetedResourceType);

                        resourceInventory.Remove(currentTargetedResourceType, weight);
                        master.m_inventory.Add(currentTargetedResourceType, weight);
                    }

                    timeSinceLastGather = gatherInteval;
                }

                //The resource inventory might delete later in the frame. Short circuits if it does, checks weight if it doesn't.
                if (resourceInventory == null || resourceInventory.m_currentWeight == 0f) 
                {
                    // currentTargetedResourceInstance = null;

                    if (master.m_inventory.m_currentWeight >= (master.m_inventory.m_maxWeight - 1f))
                    {
                        master.SwitchState(master.recallState);
                    }

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

    void SetResourceTarget(List<Item> resourceTargets) 
    {
        currentTargetedResourceMask = 0;

        if (resourceTargets.Contains(Item.Wood)) 
        {
            currentTargetedResourceMask = currentTargetedResourceMask | 1 << 8;
        }
        if (resourceTargets.Contains(Item.Fruit)) 
        {
            currentTargetedResourceMask = currentTargetedResourceMask | 1 << 9;
        }
        if (resourceTargets.Contains(Item.Stone)) 
        {
            currentTargetedResourceMask = currentTargetedResourceMask | 1 << 10;
        }
    }

    // Collider2D SenseResource(HumanStateManager master) 
    // {
    //     return Physics2D.OverlapCircle(master.transform.position, master.m_vision, currentTargetedResource);
    // }
}
