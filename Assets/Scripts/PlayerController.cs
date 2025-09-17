using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))] // Assure que le GameObject a un composant Transform
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // Ref�rence au GridManager

    [Header("Travel")]
    public float moveDuration = 0.12f; // Dur�e du mouvement entre les cellules
    public bool canMovefree = true; // Si le joueur peut se d�placer librement
    private bool isMoving = false; // Marque si le joueur est en mouvement

    private GridManager.Cell currentCell; // Cellule actuelle du joueur

    void Start() // Appel� avant la premi�re frame de mise � jour
    {
        if (gridManager == null) // Si le GridManager n'est pas assign�
        {
            Debug.LogError("GridManager reference is missing!");
            return;
        }

        var bounds = gridManager.bounds; // R�cup�re les limites du GridManager

        if (bounds.size == Vector3Int.zero) // Si les limites ne sont pas initialis�es
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

        var cell = gridManager.GetCellByCellPosition(spawnCell); // R�cup�re la cellule de spawn

        if (cell == null) // Si la cellule de spawn est invalide
        {
            Debug.LogError("Spawn cell is invalid!");
            return;
        }

        currentCell = cell; // Fixe la cellule actuelle
        transform.position = currentCell.worldPosition; // Fixe la position du joueur � la position de la cellule
        currentCell.isOccupied = true; // Marque la cellule comme occup�e
    }

    void Update() // Appel� une fois par frame
    {
        if (isMoving) return; // Si le joueur est en mouvement, ne rien faire
        HandleKeyboardInput(); // G�rer les entr�es clavier

    }

    // --- Input Keyboard ---
    void HandleKeyboardInput()
    {
        if(!canMovefree) return; // Si le joueur ne peut pas se d�placer librement, ne rien faire

        int  dx=0, dy=0; // Direction de mouvement

        // V�rifie les entr�es de fl�ches directionnelles
        if (Input.GetKeyDown(KeyCode.UpArrow))    dy=1;
        else if (Input.GetKeyDown(KeyCode.DownArrow))  dy=-1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))  dx=-1;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) dx=1;

        if(dx != 0 || dy != 0)
        {
            Vector3Int targetCellPos = currentCell.cellPos + new Vector3Int(dx, dy, 0); // Calcule la position de la cellule cible
            TryMoveToCellPos(targetCellPos); // Tente de d�placer vers la cellule cible
        }
    }

    // --- Movement ---
    void TryMoveToCellPos(Vector3Int targetCellPos)
    {
        if (!gridManager.isInsideBounds(targetCellPos)) return; // V�rifie si la cellule cible est dans les limites

        var targetCell = gridManager.GetCellByCellPosition(targetCellPos); // R�cup�re la cellule cible

        if (targetCell == null)
        {
            return;
        }

        if (targetCell.isOccupied) // Si la cellule cible est occup�e, par un ennemie ou autre
        {
            Collider2D hit = Physics2D.OverlapPoint(targetCell.worldPosition); // V�rifie s'il y a un collider � la position de la cellule cible

            if(hit != null && hit.GetComponent<EnnemyController>() != null) // Si le collider appartient � un ennemi
            {
                OnPlayerEnnemyCollision(hit.GetComponent<EnnemyController>());
                // Ici, vous pouvez ajouter la logique de fin de jeu
            }

            return; // Ne pas se d�placer vers une cellule occup�e
        }

        StartCoroutine(MoveToCellRoutine(targetCell)); // D�marre la coroutine de mouvement
    }

    void OnPlayerEnnemyCollision(EnnemyController enemy) // G�re la collision avec un ennemi
    {
        Debug.Log("Collision Player <-> Ennemi !");

        // Appelle la m�thode StartCombat du singleton TriggerCombat
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

        targetCell.isOccupied = true; // Marque la cellule cible comme occup�e

        Vector3 start = transform.position; // Starting position
        Vector3 end = targetCell.worldPosition; // Target position
        float elapsed  = 0f; // Elapsed time for the movement

        while(elapsed < moveDuration) // Tant que la dur�e du mouvement n'est pas atteinte
        {
            elapsed += Time.deltaTime; // Temp �coul� depuis le dernier frame
            float t = Mathf.Clamp01(elapsed / moveDuration); // Calcul la progression du mouvement
            transform.position = Vector3.Lerp(start, end, t); // Positionne le joueur entre le d�but et la fin
            yield return null; // Termine la frame et continue au prochain frame
        }

        transform.position = end; // Assure que la position finale est correcte
        
        currentCell.isOccupied = false; // Lib�re l'ancienne cellule

        currentCell = targetCell; // Met � jour la cellule actuelle

        isMoving = false; // Marque le joueur comme immobile
    }
}
