using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{   
    private T[,] grid;
    float nodeSize; //Side of each square in the grid

    public Grid(int width, int length, float size) 
    {
        grid = new T[width, length];

        nodeSize = size;
    }

    public void SetValue(int x, int y, T value) //Does not account for index out of range error
    {
        grid[x, y] = value;
    }

    public T GetValue(int x, int y) //Does not account for index out of range error
    {
        return grid[x, y];
    }

    public List<T> GetNeighbours(int x, int y)
    {
        List<T> neighbours = new List<T>();
        
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, grid.GetLength(0)); i++) 
        {
            for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, grid.GetLength(1)); j++) 
            {
                neighbours.Add(grid[i, j]);
            }
        }

        return neighbours;
    }

    // public float CalculateDistance(int x1, int y1, int x2, int y2) //Each side of a tile is 10, Diagonals will be rounded to 14
    // {
    //     int xDistance = Mathf.Abs(x1 - x2);
    //     int yDistance = Mathf.Abs(y1 - y2);

    //     if(xDistance > yDistance) 
    //     {
    //         return (1.4 * yDistance + 1 * (xDistance - yDistance)) * nodeSize; //This is rounding root 2 to 2sigif
    //     } else
    //     {
    //         return (1.4 * xDistance + 1 * (yDistance - xDistance)) * nodeSize;
    //     }
    // }
}