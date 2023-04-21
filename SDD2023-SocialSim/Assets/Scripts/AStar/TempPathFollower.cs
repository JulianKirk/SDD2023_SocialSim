using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPathFollower : MonoBehaviour
{
    public float speed;
    
    private PathNode currentTarget;
    private List<PathNode> currentPath;
    private int currentPathIndex;

    public GameObject marker;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) 
        {
            GotoPosition(20, 21);
        }

        FollowPath();
    }

    void GotoPosition(int targetX, int targetY) 
    {
        PathNode[,] baseGrid = new PathNode[TempWorldGen.walkableGrid.GetLength(0), TempWorldGen.walkableGrid.GetLength(1)];

        for (int x = 0; x < TempWorldGen.walkableGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < TempWorldGen.walkableGrid.GetLength(1); y++)
            {
                baseGrid[x, y] = new PathNode(x, y, TempWorldGen.walkableGrid[x, y]);
            }
        }

        Debug.Log("following path");
        PathNode startingNode = baseGrid[(int)transform.position.x, (int)transform.position.y];

        currentPathIndex = 0;
        currentPath = AstarPathfinding.instance.GeneratePath(startingNode, baseGrid[targetX, targetY], baseGrid);
        currentTarget = currentPath[0];

        foreach (PathNode node in currentPath) 
        {
            Instantiate(marker, new Vector3(node.xPos, node.yPos, 0f), Quaternion.identity);
        }

        rb.velocity = new Vector2(currentTarget.xPos - transform.position.x, currentTarget.yPos - transform.position.y);
        rb.velocity.Normalize();
        rb.velocity = rb.velocity * speed;
    }

    void FollowPath() 
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
                    rb.velocity = newVel * speed;
                } else 
                {
                    rb.velocity = new Vector2(0f, 0f); //reset speed to zero
                    currentTarget = null;
                    currentPath = null;
                    currentPathIndex = 0;
                }
            }
        }
    }
}
