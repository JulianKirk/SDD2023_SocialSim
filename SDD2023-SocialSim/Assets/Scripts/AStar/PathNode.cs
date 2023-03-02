using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    public PathNode(int x, int y, bool walkable) 
    {
        position = (x, y);
        isWalkable = walkable;
    }

    (int, int) position; 
    int hCost;
    int gCost;
    int fCost;

    bool isWalkable;

    void UpdateFCost() 
    {
        fCost = hCost + gCost;
    }
}