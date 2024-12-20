using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStateManager : MonoBehaviour
{
    public float m_health;
    protected int m_age; //Stored in years
    protected float m_speed; //In (m/s) - Also implemented in human and animal state machines

    public Rigidbody2D rBody; // Set in start or awake

    public PathNode currentTarget; //Public so that state scripts can see if the entity is currently chasing something
    protected List<PathNode> currentPath;
    protected int currentPathIndex;

    public float m_vision;

    public Vector2Int homePosition;

    public Inventory m_inventory;

    protected virtual void Awake() 
    {
        rBody = gameObject.GetComponent<Rigidbody2D>();

        homePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y); //Set the initial home position to be where the entity first spawns
    }

    protected virtual void Start()
    {
        WorldManager.OnNewYear += OnNewYear;
        WorldManager.OnNewMonth += OnNewMonth;
    }
    
    public virtual void SwitchState(State<AnimalStateManager> newState) {}
    public virtual void SwitchState(State<HumanStateManager> newState) {}

    public bool GeneratePath(int targetX, int targetY, bool isResource = false) //Needs to be called by state classes, bool indicates if it failed or not
    {
        if (targetX < 0 || targetX >= MapGenerator.walkableGrid.GetLength(0) || targetY < 0 || targetY >= MapGenerator.walkableGrid.GetLength(1)) 
        {
            return false;
        }

        rBody.velocity = new Vector2(0f, 0f);

        PathNode[,] baseGrid = new PathNode[MapGenerator.walkableGrid.GetLength(0), MapGenerator.walkableGrid.GetLength(1)];

        for (int x = 0; x < MapGenerator.walkableGrid.GetLength(0); x++)
        {
            for (int y = 0; y < MapGenerator.walkableGrid.GetLength(1); y++)
            {
                baseGrid[x, y] = new PathNode(x, y, MapGenerator.walkableGrid[x, y]);
            }
        }

        if (isResource)
        {
            baseGrid[targetX, targetY] = new PathNode(targetX, targetY, true);
        }

        // PathNode[,] baseGrid = (PathNode[,])TempWorldGen.nodeGrid.Clone(); -- FOR SOME REASON THIS IS RIDICULOUSLY PERFORMANCE INTENSIVE SO IT JUST CRASHES

        int xTarget = Mathf.Clamp((int)transform.position.x, 0, MapGenerator.walkableGrid.GetLength(0) - 1);
        int yTarget = Mathf.Clamp((int)transform.position.y, 0, MapGenerator.walkableGrid.GetLength(1) - 1);

        PathNode startingNode = baseGrid[xTarget, yTarget];

        currentPathIndex = 0;
        currentPath = AstarPathfinding.instance.GeneratePath(startingNode, baseGrid[targetX, targetY], baseGrid);
        
        if (currentPath == null || currentPath.Count < 1) 
        {
            // Debug.Log("Destination is unreachable");
            return false;
        }
        else 
        {
            try
            {
                currentTarget = currentPath[0];

                rBody.velocity = new Vector2(currentTarget.xPos - transform.position.x, currentTarget.yPos - transform.position.y);
                rBody.velocity.Normalize();
                rBody.velocity = rBody.velocity * m_speed;
            }
            catch
            {

            }
            return true;
        }
    }

    protected void FollowPath() 
    {
        if (currentTarget != null && currentPath.Count != 0) 
        {
            if (Mathf.Abs(currentTarget.xPos - transform.position.x) < 0.25 && Mathf.Abs(currentTarget.yPos - transform.position.y) < 0.25)
            // if (((new Vector2(currentTarget.xPos, currentTarget.yPos)) - (Vector2)transform.position).magnitude < 0.25)
            {
                if(currentPathIndex != currentPath.Count - 1) 
                {
                    currentPathIndex++;
                    currentTarget = currentPath[currentPathIndex];

                    Vector2 newVel = new Vector2(currentTarget.xPos - transform.position.x, currentTarget.yPos - transform.position.y);
                    newVel.Normalize();
                    rBody.velocity = newVel * m_speed;
                } else 
                {
                    ClearPath();
                }
            }
        }
    }

    public void TakeDamage(float damage) 
    {
        m_health -= damage;

        if (m_health <= 0) 
        {
            Die();
        }
    }

    public virtual void Die() //Most animals will drop meat, human deaths will increase a death counter stored in the world manager
    {
        WorldManager.OnNewYear -= OnNewYear;
        WorldManager.OnNewMonth -= OnNewMonth;

        Destroy(gameObject);
    }

    // Destroy(gameObject);

    protected abstract void OnNewYear();

    protected abstract void OnNewMonth();

    public void Wander(int radius) 
    {
        int x = (int)transform.position.x + Random.Range(-radius, radius);
        int y = (int)transform.position.y + Random.Range(-radius, radius);

        bool path = GeneratePath(x, y);

        while (!path)
        {
            x = (int)transform.position.x + Random.Range(-radius, radius);
            y = (int)transform.position.y + Random.Range(-radius, radius);
            path = GeneratePath(x, y);
        }
    }

    public void ClearPath() 
    {
        rBody.velocity = new Vector2(0f, 0f); //reset speed to zero
        currentTarget = null;
        currentPath.Clear();
        currentPathIndex = 0;
    }
}