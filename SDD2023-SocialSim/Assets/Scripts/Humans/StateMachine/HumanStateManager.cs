using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanStateManager : EntityStateManager
{    
    //Basic attributes - scale from 1 to 100
    public float m_intelligence; //Affects decision making
    public float m_strength; //Affects damage and durability

    //Multiply the increase of attributes with age
    private int m_strMultiplier;
    private int m_intMultiplier;
    private float m_spdMultiplier; 

    //protected int m_age - Inherited from EntityStateManager
    //protected float m_speed - Inherited from EntityStateManager

    public GameObject housePrefab;

    public bool homeOwner;
    public GameObject houseObject;
    public Inventory houseInventory;

    bool isStarving;

    public LayerMask enemies;

    public Collider2D animalSense;

    private State<HumanStateManager> m_currentState;

    public HumanBuildHouseState buildHouseState = new HumanBuildHouseState();
    public HumanHuntingState huntingState = new HumanHuntingState();
    public HumanWanderingState wanderingState = new HumanWanderingState();
    public HumanGatheringState gatheringState = new HumanGatheringState();
    public HumanRecallState recallState = new HumanRecallState();

    public List<Item> currentResourceTargets = new List<Item>();

    protected override void Awake() 
    {
        base.Awake(); //Awake function of EntityStateManager

        m_speed = 2; //From EntityStateManager

        m_inventory = new Inventory(100f);

        currentResourceTargets.Add(Item.Wood);
        currentResourceTargets.Add(Item.Stone);
        
        m_vision = 10f;

        m_age = 0;

        m_currentState = gatheringState;
    }

    protected override void Start() 
    {
        base.Start();

        m_inventory.Add(Item.Fruit, 25f);
        m_inventory.Add(Item.Meat, 25f);

        m_currentState.EnterState(this);
    }

    public void OnSpawn(Vector2Int homePos, int iq, int str, float spd, int age) //For parents it will input an average with a deviation 
    {
        m_intMultiplier = iq;
        m_strMultiplier = str;
        m_spdMultiplier = spd;

        m_age = age;

        m_intelligence = -m_age * (m_age - 100f) * (0.03f);
        m_strength = -m_age * (m_age - 100f) * (0.03f);
        m_speed = Mathf.Log10(m_age) + 1; //E.g. at 50 speed multiplier the baby starts with 1.25 speed


        m_vision = (9 * Mathf.Log10(m_age)) + 1; //Babies start with really bad vision

        homePosition = homePos;
        homeOwner = false;
    }

    protected override void OnNewYear()
    {
        m_age++;

        m_intelligence += (float)m_intMultiplier / (5 * m_age);
        m_strength += (float)m_strMultiplier / (5 * m_age);
        m_speed += (float)m_spdMultiplier / (10 * m_age);

        m_vision += 3.5f / m_age; //Hyperbolic relationship

    }

    protected override void OnNewMonth()
    {
        if ((m_inventory.GetWeight(Item.Fruit) + m_inventory.GetWeight(Item.Meat)) < (-m_age * (m_age - 100f) * (0.01f))) //They starve if they don't have enough food
        {
            Die();
        } 
        else 
        {
            m_inventory.RemoveFood((-m_age * (m_age - 100f) * (0.01f)));

            if (m_inventory.GetFoodWeight() < 25f) 
            {
                isStarving = true;

                currentResourceTargets.Clear();

                currentResourceTargets.Add(Item.Fruit);
                currentResourceTargets.Add(Item.Meat);

                SwitchState(gatheringState);
            } else 
            {
                isStarving = false;
            }
        }

        if (!isStarving && m_currentState != recallState && m_currentState != buildHouseState) 
        {
            SwitchState(recallState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.UpdateState(this);

        animalSense = Physics2D.OverlapCircle(transform.position, m_vision, enemies);

        Debug.Log("Current state: " + m_currentState);

        // if (animalSense != null && m_currentState != huntingState) 
        // {
        //     animalSense.gameObject.GetComponent<AnimalStateManager>().BeingHunted();
        //     SwitchState(huntingState);
        // }

    }

    void LateUpdate() 
    {
        FollowPath(); //Only runs if a path is defined
    }

    public override void SwitchState(State<HumanStateManager> newState) 
    {
        //Exit old state
        m_currentState.ExitState(this);

        //Stop all current pathfinding behaviour
        ClearPath();

        //Enter new state
        m_currentState = newState;
        m_currentState.EnterState(this);
    }

    public void SpawnHouse() //Because instantiate only work in monobehaviours
    {
        houseObject = Instantiate(housePrefab, new Vector3((int)transform.position.x, (int)transform.position.y, 0), Quaternion.identity);
    }
}
