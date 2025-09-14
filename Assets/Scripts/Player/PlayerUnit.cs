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
            characterName = RpgNameGenerator.QuickName();
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

    public void UseBasiqueAttack(GameObject target)
    {
        int damage = characterClass.baseAttack;
        Debug.Log(characterClass.className + " utilise son attaque de base et inflige " + damage + " dégâts !");

        var enemyUnit = target.GetComponent<EnemyUnit>();

        if (enemyUnit != null)
        {
            enemyUnit.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("Target does not have an EnemyUnit component!");
        }
    }

    public void UseSpecialAttack(Attack attack, GameObject target)
    {
        var enemyUnit = target.GetComponent<EnemyUnit>();

        int totalDamage = characterClass.baseAttack + attack.damage;

        // Gérer les différents types d'attaques
        if (attack.type == Attack.AttackType.Soin)
        {
            currentHealth += attack.damage;
            currentHealth = Mathf.Min(currentHealth, characterClass.baseHealth);
            Debug.Log(characterClass.className + " utilise " + attack.attackName + " et soigne " + attack.damage + " points de vie !");
            return;
        }

        if (attack.type == Attack.AttackType.Buff)
        {
             // Implémenter la logique de buff ici
             Debug.Log(characterClass.className + " utilise " + attack.attackName + " et reçoit un buff !");
             return;
        }

        if(attack.type == Attack.AttackType.Debuff)
        {
             // Implémenter la logique de debuff ici
             Debug.Log(characterClass.className + " utilise " + attack.attackName + " et inflige un debuff à l'ennemi !");
             return;
        }

        if (currentMana < attack.manaCost)
        {
            Debug.Log(characterClass.className + " n'a pas assez de mana pour utiliser " + attack.attackName + " !");
            return;
        }

        if(attack.manaCost > 0)
        {
            currentMana -= attack.manaCost;
        }

        if(attack.damage > 0)
        {
            Debug.Log(characterClass.className + " utilise " + attack.attackName + " et inflige " + totalDamage + " dégâts !");

            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(totalDamage);
            }
            else
            {
                Debug.LogError("Target does not have an EnemyUnit component!");
            }
        }
        
    }
}
