using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterClass", menuName = "RPG/Character Class", order = 0)]
public class CharacterClassData : ScriptableObject
{
    [Header("Informations Générales")]
    public string className;
    [TextArea] public string classDescription;
    public Sprite classIcon;

    [Header("Statistiques de Base")]
    public int baseHealth = 100;
    public int baseMana = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int initiative = 5;
    public int baseMovement = 3;

    [Header("Croissance des Statistiques par Niveau")]
    public int healthPerLevel = 10;
    public int manaPerLevel = 5;
    public int attackPerLevel = 2;
    public int defensePerLevel = 1;
    public int initiativePerLevel = 1;
    public int movementPerLevel = 0;

    [Header("Attaques")]
    public List<Attack> attacks = new List<Attack>(); // Liste des noms des compétences ou attaques

    [Header("Equipements")]
    public List<Item> inventory = new List<Item>(); // Liste des noms des équipements de départ
}
