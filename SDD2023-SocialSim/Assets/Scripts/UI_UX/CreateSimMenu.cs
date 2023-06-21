using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateSimMenu : MonoBehaviour
{
    string savePath;

    public Slider IntelligenceSlider;
    public Slider StrengthSlider;
    public Slider HumanNumberSlider;
    public Slider MapSizeSlider;

    public GameObject mainMenu;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/simData.json";
    }

    public void StartSimulation() //Added to the OnValueChange events on the sliders in the editor
    {
        SimData dataToSave = new SimData(HumanNumberSlider.value, (int)MapSizeSlider.value, IntelligenceSlider.value, StrengthSlider.value);

        string jsonText = JsonConvert.SerializeObject(dataToSave);

        File.WriteAllText(savePath, jsonText);

        SceneManager.LoadScene(1);
    }

    public void ExitMenu() 
    {
        mainMenu.SetActive(true);        
        gameObject.SetActive(false);        
    }
}
