using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sex
{
    male,
    female
}

public class HumanStateManager : EntityStateManager
{    
    //Basic attributes - scale from 1 to 100
    public float m_intelligence; //Affects decision making
    public float m_strength; //Affects damage and durability

    //Multiply the increase of attributes with age
    private int m_strMultiplier;
    private int m_intMultiplier;

    public Sex m_sex;
    public bool seekingPartner;
    public bool hasPartner;
    private HumanStateManager m_partner; 

    public int NumberOfChildren;

    //public HumanStateManager babyTemplate; //Assigned in editor

    public HumanStateManager Partner 
    {
        get { 
                if (m_partner == null) 
                {
                    hasPartner = false;
                }
                return m_partner; 
            }
        set {
            if (value is HumanStateManager){
                m_partner = value;
                hasPartner = true;
            }
        }
    }

    //protected int m_age - Inherited from EntityStateManager
    //protected float m_speed - Inherited from EntityStateManager

    public GameObject housePrefab;

    public bool homeOwner;
    public GameObject houseObject;
    public Inventory houseInventory;
    //public Vector2 homePosition - inherited from EntityStateManager

    public bool isStarving;

    public LayerMask enemies;

    public Collider2D animalSense;

    private State<HumanStateManager> m_currentState;

    public HumanBuildHouseState buildHouseState = new HumanBuildHouseState();
    public HumanHuntingState huntingState = new HumanHuntingState();
    public HumanWanderingState wanderingState = new HumanWanderingState();
    public HumanGatheringState gatheringState = new HumanGatheringState();
    public HumanRecallState recallState = new HumanRecallState();
    public HumanBreedingState breedingState = new HumanBreedingState();
    public HumanSeekPartnerState seekPartnerState = new HumanSeekPartnerState();
    public HumanDecisiveState decisiveState = new HumanDecisiveState();

    public List<Item> currentResourceTargets = new List<Item>();
    public Dictionary<Item, Vector2Int> lastSeenResourceLocations = new Dictionary<Item, Vector2Int>();

    protected override void Awake() 
    {
        base.Awake(); //Awake function of EntityStateManager

        m_speed = 2; //From EntityStateManager

        m_inventory = new Inventory(120f);

        currentResourceTargets.Add(Item.Wood);
        currentResourceTargets.Add(Item.Stone);
        
        m_vision = 10f;

        m_age = 0;

        m_currentState = gatheringState;

        m_sex = Random.Range(0, 2) == 1 ? Sex.male : Sex.female;
    }

    protected override void Start() 
    {
        base.Start();

        m_inventory.Add(Item.Fruit, 40f);
        m_inventory.Add(Item.Meat, 40f);

        m_currentState.EnterState(this);
    }

    public void OnSpawn(Vector2Int homePos, GameObject homeObject, int iq, int str, int age) // For parents it will input an average with a deviation 
    {
        m_intMultiplier = iq;
        m_strMultiplier = str;

        m_age = age;

        m_sex = Random.Range(0, 2) == 0 ? Sex.male : Sex.female;

        Debug.Log("Sex: " + m_sex);

        m_intelligence = -m_age * (m_age - 100f) * (0.03f) + 0.5f;
        m_strength = -m_age * (m_age - 100f) * (0.03f) + 0.5f;
        m_speed = /*-m_age * (m_age - 100f) * (0.001f);*/ Mathf.Log10(m_age + 1) + 1;


        m_vision = (9 * Mathf.Log10(m_age + 1)) + 1; // Babies start with really bad vision

        homePosition = homePos;
        houseObject = homeObject;

        homeOwner = false;
        hasPartner = false;
        
        NumberOfChildren = 0;

        //Start off exploring around their home
        lastSeenResourceLocations.Add(Item.Wood, homePosition);
        lastSeenResourceLocations.Add(Item.Stone, homePosition);
        lastSeenResourceLocations.Add(Item.Fruit, homePosition);
        lastSeenResourceLocations.Add(Item.Meat, homePosition);
    }

