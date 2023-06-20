using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    private float m_health = 300f;

    public Inventory m_inventory = new Inventory(1000f); //Total storage capacity of 1000f

    public void TakeDamage(float damage) //Animals or opposing factions may be able to destroy houses
    {
        m_health -= damage;

        if (m_health <= 0) 
        {
            Die();
        }
    }

    void Die() 
    {
        Destroy(gameObject);
    }
}