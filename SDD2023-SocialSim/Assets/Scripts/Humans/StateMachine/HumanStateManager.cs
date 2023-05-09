using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateManager : EntityStateManager
{    
    //Basic attributes - scale from 1 to 100
    private float m_intelligence;
    private float m_strength;

    private int m_strMultiplier;
    private int m_intMultiplier;

    private float m_spdMultiplier; //Speed is not from 1 to 100 but instead m/s

    //protected int m_age - Inherited from EntityStateManager
    //protected float m_speed - Inherited from EntityStateManager
    private float m_vision = 10f;

    public LayerMask enemies;

    public Collider2D animalSense;

    private State<HumanStateManager> m_currentState;
    public HumanBuildingState buildingState = new HumanBuildingState();
    public HumanHuntingState huntingState = new HumanHuntingState();
    public HumanWanderingState wanderingState = new HumanWanderingState();

    protected override void Awake() 
    {
        base.Awake(); //Awake function of EntityStateManager
        
        m_currentState = wanderingState;
    }

    void Start() 
    {
        m_currentState.EnterState(this);
    }

    public void OnSpawn(int iq, int str, float spd, int age) //For parents it will input an average with a deviation 
    {
        m_intMultiplier = iq;
        m_strMultiplier = str;
        m_spdMultiplier= spd;

        m_age = age;

        m_intelligence = (0.25f + age) * iq;
        m_strength = (0.25f + age) * str;
        m_speed = (0.05f + age) * spd; //E.g. at 50 speed multiplier the baby starts with 1.25 speed


        m_vision = 5f; //Babies start with bad vision
    }

    protected override void OnNewYear()
    {
        m_intelligence += (float)m_intMultiplier / 10; //MAYBE CHANGE THIS TO A HYPERBOLIC RELATIONSHIP LIKE VISION
        m_strength += (float)m_strMultiplier / 10;
        m_speed += (float)m_spdMultiplier / 10;

        m_vision += 3.5f / m_age; //Hyperbolic relationship

    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.UpdateState(this);

        FollowPath(); //Only runs if a path is defined

        animalSense = Physics2D.OverlapCircle(transform.position, m_vision, enemies);

        if (animalSense != null && m_currentState != huntingState) 
        {
            SwitchState(huntingState);
        }
    }

    public override void SwitchState(State<HumanStateManager> newState) 
    {
        m_currentState.ExitState(this);
        m_currentState = newState;

        //Stop all current pathfinding behaviour
        currentTarget = null;
        currentPath = null;
        currentPathIndex = 0;

        m_currentState.EnterState(this);
    }
}
