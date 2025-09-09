using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GridManager gridManager; // Reference to the GridManager
    private GridManager.Cell currentCell; // Current cell the player is on

    void Start() // Called before the first frame update
    {
        if (gridManager == null) // If GridManager is not assigned
        {
            Debug.LogError("GridManager reference is missing!");
            return;
        }

        BoundsInt bounds = gridManager.tilemap.cellBounds; // Get the bounds of the tilemap

        Vector3Int spawnCell = new Vector3Int(bounds.xMin, bounds.yMin + bounds.size.y / 2, 0); // Calculate spawn cell position

        currentCell = gridManager.GetCell(spawnCell.x - bounds.xMin, spawnCell.y - bounds.yMin); // Get the cell at the spawn position

        if (currentCell != null) // If the cell is valid
        {
            transform.position = gridManager.tilemap.GetCellCenterWorld(spawnCell); // Set player position to the cell's world position
            currentCell.isOccupied = true; // Mark the cell as occupied
        }
        else
        {
            Debug.LogError("Spawn cell is out of bounds!");
        }
    }
}
