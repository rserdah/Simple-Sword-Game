using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InventoryItem")]
public class InventoryItem : ScriptableObject
{
    new public string name;
    public Texture image;
    public string description;

    //public bool isStackable;

    public virtual bool IsStackable()
    {
        return true; //Every other InventoryItem BESIDES weapons are stackable (so WeaponItem.IsStackable() always returns false but for every other item it will be true)
    }
}
