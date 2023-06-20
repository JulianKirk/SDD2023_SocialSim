using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Resource
{

    // Start is called before the first frame update
    void Awake()
    {
        ResourceType = Item.Wood;

        m_inventory = new Inventory(50f);
        m_inventory.Add(Item.Wood, 16f);
    }
}
