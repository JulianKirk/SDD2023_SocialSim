using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateManager : MonoBehaviour
{    
    //Basic attributes - scale from 1 to 100
    private int m_intelligence;
    private int m_strength;
    private int m_speed;
    private int m_age; //stored in years
    //public int social; ----- Might be too annoying to implement
    //Possibly add other attributes such as weight or height - Depends on complexity and development speed of the earlier ones

    protected State m_currentState;
    public BuildingState buildingState = new BuildingState();
    public HuntingState huntingState = new HuntingState();

    void Awake() 
    {
        m_currentState = buildingState;
        Debug.Log("State Machine Awoken");
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
    }

    public void SwitchState(State newState) 
    {
        m_currentState = newState;
        m_currentState.EnterState(this);
    }
}
