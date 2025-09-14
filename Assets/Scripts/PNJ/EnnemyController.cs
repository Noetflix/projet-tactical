using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // Ref�rence au GridManager

    [Header("Param�tres IA")]
    public float moveInterval = 5.0f; // Temps entre les mouvements de l'ennemi
    public float moveDuration = 0.12f; // Dur�e du mouvement entre les cellules

    [HideInInspector] public GridManager.Cell currentCell; // Cellule actuelle du joueur

    private bool isMoving = false; // Marque si l'ennemi est en mouvement

    // Start is called before the first frame update
    void Start()
    {
        if (gridManager == null) // Si le GridManager n'est pas assign�
        {
            Debug.LogError("GridManager reference is missing!");
            return;
        }

        PlaceRandomly();
        StartCoroutine(RandomMovementRoutine());
    }

    public void PlaceRandomly() // Place l'ennemi al�atoirement sur la grille
    {
        var bounds = gridManager.bounds; // R�cup�re les limites du GridManager

        // Parcourt toutes les cellules dans les limites
        for (int i = 0; i < 100; i++)
        {
            int randX = Random.Range(bounds.xMin, bounds.xMax); // G�n�re une position x al�atoire dans les limites
            int randY = Random.Range(bounds.yMin, bounds.yMax); // G�n�re une position y al�atoire dans les limites

            Vector3Int candidatePos = new Vector3Int(randX, randY, 0); // Position candidate
            var cell = gridManager.GetCellByCellPosition(candidatePos); // R�cup�re la cellule candidate

            if (cell != null && !cell.isOccupied) // Si la cellule est valide et non occup�e
            {
                currentCell = cell; // Fixe la cellule actuelle
                transform.position = currentCell.worldPosition; // Fixe la position de l'ennemi � la position de la cellule
                currentCell.isOccupied = true; // Marque la cellule comme occup�e
                return; // Sort de la fonction
            }
        }

        Debug.LogError("Failed to place enemy: no free cells available.");

    }

    IEnumerator RandomMovementRoutine() // Routine de mouvement al�atoire
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval); // Attendre l'intervalle de temps

            if (!isMoving) // Si l'ennemi n'est pas en mouvement
            {
                AttemptRandomMove(); // Tente un mouvement al�atoire
            }
        }
    }

    public void AttemptRandomMove() // Tente un mouvement al�atoire
    {

        Vector3Int[] directions = new Vector3Int[] // Directions possibles (haut, bas, gauche, droite)
        {
            new Vector3Int(0, 1, 0),   // Haut
            new Vector3Int(0, -1, 0),  // Bas
            new Vector3Int(-1, 0, 0),  // Gauche
            new Vector3Int(1, 0, 0)    // Droite
        };

        Vector3Int dir = directions[Random.Range(0, directions.Length)]; // Choisit une direction al�atoire

        Vector3Int targetPos = new Vector3Int(
            currentCell.cellPos.x + dir.x,
            currentCell.cellPos.y + dir.y,
            0
            ); // Calcule la position cible

        if (!gridManager.isInsideBounds(targetPos)) // V�rifie si la cellule cible est dans les limites
        {
            dir *= -1; // Inverse la direction
            targetPos = new Vector3Int(
                currentCell.cellPos.x + dir.x,
                currentCell.cellPos.y + dir.y,
                0
                ); // Recalcule la position cible

            if (!gridManager.isInsideBounds(targetPos))
                return;
        }


        var targetCell = gridManager.GetCellByCellPosition(targetPos); // R�cup�re la cellule cible


        if (targetCell == null)
        {
            Debug.LogError("Target cell is null!");
            return;
        }

        if (targetCell.isOccupied) // Si la cellule cible est occup�e par un autre ennemi, player ou autre
        {
            CheckCollisionWithPlayer(targetCell); // V�rifie la collision avec le joueur
            return;
        }

        StartCoroutine(MoveToCellRoutine(targetCell)); // D�marre la coroutine de d�placement vers la cellule cible
    }

    IEnumerator MoveToCellRoutine(GridManager.Cell targetCell) // Coroutine de d�placement vers une cellule cible
    {
        isMoving = true; // Marque l'ennemie comme en mouvement

        targetCell.isOccupied = true; // Marque la cellule cible comme occup�e

        Vector3 start = transform.position; // Starting position
        Vector3 end = targetCell.worldPosition; // Target position
        float elapsed = 0f; // Elapsed time for the movement

        while (elapsed < moveDuration) // Tant que la dur�e du mouvement n'est pas atteinte
        {
            elapsed += Time.deltaTime; // Temp �coul� depuis le dernier frame
            float t = Mathf.Clamp01(elapsed / moveDuration); // Calcul la progression du mouvement
            transform.position = Vector3.Lerp(start, end, t); // Positionne l'ennemie entre le d�but et la fin
            yield return null; // Termine la frame et continue au prochain frame
        }

        transform.position = end; // Assure que la position finale est correcte

        currentCell.isOccupied = false; // Lib�re l'ancienne cellule

        currentCell = targetCell; // Met � jour la cellule actuelle

        isMoving = false; // Marque le joueur comme immobile
    }

    void CheckCollisionWithPlayer(GridManager.Cell targetCell) // V�rifie la collision avec le joueur
    {
        Collider2D hit = Physics2D.OverlapPoint(targetCell.worldPosition); // V�rifie s'il y a un collider � la position de la cellule actuelle

        if (hit != null && hit.GetComponent<PlayerController>() != null) // Si le collider appartient au joueur
        {
            if (TriggerCombat.Instance != null) // Appelle la m�thode StartCombat du singleton TriggerCombat
            {
                TriggerCombat.Instance.StartCombat(hit.gameObject, this.gameObject);
            }
            else
            {
                Debug.LogError("TriggerCombat instance is null!");
            }
        }
    }
}
