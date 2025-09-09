using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{

    public Tilemap tilemap; // Reference to the Tilemap component
    public int rows = 10; // Number of rows in the grid
    public int cols = 15; // Number of columns in the grid

    public class Cell
    {
        public Vector3 worldPosition; // World position of the cell
        public Vector3Int gridPosition; // Grid position of the cell
        public bool isOccupied = false; // Whether the cell is occupied
    }

    private Cell[,] grid; // 2D array to hold the grid cells

    void Awake() // Called when the script instance is being loaded
    {
        InitializeGrid();
    }

    void InitializeGrid() // Initializes the grid based on the tilemap
    {
        grid = new Cell[rows, cols];

        // Loop through each cell in the grid
        for (int x = 0; x < rows; x++) 
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0); // Grid position
                Vector3 worldPosition = tilemap.CellToWorld(gridPosition) + tilemap.tileAnchor; // Convert to world position
                grid[x, y] = new Cell // Create a new cell
                {
                    worldPosition = worldPosition,
                    gridPosition = gridPosition,
                    isOccupied = false
                };
            }
        }
    }

    public Cell GetCell(int x, int y) // Returns the cell at the specified grid position
    {
        if (x >= 0 && x < rows && y >= 0 && y < cols) // Check bounds
        {
            return grid[x, y];
        }

        return null; // Return null if out of bounds
    }

    public Cell GetCellFromWorldPosition(Vector3 worldPosition) // Returns the cell at the specified world position
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPosition); // Convert world position to cell position
        return GetCell(cellPos.x, cellPos.y); // Get the cell at that position
    }
}
