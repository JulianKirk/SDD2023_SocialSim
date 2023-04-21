using System; //For Lazy<>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinding
{
    private static readonly Lazy<AstarPathfinding> _instance = new Lazy<AstarPathfinding>(() => new AstarPathfinding());

    public static AstarPathfinding instance { get { return _instance.Value; } }

    private AstarPathfinding(){}

    // PathNode[,] DefaultGrid; --OLD GRID STUFF

    public List<PathNode> GeneratePath(PathNode startNode, PathNode endNode, /*bool[,] walkableGrid,*/ PathNode[,] baseGrid) 
    //Returns a list of nodes (their x-values are grid dependent). Adjusting for node width is up to the player
    {
        // Dictionary<(int, int), PathNode> Grid = new Dictionary<(int, int), PathNode>();
        // Grid<PathNode> nodeGrid = new Grid<PathNode>(100, 100);

        // Grid<bool> neighbourGrid = new Grid<bool>(100, 100); //Obtained from world gen

        // DefaultGrid = baseGrid; --OLD GRID STUFF

        if (endNode.walkable == false)
        {
            return null;
        }

        List<PathNode> OpenList = new List<PathNode>();
        List<PathNode> ClosedList = new List<PathNode>();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.UpdateFCost();
        startNode.parent = null;

        PathNode currentNode = startNode;
        OpenList.Add(currentNode);

        int tempIterationCount = 0;

        while (OpenList.Count > 0)
        {
            currentNode = SelectNewCurrentNode(OpenList);

            if(currentNode == endNode) 
            {
                Debug.Log("Astar Succeeded. Iterated " + tempIterationCount + " times.");
                return GenerateFinalPath(currentNode);
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourNodes(currentNode, baseGrid)) 
            {
                if(ClosedList.Contains(neighbourNode)) 
                {
                    continue;
                }

                int newNeigbourGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);

                if (neighbourNode.gCost > newNeigbourGCost /*|| !OpenList.Contains(neighbourNode)*/) 
                {
                    //If it is not in the OpenList already then it will not have had its gCost be set yet
                    neighbourNode.gCost = newNeigbourGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.parent = currentNode;

                    if (!OpenList.Contains(neighbourNode))
                    {
                        OpenList.Add(neighbourNode);
                    }
                }
            }

            tempIterationCount++;
        }

        Debug.Log("Astar Failed. Iterated " + tempIterationCount + " times");
        return null; //new List<PathNode>(); //Just return an empty list if it somehow fails
    }

    private List<PathNode> GenerateFinalPath(PathNode finalNode)
    {
        List<PathNode> path = new List<PathNode>();

        path.Add(finalNode);

        PathNode currentNode = finalNode;
        
        while (currentNode.parent != null) //Will not add the starting node
        {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    private List<PathNode> GetNeighbourNodes(PathNode centreNode, PathNode[,] DefaultGrid) 
    {
        List<PathNode> neighbourList = new List<PathNode>();

        //Add the neighbours
        for (int i = -1; i <= 1; i++) 
        {
            for (int j = -1; j <= 1; j++) 
            {
                int realX = centreNode.xPos + i;
                int realY = centreNode.yPos + j;

                if((i == 0 && j == 0) || realX < 0 || realX >= DefaultGrid.GetLength(0) || realY < 0 || realY >= DefaultGrid.GetLength(1)) 
                {
                    continue;
                }

                PathNode possibleNeighbour = DefaultGrid[realX, realY];

                if(possibleNeighbour.walkable && !neighbourList.Contains(possibleNeighbour)) 
                {
                    neighbourList.Add(possibleNeighbour); //Because arrays are reference types
                }
            }
        }

        return neighbourList;
    }

    private int CalculateDistance(PathNode Node1, PathNode Node2) //Each side of a tile is 10, Diagonals will be rounded to 14
    {
        int xDistance = Mathf.Abs(Node1.xPos - Node2.xPos);
        int yDistance = Mathf.Abs(Node1.yPos - Node2.yPos);

        if(xDistance > yDistance) 
        {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        } else
        {
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
    }

    public PathNode SelectNewCurrentNode(List<PathNode> nodeList) 
    {
        PathNode currentLowest = nodeList[0];
        foreach(PathNode node in nodeList) //Gets the node with the lowest f-cost
        {
            if(node.fCost < currentLowest.fCost || (node.fCost == currentLowest.fCost && node.hCost < currentLowest.hCost)) //Short circuits
            {                    
                currentLowest = node;
            }
        }
        
        return currentLowest;
    }
}
