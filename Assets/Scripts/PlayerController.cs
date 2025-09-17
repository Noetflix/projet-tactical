using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))] // Assure que le GameObject a un composant Transform
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // Reférence au GridManager

    [Header("Travel")]
    public float moveDuration = 0.12f; // Durée du mouvement entre les cellules
    public bool canMovefree = true; // Si le joueur peut se déplacer librement
    private bool isMoving = false; // Marque si le joueur est en mouvement

    private GridManager.Cell currentCell; // Cellule actuelle du joueur

    void Start() // Appelé avant la première frame de mise à jour
    {
        if (gridManager == null) // Si le GridManager n'est pas assigné
        {
            Debug.LogError("GridManager reference is missing!");
            return;
        }

        var bounds = gridManager.bounds; // Récupère les limites du GridManager

        if (bounds.size == Vector3Int.zero) // Si les limites ne sont pas initialisées
        {
            Debug.LogError("GridManager bounds are not initialized!");
            return;
        }

        // Fixe la position de spawn au centre de la bordure gauche
        Vector3Int spawnCell = new Vector3Int(
            bounds.xMin,
            bounds.yMin + (bounds.size.y / 2),
            0
            );

        var cell = gridManager.GetCellByCellPosition(spawnCell); // Récupère la cellule de spawn

        if (cell == null) // Si la cellule de spawn est invalide
        {
            Debug.LogError("Spawn cell is invalid!");
            return;
        }

        currentCell = cell; // Fixe la cellule actuelle
        transform.position = currentCell.worldPosition; // Fixe la position du joueur à la position de la cellule
        currentCell.isOccupied = true; // Marque la cellule comme occupée
    }

    void Update() // Appelé une fois par frame
    {
        if (isMoving) return; // Si le joueur est en mouvement, ne rien faire
        HandleKeyboardInput(); // Gérer les entrées clavier

    }

    // --- Input Keyboard ---
    void HandleKeyboardInput()
    {
        if(!canMovefree) return; // Si le joueur ne peut pas se déplacer librement, ne rien faire

        int  dx=0, dy=0; // Direction de mouvement

        // Vérifie les entrées de flèches directionnelles
        if (Input.GetKeyDown(KeyCode.UpArrow))    dy=1;
        else if (Input.GetKeyDown(KeyCode.DownArrow))  dy=-1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))  dx=-1;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) dx=1;

        if(dx != 0 || dy != 0)
        {
            Vector3Int targetCellPos = currentCell.cellPos + new Vector3Int(dx, dy, 0); // Calcule la position de la cellule cible
            TryMoveToCellPos(targetCellPos); // Tente de déplacer vers la cellule cible
        }
    }

    // --- Movement ---
    void TryMoveToCellPos(Vector3Int targetCellPos)
    {
        if (!gridManager.isInsideBounds(targetCellPos)) return; // Vérifie si la cellule cible est dans les limites

        var targetCell = gridManager.GetCellByCellPosition(targetCellPos); // Récupère la cellule cible

        if (targetCell == null)
        {
            return;
        }

        if (targetCell.isOccupied) // Si la cellule cible est occupée, par un ennemie ou autre
        {
            Collider2D hit = Physics2D.OverlapPoint(targetCell.worldPosition); // Vérifie s'il y a un collider à la position de la cellule cible

            if(hit != null && hit.GetComponent<EnnemyController>() != null) // Si le collider appartient à un ennemi
            {
                OnPlayerEnnemyCollision(hit.GetComponent<EnnemyController>());
                // Ici, vous pouvez ajouter la logique de fin de jeu
            }

            return; // Ne pas se déplacer vers une cellule occupée
        }

        StartCoroutine(MoveToCellRoutine(targetCell)); // Démarre la coroutine de mouvement
    }

    void OnPlayerEnnemyCollision(EnnemyController enemy) // Gère la collision avec un ennemi
    {
        Debug.Log("Collision Player <-> Ennemi !");

        // Appelle la méthode StartCombat du singleton TriggerCombat
        if (TriggerCombat.Instance != null)
        {
            TriggerCombat.Instance.StartCombat(this.gameObject, enemy.gameObject);
        }
        else
        {
            Debug.LogError("TriggerCombat instance is null!");
        }
    }

    IEnumerator MoveToCellRoutine(GridManager.Cell targetCell)
    {
        isMoving = true; // Marque le joueur comme en mouvement

        targetCell.isOccupied = true; // Marque la cellule cible comme occupée

        Vector3 start = transform.position; // Starting position
        Vector3 end = targetCell.worldPosition; // Target position
        float elapsed  = 0f; // Elapsed time for the movement

        while(elapsed < moveDuration) // Tant que la durée du mouvement n'est pas atteinte
        {
            elapsed += Time.deltaTime; // Temp écoulé depuis le dernier frame
            float t = Mathf.Clamp01(elapsed / moveDuration); // Calcul la progression du mouvement
            transform.position = Vector3.Lerp(start, end, t); // Positionne le joueur entre le début et la fin
            yield return null; // Termine la frame et continue au prochain frame
        }

        transform.position = end; // Assure que la position finale est correcte
        
        currentCell.isOccupied = false; // Libère l'ancienne cellule

        currentCell = targetCell; // Met à jour la cellule actuelle

        isMoving = false; // Marque le joueur comme immobile
    }
}
