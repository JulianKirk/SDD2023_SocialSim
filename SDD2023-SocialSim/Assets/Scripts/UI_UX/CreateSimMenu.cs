using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateSimMenu : MonoBehaviour
{
    string savePath;

    public Slider IntelligenceSlider;
    public Slider StrengthSlider;
    public Slider HumanNumberSlider;
    public Slider MapSizeSlider;

    public TMP_Text IntelligenceText;
    public TMP_Text StrengthText;
    public TMP_Text HumanNumberText;
    public TMP_Text MapSizeText;

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

    public void UpdateHumanNumberText() 
    {
        HumanNumberText.text = ((int)HumanNumberSlider.value).ToString();
    }

    public void UpdateIntelligenceText() 
    {
        IntelligenceText.text = ((int)IntelligenceSlider.value).ToString();
    }

    public void UpdateStrengthText() 
    {
        StrengthText.text = ((int)StrengthSlider.value).ToString();
    }

    public void UpdateMapSizeText() 
    {
        MapSizeText.text = ((int)MapSizeSlider.value).ToString();
    }

    public void ExitMenu() 
    {
        mainMenu.SetActive(true);        
        gameObject.SetActive(false);        
    }
}
