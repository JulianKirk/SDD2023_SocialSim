using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AstarPathfinding
{
    public List<(int, int)> GeneratePath(PathNode startNode, PathNode endNode) 
    {
        Dictionary<(int, int), PathNode> Grid = new Dictionary<(int, int), PathNode>();

        List<PathNode> OpenList;
        List<PathNode> ClosedList;

        startNode.gCost = 0;
        startNode.parent = null;

        PathNode currentNode = startNode;
        OpenList.Add(currentNode);

        while (OpenList.Count > 0)
        {
            if(currentNode == endNode) 
            {
                return GenerateFinalPath(currentNode);

            }

            foreach (PathNode neighbourNode in GetNeighbourNodes(currentNode, OpenList)) 
            {
                if(ClosedList.Contains(node)) 
                {
                    continue;
                }

                int newNeigbourGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);

                if (neighbourNode.gCost > newNeigbourGCost || !OpenList.Contains(neighbourNode)) 
                {
                    //If it is not in the OpenList already then it will not have had its gCost be set yet
                    neighbourNode.gCost = newNeigbourGCost;
                    neighbourNode.hCost = GetDistance(neighbourNode, endNode);
                    neighbourNode.parent = currentNode;

                    if (!OpenList.Contains(neighborNode))
                    {
                        OpenList.Add(neighborNode);
                    }
                }
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            foreach(PathNode node in OpenList) //Set a new node to focus on 
            {
                if(node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost)) 
                {                    
                    currentNode = node;
                }
            }
        }
    }

    private List<(int, int)> GenerateFinalPath(PathNode finalNode)
    {

    }

    private List<PathNode> GetNeighbourNodes(PathNode centreNode, List<PathNode> openList) 
    {
        List<PathNode> neighbourList;

        //Add the neighbours

        return neighbourList;
    }

    private int CalculateDistance(PathNode Node1, PathNode Node2) //Each side of a tile is 10, Diagonals will be rounded to 14
    {
        int xDistance = Mathf.Abs(Node1.x - Node2.x);
        int yDistance = Mathf.Abs(Node1.x - Node2.x);

        if(xDistance > yDistance) 
        {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        } else
        {
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
    }
}
