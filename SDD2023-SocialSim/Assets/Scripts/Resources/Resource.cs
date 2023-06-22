using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Item ResourceType;

    public Inventory m_inventory;

    void Start()
    {
        ResourceColliderManager.instance.RegisterInventory(gameObject.GetComponent<Collider2D>(), this);
    }

    void Update()
    {
        if (m_inventory.m_currentWeight <= 0) 
        {
            Die();
        }
    }

    protected void Die() 
    {
        //Maybe play a dying animation and sound
        Destroy(gameObject);
    }
}