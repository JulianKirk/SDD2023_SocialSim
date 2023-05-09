using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenericSingleton : GenericSingleton<TestGenericSingleton>
{
    public int test = 1;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start method still works");
    }

    public void Shout() 
    {
        Debug.Log("Rah!! Generic singleton is here");
        Debug.Log(test);
    }
}
