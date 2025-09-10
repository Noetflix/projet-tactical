using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{

    public Tilemap tilemap; // R�f�rence au Tilemap

    // Classe repr�sentant une cellule dans la grille
    public class Cell
    {
        public Vector3 worldPosition; // Position dans le monde
        public Vector3Int cellPos; // Position dans la grille (cellule)
        public bool isOccupied = false; // Si la cellule est occup�e
    }

    private Dictionary<Vector3Int, Cell> cells; // Dictionnaire des cellules par position de cellule
    public BoundsInt bounds { get; private set; }

    // Awake est appel� lorsque l'instance du script est charg�e
    void Awake()
    {
        if (tilemap == null) // Si le Tilemap n'est pas assign�
        {
            Debug.LogError("Tilemap reference is missing!");
            return;
        }

        InitializeGridFromTilemapBounds();
    }

    void InitializeGridFromTilemapBounds() // Initialise la grille � partir des limites du Tilemap
    {
        bounds = tilemap.cellBounds; // R�cup�re les limites du Tilemap
        cells = new Dictionary<Vector3Int, Cell>(); // Initialise le dictionnaire des cellules

        for (int x = bounds.xMin; x < bounds.xMax; x++) // Boucle � travers l'axe x des limites
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++) // Boucle � travers l'axe y des limites
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0); // Cree la position de la cellule
                Vector3  worldPos = tilemap.GetCellCenterWorld(cellPos); // Recup�re la position dans le monde de la cellule

                // Seulement ajouter la cellule si elle existe dans le Tilemap
                Cell c = new Cell
                {
                    cellPos = cellPos,
                    worldPosition = worldPos,
                    isOccupied = false
                };

                cells[cellPos] = c; // Ajoute la cellule au dictionnaire
            }
        }

        Debug.Log($"Grid initialized with {cells.Count} cells.");
    }

    public Cell GetCellByCellPosition(Vector3Int cellPos)  // R�cup�re la cellule � la position de cellule sp�cifi�e
    {
        cells.TryGetValue(cellPos, out Cell c); // Essaye de r�cup�rer la cellule du dictionnaire
        return c; // Retourne la cellule (ou null si elle n'existe pas)
    }

    public Cell GetCellByIndex(int indexX, int indexY) // Recuperer la cellule aux indices sp�cifi�s
    {
        Vector3Int cellPos = new Vector3Int(bounds.xMin + indexX, bounds.yMin + indexY, 0); // Calculer la position de la cellule
        return GetCellByCellPosition(cellPos); // Retourner la cellule � cette position
    }

    public Cell GetCellFromWorldPosition(Vector3 worldPos) // Recup�re la cellule � partir de la position dans le monde
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos); // Convertit la position dans le monde en position de cellule
        return GetCellByCellPosition(cellPos); // Retourne la cellule � cette position
    }

    public bool isInsideBounds(Vector3Int cellPos) // Verifie si la position de la cellule est � l'int�rieur des limites
    {
        return cellPos.x >= bounds.xMin && cellPos.x < bounds.xMax &&
               cellPos.y >= bounds.yMin && cellPos.y < bounds.yMax;
    }

    public Vector3Int indexToCellPos(int indexX, int indexY) // Convertit les indices en position de cellule
    {
        return new Vector3Int(bounds.xMin + indexX, bounds.yMin + indexY, 0);
    }

    void OnDrawGizmosSelected() // Dessine les gizmos dans l'�diteur pour visualiser la grille, Debugging
    {
        if (tilemap == null) return;
        var b = tilemap.cellBounds;
        Gizmos.color = Color.green;
        for (int x = b.xMin; x < b.xMax; x++)
        {
            for (int y = b.yMin; y < b.yMax; y++)
            {
                Vector3Int p = new Vector3Int(x, y, 0);
                Vector3 world = tilemap.GetCellCenterWorld(p);
                Gizmos.DrawWireCube(world, Vector3.one * 0.9f);
            }
        }
    }
}
