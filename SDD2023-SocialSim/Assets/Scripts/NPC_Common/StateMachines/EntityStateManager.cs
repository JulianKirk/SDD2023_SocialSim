using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStateManager : MonoBehaviour
{
    public float m_health;
    protected int m_age; //Stored in years
    protected float m_speed; //In (m/s) - Also implemented in human and animal state machines

    protected Rigidbody2D rBody; // Set in start or awake

    protected PathNode currentTarget;
    protected List<PathNode> currentPath;
    protected int currentPathIndex;

    protected virtual void Awake() 
    {
        m_speed = 2; //From EntityStateManager

        rBody = gameObject.GetComponent<Rigidbody2D>();

        WorldManager.instance.OnNewYear += OnNewYear;
    }

    public abstract void SwitchState(HumanBaseState newState);

    public bool GeneratePath(int targetX, int targetY) //Needs to be called by state classes, bool indicates if it failed or not
    {
        if (targetX < 0 || targetX >= TempWorldGen.walkableGrid.GetLength(0) || targetY < 0 || targetY >= TempWorldGen.walkableGrid.GetLength(1)) 
        {
            return false;
        }

        rBody.velocity = new Vector2(0f, 0f);

        PathNode[,] baseGrid = new PathNode[TempWorldGen.walkableGrid.GetLength(0), TempWorldGen.walkableGrid.GetLength(1)];

        for (int x = 0; x < TempWorldGen.walkableGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < TempWorldGen.walkableGrid.GetLength(1); y++)
            {
                baseGrid[x, y] = new PathNode(x, y, TempWorldGen.walkableGrid[x, y]);
            }
        }

        // PathNode[,] baseGrid = (PathNode[,])TempWorldGen.nodeGrid.Clone(); -- FOR SOME REASON THIS IS RIDICULOUSLY PERFORMANCE INTENSIVE SO IT JUST CRASHES

        PathNode startingNode = baseGrid[(int)transform.position.x, (int)transform.position.y];

        currentPathIndex = 0;
        currentPath = AstarPathfinding.instance.GeneratePath(startingNode, baseGrid[targetX, targetY], baseGrid);
        
        if (currentPath == null) 
        {
            Debug.Log("Destination is unreachable");
            return false;
        }

        currentTarget = currentPath[0];

        rBody.velocity = new Vector2(currentTarget.xPos - transform.position.x, currentTarget.yPos - transform.position.y);
        rBody.velocity.Normalize();
        rBody.velocity = rBody.velocity * m_speed;

        return true;
    }

    protected void FollowPath() 
    {
        if (currentTarget != null) 
        {
            if (Mathf.Abs(currentTarget.xPos - transform.position.x) < 0.2 && Mathf.Abs(currentTarget.yPos - transform.position.y) < 0.2) 
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
                    rBody.velocity = new Vector2(0f, 0f); //reset speed to zero
                    currentTarget = null;
                    currentPath = null;
                    currentPathIndex = 0;
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
        Destroy(gameObject);
    }

    protected void OnNewYear() 
    {
        m_age++;
    }
}