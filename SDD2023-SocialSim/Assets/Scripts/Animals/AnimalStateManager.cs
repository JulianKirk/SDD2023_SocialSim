using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateManager : EntityStateManager
{
    public float m_damage; //Indicates the amount of damage this animal does - not used the same way as human "strength"

    private State<AnimalStateManager> m_currentState;
    public State<AnimalStateManager> wanderingState = new AnimalWanderingState();

    public bool threatIsSensed;

    public void OnSpawn(float dmg, float spd) 
    {
        m_damage = dmg;
        m_speed = spd;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        m_vision = 15f;

        m_currentState = wanderingState;

        m_currentState.EnterState(this);
    }

    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void SwitchState(State<AnimalStateManager> newState) 
    {
        m_currentState.ExitState(this);
        m_currentState = newState;

        //Stop all current pathfinding behaviour
        currentTarget = null;
        currentPath = null;
        currentPathIndex = 0;

        m_currentState.EnterState(this);
    }

    protected override void OnNewYear()
    {
        
    }

    protected override void OnNewMonth()
    {
        
    }

    public void BeingHunted() 
    {
        //Switch to a "flee" state
    }
}
