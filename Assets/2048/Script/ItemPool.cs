using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool
{
    private static ItemPool instance;
    private Queue<Item> items = new Queue<Item>();

    public static ItemPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ItemPool();
            }
            return instance;
        }
    }

    public Item GetItem()
    {
        if (items.Count > 0)
        {
            Item item = items.Dequeue();
            item.gameObject.SetActive(true);
            return item;
        }
        else
        {
            GameObject item = Object.Instantiate(Resources.Load<GameObject>("Prefab/Item"));
            return item.GetComponent<Item>();
        }
    }

    public void RemoveItem(Item item)
    {
        item.gameObject.SetActive(false);
        items.Enqueue(item);
    }
}
