using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBase : StateMachine
{
    //Basic attributes - scale from 1 to 100
    private int m_intelligence;
    private int m_strength;
    private int m_speed;
    private int m_age; //stored in months
    //public int social; ----- Might be too annoying to implement
    //Possibly add other attributes such as weight or height - Depends on complexity and development speed of the earlier ones

    private float m_monthTimer;
    private float temp_monthLengthInSeconds = 10f; //Grab it from a global settings singleton later or something instead

    public BuildingState buildingState = new BuildingState();
    public HuntingState huntingState = new HuntingState();

    void Awake() 
    {
        m_currentState = buildingState;
        Debug.Log("Awoken");
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
        //Time loop to update age - possibly rework later to be dependent on "world time"
        m_monthTimer += Time.deltaTime;
        if (m_monthTimer > temp_monthLengthInSeconds) 
        {
            m_age += 1;
            m_monthTimer = 0f;
        }

        m_currentState.OnStateUpdate(this);
    }
}
