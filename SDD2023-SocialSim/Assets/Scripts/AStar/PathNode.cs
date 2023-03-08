using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    (int, int) position;
    int hCost;
    int gCost;
    int fCost;
    bool isWalkable;
    PathNode parent;

    public PathNode(int x, int y, bool walkable) 
    {
        position = (x, y);
        isWalkable = walkable;
    }

    void UpdateFCost() 
    {
        fCost = hCost + gCost;
    }
}