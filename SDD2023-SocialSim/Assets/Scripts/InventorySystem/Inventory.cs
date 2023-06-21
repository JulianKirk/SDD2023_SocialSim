using System; //For Lazy<>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    Water,
    Fruit,
    Meat,
    Grass,
    Wood,
    Stone,
    Metal
}

public class Inventory
{
    public float m_currentWeight; //in KGs
    public float m_maxWeight; //in KGs

    private Dictionary<Item, float> m_items = new Dictionary<Item, float>();

    public Inventory(float mWeight) 
    {
        m_maxWeight = mWeight;
        m_currentWeight = 0f;
    }

    public bool Contains(Item item) 
    {
        return (m_items[item] != 0f);
    }
    
    public void Add(Inventory inv) 
    {
        foreach (KeyValuePair<Item, float> item in inv.m_items) 
        {
            if (!Add(item.Key, item.Value)) 
            {
                return; //Stop adding together the inventories if there is no more space
            }
        }
    }

    public bool Add(Item item, float weight) 
    {
        if ((m_currentWeight + weight) > m_maxWeight) 
        {
            return false;
        }

        if (m_items.ContainsKey(item)) 
        {
            m_items[item] += weight;
            m_currentWeight += weight;
        } 
        else 
        {
            m_items.Add(item, weight);
            m_currentWeight += weight;
        }

        return true;
    }

    public bool Remove(Item item, float weight) 
    {
        if (!m_items.ContainsKey(item) || m_items[item] < weight) // If it doesn't have that resource then nothing happens
        {
            return false;
        }

        m_items[item] -= weight;
        m_currentWeight -= weight;
        
        return true;
    }

    public float RemoveAll(Item item)
    {   
        if(!m_items.ContainsKey(item)) 
        {
            return 0f;
        }

        float itemWeight = m_items[item];

        Remove(item, itemWeight);

        return itemWeight;
    }

    public float RemoveFood(float weight) 
    {
        float weightRemaining = weight;

        if (!Remove(Item.Fruit, weightRemaining))
        {
            weightRemaining -= RemoveAll(Item.Fruit);

            Remove(Item.Meat, weightRemaining);
        }

        float meatRatio = weightRemaining/weight;

        return meatRatio;
    }

    public void AddFood(float weight, float meatRatio) 
    {
        Add(Item.Fruit, weight * meatRatio);

        Add(Item.Meat, weight * (1 - meatRatio));
    }

    public float GetFoodWeight() 
    {
        return GetWeight(Item.Meat) + GetWeight(Item.Fruit);
    }

    public float GetMaterialWeight() 
    {
        return GetWeight(Item.Stone) + GetWeight(Item.Wood);
    }

    public void DumpMaterial(float woodWeight, float stoneWeight) //Assuming it is being dumped into an endless container
    {
        //Mainly here to avoid having to call GetComponent from the human state classes
        Add(Item.Wood, woodWeight);
        Add(Item.Stone, stoneWeight);
    }

    public float GetWeight(Item item) 
    {
        if (m_items.ContainsKey(item)) {
            return m_items[item];
        } 
        else 
        {
            return 0f;
        }
    }
}