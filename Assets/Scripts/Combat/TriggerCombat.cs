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
    public Canvas uiCanvas; // Canvas pour les éléments UI
    public GameObject dialogueBubblePrefab; // Préfabriqué pour la bulle de dialogue
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
            Debug.LogWarning("Pas de tilemap trouvé sous le joueur !");
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

        Debug.Log($"Combat déclenché ! Player: {player.name}, Enemy: {enemy.name}");

        // - Stop le déplacement du joueur et de l'ennemi
        enemy.GetComponent<EnnemyController>().StopMovement();

        player.GetComponent<PlayerController>().canMovefree = false;

        // - Marque le player et l'ennemi comme "en combat" (si nécessaire)
        player.GetComponent<PlayerUnit>().isInCombat = true;
        enemy.GetComponent<EnemyUnit>().isInCombat = true;

        // - Affichage texte "Début du combat"
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
            Debug.LogWarning("Le prefab de la bulle de dialogue ou le Canvas UI n'est pas assigné dans l'inspecteur.");
        }
        // - Charger la scène de combat ou activer l'interface de combat
        PrepareBattlePosition(player.GetComponent<PlayerUnit>(), enemy.GetComponent<EnemyUnit>(), currentTilemap);
        ShowAndHideCombatGrid(currentTilemap);

        Camera.main.GetComponent<CameraController>().ModeOpenWorldOrCombat(true);

       // Camera.main.GetComponent<CameraController>().LockOn(player.transform);
        // - CombatManager à voir
    }

    public void PrepareBattlePosition(PlayerUnit player, EnemyUnit enemy, Tilemap combatTilemap)
    {
        // Limites de la tile map
        BoundsInt cellBounds = combatTilemap.cellBounds;

        // Calcul vertical centré
        int centerY = (cellBounds.yMin + cellBounds.yMax) / 2;

        // Decalage horizontal pour éloigner les personnages du bord
        int horizontalOffset = 2;

        // Positions en coordonnées de cellules
        Vector3Int playerCell = new Vector3Int(cellBounds.xMin + horizontalOffset, centerY, 0);
        Vector3Int enemyCell = new Vector3Int(cellBounds.xMax - horizontalOffset, centerY, 0);

        // Conversion en position monde
        Vector3 playerPos = combatTilemap.GetCellCenterWorld(playerCell);
        Vector3 enemyPos = combatTilemap.GetCellCenterWorld(enemyCell);

        // Mise à jour des positions
        player.transform.position = playerPos;
        enemy.transform.position = enemyPos;
    }

    public void ShowAndHideCombatGrid(Tilemap combatTileMap) 
    {
        Tilemap overlayTileMap = tilemapHelper.GetOverlayTilemap(); // Récupère la tilemap d'overlay

        if (overlayTileMap == null)
        {
            Debug.LogWarning("Aucune tilemap d'overlay trouvée !");
            return;
        }

        // Tile à utiliser pour afficher la grille
        Tile baseTile = tilemapHelper.GetGridTile();

        if(baseTile == null)
        {
            Debug.LogWarning("Aucun tile de grille trouvé !");
            return;
        }

        if (overlayTileMap.HasTile(Vector3Int.zero))
        {
            // La grille est affiché -> on la supprime

            overlayTileMap.ClearAllTiles(); // Supprime toutes les tuiles pour masquer la grille
            Debug.Log("Grille de combat cachée.");
        }
        else
        {
            // La grille est cachée -> on la génère

            overlayTileMap.ClearAllTiles();

            BoundsInt bounds = combatTileMap.cellBounds;

            // Parcours toutes les cellules occupées
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (combatTileMap.HasTile(pos))
                {
                    overlayTileMap.SetTile(pos, baseTile);
                }
            }

            Debug.Log("Grille de combat affichée.");
        }
    }

    private IEnumerator ShowAndThenStartBattle(DialogueBubble dialogueBubble, GameObject player, GameObject enemy, GameObject inst)
    {
        yield return dialogueBubble.ShowCoroutine("Début du combat !", enemy.transform, requirePlayerConfirmation);

        Destroy(inst); // Détruire l'instance de la bulle de dialogue après utilisation

        // Après la bulle de dialogue, démarrer le combat
        Debug.Log("Démarrage du combat...");
        // Ajouter ici la logique pour démarrer le combat
    }
}
