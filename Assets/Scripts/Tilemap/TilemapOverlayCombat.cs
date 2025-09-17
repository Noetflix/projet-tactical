using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tilemap/TilemapOverlayCombat")]
public class TilemapOverlayCombat : Tile
{
    public Color color = new Color(1f, 1f, 1f, 0.2f); // blanc semi-transparent

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }
}