    protected override void OnNewYear()
    {
        m_age++;

        m_intelligence += (float)m_intMultiplier / (5 * m_age);
        m_strength += (float)m_strMultiplier / (5 * m_age);
         m_speed = /*-m_age * (m_age - 100f) * (0.001f);*/ Mathf.Log10(m_age + 1) + 1;

        m_vision += 3.5f / m_age; //Hyperbolic relationship

        if (m_age == 15) 
        {
            WorldManager.HousePositions.Remove(homePosition);

            if (!WorldManager.HousePositions.Contains(homePosition)) 
            {
                Destroy(houseObject);
            }

            homeOwner = false;
            //houseObject = null; - Can't do this because Unity overrides it in a weird way

            //Set a new home position
            int newXPos = Mathf.Clamp(homePosition.x + Random.Range(-20, 20), 0, MapGenerator.walkableGrid.GetLength(0));
            int newYPos = Mathf.Clamp(homePosition.y + Random.Range(-20, 20), 0, MapGenerator.walkableGrid.GetLength(1));

            homePosition = new Vector2Int(newXPos, newXPos);

            while (WorldManager.HousePositions.Contains(homePosition)) //Make sure there isn't a home there already
            {
                newXPos = Mathf.Clamp(homePosition.x + Random.Range(-20, 20), 0, MapGenerator.walkableGrid.GetLength(0));
                newYPos = Mathf.Clamp(homePosition.y + Random.Range(-20, 20), 0, MapGenerator.walkableGrid.GetLength(1));

                homePosition = new Vector2Int(newXPos, newXPos);
            }
        }
    }

    protected override void OnNewMonth()
    {
        if ((m_inventory.GetFoodWeight()) < (-m_age * (m_age - 100f) * (0.006f))) //They starve if they don't have enough food
        {
            Debug.Log("I starved during the " + m_currentState + " state.");

            WorldManager.RESULTS.deathsByCauses["Starvation"] += 1;

            Die();
        } 
        else 
        {
            m_inventory.RemoveFood((-m_age * (m_age - 100f) * (0.006f)));

            if (m_inventory.GetFoodWeight() < 25f) //1 and 2 thirds months worth of food (for a human in their prime) 
            {
                isStarving = true;

                currentResourceTargets.Clear();

                currentResourceTargets.Add(Item.Fruit);
                currentResourceTargets.Add(Item.Meat);

                SwitchState(gatheringState);
            } else 
            {
                currentResourceTargets.Clear();

                currentResourceTargets.Add(Item.Stone);
                currentResourceTargets.Add(Item.Wood);

                isStarving = false;
            }
        }

        if (!isStarving 
            && m_currentState != recallState 
            && m_currentState != buildHouseState 
            && m_currentState != breedingState) 
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

    public override void Die() 
    {
        if (hasPartner) 
        {
            m_partner.hasPartner = false; //Widow the human's partner so they can match with
        }
        else
        {
            Destroy(houseObject);
            WorldManager.HousePositions.Remove(homePosition);
            WorldManager.HumanCollective.Remove(this);
        }

        WorldManager.HumanCollective.Remove(this);
        WorldManager.RESULTS.totalDeaths += 1;
        WorldManager.RESULTS.deathsByYear[WorldManager.YearsPassed] += 1;

        base.Die();
    }

    public void SpawnHouse() //Because instantiate only work in monobehaviours
    {
        houseObject = Instantiate(housePrefab, new Vector3((int)transform.position.x, (int)transform.position.y, 0), Quaternion.identity);

        WorldManager.HousePositions.Add(homePosition);

        WorldManager.RESULTS.totalHousesBuilt += 1;
        WorldManager.RESULTS.housesBuiltByYear[WorldManager.YearsPassed] += 1;
    }

    public void SpawnBabies(int kidNum) 
    {
        int xTarget = Mathf.Clamp((int)transform.position.x, 0, MapGenerator.walkableGrid.GetLength(0) - 1);
        int yTarget = Mathf.Clamp((int)transform.position.y, 0, MapGenerator.walkableGrid.GetLength(1) - 1);

        for (int n = 0; n < kidNum; n++) 
        {
            int newIQ = m_intMultiplier + Random.Range(-20, 20);
            int newSTR = m_strMultiplier + Random.Range(-20, 20);

            WorldManager.SpawnHuman(this, houseObject, xTarget, yTarget, newIQ, newSTR, 0);
        }

        WorldManager.RESULTS.totalBirths += kidNum;
        WorldManager.RESULTS.birthsByYear[WorldManager.YearsPassed] += kidNum;

        NumberOfChildren++;
    }
}
