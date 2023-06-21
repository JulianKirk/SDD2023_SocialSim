using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject SimMenu;

    public void OpenOptionsMenu() 
    {
        OptionsMenu.SetActive(true);

        gameObject.SetActive(false);
    }

    public void OpenSimMenu() 
    {
        SimMenu.SetActive(true);

        gameObject.SetActive(false);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }
}
