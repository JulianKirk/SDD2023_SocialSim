using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Resource
{
    // Start is called before the first frame update
    void Awake()
    {
        ResourceType = Item.Stone;

        m_inventory = new Inventory(50f);
        m_inventory.Add(Item.Stone, 50f);
    }
}