using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    public int xPos;
    public int yPos;

    public int hCost;
    public int gCost; //Real gCosts should never be able to get this high
    public int fCost;

    public bool walkable;

    public PathNode parent;

    public PathNode(int x, int y, bool walk) 
    {
        xPos = x;
        yPos = y;

        gCost = 10000000;

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