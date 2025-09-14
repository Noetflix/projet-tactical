using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs des classes")]
    public GameObject playerArcherPrefab;
    public GameObject playerWarriorPrefab;
    public GameObject playerMagePrefab;

    void Start()
    {
        string chosenClass = MenuClasses.SelectedClass;
        GameObject prefabToSpawn = null;

        // Déterminer quel prefab instancier en fonction de la classe choisie
        switch (chosenClass)
        {
            case "Archer":
                prefabToSpawn = playerArcherPrefab;
                break;
            case "Warrior":
                prefabToSpawn = playerWarriorPrefab;
                break;
            case "Mage":   
                prefabToSpawn = playerMagePrefab;
                break;
            default:
                Debug.LogWarning("Aucune Classe choisie");
                break;
        }

        if (prefabToSpawn != null)
        {
            GameObject playerInstance = Instantiate(prefabToSpawn);

            GridManager gridManager = FindObjectOfType<GridManager>();

            if (gridManager != null)
            {
                PlayerController pc = playerInstance.GetComponent<PlayerController>();

                if (pc != null)
                {
                    pc.gridManager = gridManager;
                }
                else
                {
                    Debug.LogError("Le prefab du joueur n'a pas de PlayerController attaché.");
                }
            }
            else
            {
                Debug.LogError("Aucun GridManager trouvé dans la scène.");
            }
        }
    }
}
