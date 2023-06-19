using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
// should be abstract
public abstract class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static readonly Lazy<T> lazyInstance = new Lazy<T>(CreateObject);

    public static T instance => lazyInstance.Value;

    private static T CreateObject() {
        GameObject owner = new GameObject(typeof(T).FullName);
        T instance = owner.AddComponent<T>();
        // DontDestroyOnLoad(owner);
        return instance;
    }
}
