using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TriggerCombat : MonoBehaviour
{
    // Singleton instance
    public static TriggerCombat Instance { get; private set; }

    [Header("References")]
    public GridManager gridManager;

    [Header("UI Elements")]
    public Canvas uiCanvas; // Canvas pour les �l�ments UI
    public GameObject dialogueBubblePrefab; // Pr�fabriqu� pour la bulle de dialogue
    public bool requirePlayerConfirmation = false; // Si vrai, attend la confirmation du joueur pour fermer la bulle
    public TileMapHelper tilemapHelper;

    private Vector3 playerPreCombatPosition;

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Appeler quand le player et un ennemi se rencontre
    /// </summary>
    
    public void StartCombat(GameObject player, GameObject enemy)
     {
        playerPreCombatPosition = player.transform.position; // Sauvegarde la position du joueur avant le combat
        Tilemap currentTilemap = tilemapHelper.GetCurrentTilemap(playerPreCombatPosition);

        if (currentTilemap == null)
        {
            Debug.LogWarning("Pas de tilemap trouv� sous le joueur !");
            return;
        }

        if (player.GetComponent<PlayerUnit>() == null)
        {
            Debug.LogError("PlayerUnit est null !");
            return;
        }

        if (enemy.GetComponent<EnemyUnit>() == null)
        {
            Debug.LogError("EnemyUnit est null !");
            return;
        }

        Debug.Log($"Combat d�clench� ! Player: {player.name}, Enemy: {enemy.name}");

        // - Stop le d�placement du joueur et de l'ennemi
        enemy.GetComponent<EnnemyController>().StopMovement();

        player.GetComponent<PlayerController>().canMovefree = false;

        // - Marque le player et l'ennemi comme "en combat" (si n�cessaire)
        player.GetComponent<PlayerUnit>().isInCombat = true;
        enemy.GetComponent<EnemyUnit>().isInCombat = true;

        // - Affichage texte "D�but du combat"
        GameObject inst = null;
        DialogueBubble dialogueBubble = null;

        if(dialogueBubblePrefab != null && uiCanvas != null)
        {
            inst = Instantiate(dialogueBubblePrefab, uiCanvas.transform);
            dialogueBubble = inst.GetComponent<DialogueBubble>();

            if(dialogueBubble != null)
            {
                StartCoroutine(ShowAndThenStartBattle(dialogueBubble, player, enemy, inst));
            }
            else
            {
                Debug.LogWarning("Le prefab de la bulle de dialogue ne contient pas de composant DialogueBubble.");
            }
        } else
        {
            Debug.LogWarning("Le prefab de la bulle de dialogue ou le Canvas UI n'est pas assign� dans l'inspecteur.");
        }
        // - Charger la sc�ne de combat ou activer l'interface de combat
        PrepareBattlePosition(player.GetComponent<PlayerUnit>(), enemy.GetComponent<EnemyUnit>(), currentTilemap);
        ShowAndHideCombatGrid(currentTilemap);

        Camera.main.GetComponent<CameraController>().ModeOpenWorldOrCombat(true);

       // Camera.main.GetComponent<CameraController>().LockOn(player.transform);
        // - CombatManager � voir
    }

    public void PrepareBattlePosition(PlayerUnit player, EnemyUnit enemy, Tilemap combatTilemap)
    {
        // Limites de la tile map
        BoundsInt cellBounds = combatTilemap.cellBounds;

        // Calcul vertical centr�
        int centerY = (cellBounds.yMin + cellBounds.yMax) / 2;

        // Decalage horizontal pour �loigner les personnages du bord
        int horizontalOffset = 2;

        // Positions en coordonn�es de cellules
        Vector3Int playerCell = new Vector3Int(cellBounds.xMin + horizontalOffset, centerY, 0);
        Vector3Int enemyCell = new Vector3Int(cellBounds.xMax - horizontalOffset, centerY, 0);

        // Conversion en position monde
        Vector3 playerPos = combatTilemap.GetCellCenterWorld(playerCell);
        Vector3 enemyPos = combatTilemap.GetCellCenterWorld(enemyCell);

        // Mise � jour des positions
        player.transform.position = playerPos;
        enemy.transform.position = enemyPos;
    }

    public void ShowAndHideCombatGrid(Tilemap combatTileMap) 
    {
        Tilemap overlayTileMap = tilemapHelper.GetOverlayTilemap(); // R�cup�re la tilemap d'overlay

        if (overlayTileMap == null)
        {
            Debug.LogWarning("Aucune tilemap d'overlay trouv�e !");
            return;
        }

        // Tile � utiliser pour afficher la grille
        Tile baseTile = tilemapHelper.GetGridTile();

        if(baseTile == null)
        {
            Debug.LogWarning("Aucun tile de grille trouv� !");
            return;
        }

        if (overlayTileMap.HasTile(Vector3Int.zero))
        {
            // La grille est affich� -> on la supprime

            overlayTileMap.ClearAllTiles(); // Supprime toutes les tuiles pour masquer la grille
            Debug.Log("Grille de combat cach�e.");
        }
        else
        {
            // La grille est cach�e -> on la g�n�re

            overlayTileMap.ClearAllTiles();

            BoundsInt bounds = combatTileMap.cellBounds;

            // Parcours toutes les cellules occup�es
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (combatTileMap.HasTile(pos))
                {
                    overlayTileMap.SetTile(pos, baseTile);
                }
            }

            Debug.Log("Grille de combat affich�e.");
        }
    }

    private IEnumerator ShowAndThenStartBattle(DialogueBubble dialogueBubble, GameObject player, GameObject enemy, GameObject inst)
    {
        yield return dialogueBubble.ShowCoroutine("D�but du combat !", enemy.transform, requirePlayerConfirmation);

        Destroy(inst); // D�truire l'instance de la bulle de dialogue apr�s utilisation

        // Apr�s la bulle de dialogue, d�marrer le combat
        Debug.Log("D�marrage du combat...");
        // Ajouter ici la logique pour d�marrer le combat
    }
}
