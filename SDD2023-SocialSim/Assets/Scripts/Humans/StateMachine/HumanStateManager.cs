using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateManager : EntityStateManager
{    
    //Basic attributes - scale from 1 to 100
    private int m_intelligence;
    private int m_strength;
    private int m_age; //stored in years
    //protected float m_speed - Inherited from EntityStateManager

    private HumanBaseState m_currentState;// - Inherited from EntityStateManager
    public HumanBuildingState buildingState = new HumanBuildingState();
    public HumanHuntingState huntingState = new HumanHuntingState();
    public HumanWanderingState wanderingState = new HumanWanderingState();

    protected override void Awake() 
    {
        base.Awake(); //Awake function of EntityStateManager

        m_currentState = wanderingState;

        m_currentState.EnterState(this);
    }

    public void OnSpawn(int iq, int str, int spd) //For parents it will input an average with a deviation 
    {
        m_intelligence = iq;
        m_strength = str;
        m_speed = spd;
        m_age = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.UpdateState(this);

        FollowPath(); //Only runs if a path is defined
    }

    public override void SwitchState(HumanBaseState newState) 
    {
        m_currentState.ExitState(this);
        m_currentState = newState;
        m_currentState.EnterState(this);
    }
}
