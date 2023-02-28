using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem")]
public class WeaponItem : InventoryItem
{
    //[HideInInspector]
    //new public bool isStackable = false; //No weapon is stackable so set it to false and hide it in Inspector

    public int damage = 1;


    public override bool IsStackable()
    {
        return false; //Every other InventoryItem BESIDES weapons are stackable
    }
}
