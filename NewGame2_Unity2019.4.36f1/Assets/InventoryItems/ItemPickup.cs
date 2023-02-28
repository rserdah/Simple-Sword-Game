using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem inventoryItem;


    private void Start()
    {
        gameObject.tag = "ItemPickup";
    }
}
