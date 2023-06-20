using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPlant : Resource
{

    // Start is called before the first frame update
    void Awake()
    {
        ResourceType = Item.Fruit;

        m_inventory = new Inventory(50f);
        m_inventory.Add(Item.Fruit, 8f);
    }
}
