using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [Header("Classe choisie")]
    public CharacterClassData characterClass;

    [Header("Données du joueur")]
    public string characterName;
    public int currentHealth;
    public int currentMana;
    public List<Attack> attacks;
    public List<Item> inventory;

    private void Start()
    {
        // Initialiser les données du joueur en fonction de la classe choisie
        if (characterClass != null)
        {
            currentHealth = characterClass.baseHealth;
            currentMana = characterClass.baseMana;
            attacks = new List<Attack>(characterClass.attacks);
            inventory = new List<Item>(characterClass.inventory);
        }
        else
        {
            Debug.LogError("CharacterClassData is not assigned!");
        }
    }
}
