using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryColliderManager
{
    private static readonly Lazy<InventoryColliderManager> _instance = new Lazy<InventoryColliderManager>(() => new InventoryColliderManager());

    public static InventoryColliderManager instance { get { return _instance.Value; } }

    public Dictionary<Collider2D, Inventory> Collection = new Dictionary<Collider2D, Inventory>();
}