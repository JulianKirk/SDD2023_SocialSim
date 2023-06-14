using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Inventory m_inventory;

    void Update()
    {
        if (m_inventory.m_currentWeight <= 0) 
        {
            Die();
        }
    }

    void Die() 
    {
        //Maybe play a dying animation and sound
        Destroy(gameObject);
    }
}