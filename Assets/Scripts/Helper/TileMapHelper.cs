using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapHelper : MonoBehaviour
{
    public Tilemap[] allTilemaps; // Array to hold references to all TileMap objects in the scene
    public Tilemap overlayCombatTilemap; // TileMap for combat overlay
    public Tile gridTile; // Tile used for grid overlay

    public Tilemap GetOverlayTilemap() => overlayCombatTilemap;
    public Tile GetGridTile() => gridTile;

    public Tilemap GetCurrentTilemap(Vector3 playerPos)
    {
        foreach (var tilemap in allTilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int cellPos = tilemap.WorldToCell(playerPos);

            if (bounds.Contains(cellPos))
            {
                return tilemap; // Return the TileMap that contains the player's position
            }
        }
        return null; // Return null if no TileMap contains the player's position
    }
}
