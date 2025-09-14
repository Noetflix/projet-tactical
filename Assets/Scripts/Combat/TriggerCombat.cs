using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCombat : MonoBehaviour
{
    // Singleton instance
    public static TriggerCombat Instance { get; private set; }

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
        Debug.Log($"Combat d�clench� ! Player: {player.name}, Enemy: {enemy.name}");
        // Ajouter ici la logique pour d�marrer le combat, comme charger une sc�ne de combat ou activer un mode combat
    }
}
