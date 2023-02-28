using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public struct ItemCountPair
    {
        public ItemCountPair(InventoryItem _item, int _count)
        {
            item = _item;
            count = _count;

            name = _item ? _item.name : "NullItem" + Random.Range(0, 10000);
        }

        [HideInInspector]
        public string name; //Mostly non-functional, only so it automatically names the List element with this name (also has to be public and the first var. in the struct), but the actual variable is hidden so don't have to see the variable

        public InventoryItem item;
        public int count;


        public void Add(int addCount)
        {
            count += addCount;
        }
    }

    private int index = 0;
    public int itemBlockCount;
    public Vector2 printedRange;

    public ItemBlock[] itemBlocks;

    public List<ItemCountPair> inventory = new List<ItemCountPair>();


    private void Start()
    {
        for(int i = 0; i < itemBlocks.Length; i++)
        {
            itemBlocks[i].Initialize();
        }

        itemBlockCount = itemBlocks.Length;

        printedRange = new Vector2(0, itemBlockCount - 1);

        PrintScreen();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(index > 0)
            {
                PreScreenChange();

                Scroll(-1);

                PrintScreen();
            }
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(index < inventory.Count - 1)
            {
                PreScreenChange();

                Scroll(1);

                PrintScreen();
            }
        }
    }

    private bool IsInRange()
    {
        return index >= printedRange.x && index <= printedRange.y;
    }

    private void Scroll(int amount)
    {
        if(index + amount < inventory.Count && index + amount >= 0)
        {
            index += amount;

            if(!IsInRange())
                printedRange += amount * Vector2.one; //Increment both the min and max of the range by the amount
        }
    }

    private bool IsOnFirstItem()
    {
        return index % itemBlockCount == 0;
    }

    private bool IsOnLastItem()
    {
        return index % itemBlockCount == itemBlockCount - 1;
    }

    private void PreScreenChange()
    {
        itemBlocks[IndexToScreenIndex(index)].Deselect();
    }

    public void PrintScreen()
    {
        int i, j;

        for(i = 0, j = (int)printedRange.x; i < itemBlockCount; i++, j++)
        {
            itemBlocks[i].Print(inventory[j].item);

            if(j == index)
                itemBlocks[i].Select();
        }
    }

    public void AddToInventory(InventoryItem item, int count = 1)
    {
        int index = IndexOf(item);

        if(index > -1 && item.IsStackable()) //If already has this InventoryItem (and the InventoryItem is stackable, if not stackable, add a new one in as a new element)
        {
            //Can't directly change the InventoryItem struct's count and make it change in the List so have to copy it to another InventoryItem, change the count, and then re-assign it at the same index
            ItemCountPair i = inventory[index];
            i.count += count;
            inventory[index] = i;
            Debug.LogError("Has this item at index " + index);
        }
        else //If does not already have this InventoryItem
        {
            ItemCountPair i = new ItemCountPair(item, count);

            inventory.Add(i);
        }
    }

    public int IndexOf(InventoryItem item)
    {
        for(int i = 0; i < inventory.Count; i++)
        {
            if(inventory[i].item.Equals(item))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Converts from the index in the inventory to the corresponding index of all the ItemBlocks
    /// </summary>
    /// <returns></returns>
    private int IndexToScreenIndex(int i)
    {
        return i - (int)printedRange.x;
    }
}
