using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBlock : MonoBehaviour
{
    /*
    Hierarchy of an ItemBlock Transform:
        ItemBlock
            Background
            ItemImage
            ItemName
            DamageTitle (just Text that says "Damage"; don't edit)
                DamageText
            DescriptionText
    */

    private RawImage itemImage;
    public Text itemName;
    public Text itemDamage;
    public Text itemDescription;

    public RawImage background;

    private Color normal;
    public Color selectedTint = Color.gray;


    public void Initialize()
    {
        itemImage = transform.GetChild(1).GetComponent<RawImage>();
        itemName = transform.GetChild(2).GetComponent<Text>();
        itemDamage = transform.GetChild(3).GetChild(0).GetComponent<Text>();
        itemDescription = transform.GetChild(4).GetComponent<Text>();

        background = transform.GetChild(0).GetComponent<RawImage>();
        normal = background.color;
    }

    public void Print(InventoryItem inventoryItem)
    {
        itemImage.texture = inventoryItem.image;
        itemName.text = inventoryItem.name;

        if(inventoryItem is WeaponItem)
            itemDamage.text = ((WeaponItem)inventoryItem).damage + "";
        else
            itemDamage.text = "";

        itemDescription.text = inventoryItem.description;
    }

    public void Select()
    {
        background.color = normal * selectedTint;
    }

    public void Deselect()
    {
        background.color = normal;
    }
}
