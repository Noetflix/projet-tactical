using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    // Détails de l'attaque
    public string attackName;
    public string description;
    public int damage;
    public int manaCost;
    public float range;
    public AttackType type;
    public Sprite icon;

    // Types d'attaques
    public enum AttackType
    {
        Physique,
        Perforant,
        Magique,
        Soin,
        Buff,
        Debuff
    }
}
