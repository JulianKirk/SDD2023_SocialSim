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
        if (m_items[item] < weight) 
        {
            return false;
        }

        m_items[item] -= weight;
        m_currentWeight -= weight;
        
        return true;
    }

    public float GetWeight(Item item) 
    {
        return m_items[item];
    }
}