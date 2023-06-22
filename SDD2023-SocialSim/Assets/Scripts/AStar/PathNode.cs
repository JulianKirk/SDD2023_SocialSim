using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    public int xPos;
    public int yPos;

    public int hCost; //Distance from the current position to the target position
    public int gCost; //Distance from the start position to the current position
    public int fCost; //Sum of h and g costs

    public bool walkable;

    public PathNode parent; //Set during pathfinding - will be a neighbouring node

    public PathNode(int x, int y, bool walk) 
    {
        xPos = x;
        yPos = y;

        gCost = 10000000; //Real gCosts should never be able to get this high so it is good for resetting later

        walkable = walk;

        parent = null; //Make sure this doesn't override any other calls
    }

    public PathNode(PathNode oldNode) 
    {
        xPos = oldNode.xPos;
        yPos = oldNode.yPos;
        gCost = 10000000;
        parent = null;

        walkable = oldNode.walkable;
    }

    public void UpdateFCost() 
    {
        fCost = hCost + gCost;
    }
}