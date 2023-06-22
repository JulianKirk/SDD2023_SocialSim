using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceColliderManager
{
    private static readonly Lazy<ResourceColliderManager> _instance = new Lazy<ResourceColliderManager>(() => new ResourceColliderManager());

    public static ResourceColliderManager instance { get { return _instance.Value; } }

    private Dictionary<Collider2D, Resource> m_collection = new Dictionary<Collider2D, Resource>();

    public Resource GetInventory(Collider2D collider) 
    {
        if (m_collection.ContainsKey(collider)) 
        {
            return m_collection[collider];
        } 
        else 
        {
            return null;
        }
    }

    public void RegisterInventory(Collider2D collider, Resource rec) 
    {
        if(!m_collection.ContainsKey(collider)){
            m_collection.Add(collider, rec);
        }
    }
}