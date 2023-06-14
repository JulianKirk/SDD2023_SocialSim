using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempWorldGen : MonoBehaviour
{   
    public int mapWidth;
    public int mapLength;

    public GameObject marker;
    public GameObject obstacleMarker;

    public int obstacleChance;

    public static bool[,] walkableGrid; //Can maybe move this within awake later - this was here because before it was public // JK it will have to be universal somehow
    // public static PathNode[,] nodeGrid;

    void Awake() 
    {
        // walkableGrid = new bool[mapWidth, mapLength];

        walkableGrid = new bool[mapWidth, mapLength];
        // nodeGrid = new PathNode[mapWidth, mapLength];

        for (int x = 0; x < mapWidth; x++) 
        {
            for (int y = 0; y < mapLength; y++) 
            {
                bool wlkble = UnityEngine.Random.Range(1, 100) >= obstacleChance;
                // nodeGrid[x,y] = new PathNode(x, y, wlkble);

                if (wlkble) 
                {
                    walkableGrid[x, y] = true;
                } else 
                {
                    walkableGrid[x, y] = false; //Is this necessary? default bool value might already be false
                }

                if (!wlkble) 
                {
                    Instantiate(obstacleMarker, new Vector3(x, y, 0f), Quaternion.identity);
                }
            }
        }

        // PathNode startNode = baseGrid[1, 2];
        // PathNode endNode = baseGrid[70, 86];

        // if (endNode.walkable == false) 
        // {
        //     Debug.Log("End Node not walkable");
        // }

        // // Debug.Log(AstarPathfinding.instance.GeneratePath(startNode, endNode, walkableGrid));

        // List<PathNode> newPath = AstarPathfinding.instance.GeneratePath(startNode, endNode, baseGrid);

        // foreach (PathNode node in newPath) 
        // {
        //     Instantiate(marker, new Vector3(node.xPos, node.yPos, 0f), Quaternion.identity);
        // }
    }
}