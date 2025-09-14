using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    [TextArea] public string description;
    public ItemType type;
    public int value; // Ex: points de soin, puissance, prix

    public Sprite icon;
}

public enum ItemType
{
    Consommable,   // potion, nourriture
    Arme,
    Armure,
    Accessoire,
    CleOuSpecial
}