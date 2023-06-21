using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    string savePath;

    public Slider MasterVolume;
    public Slider PanSensitivity;

    public AudioMixer MainMixer;

    public GameObject mainMenu;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/options.json";
        
        if (!File.Exists(savePath)) 
        {
            string jsonText = JsonConvert.SerializeObject(new OptionsData(0.5f, 0.5f));

            File.WriteAllText(savePath, jsonText);

            MainMixer.SetFloat("Master", (0.5f * 80) - 80);
        } 
        else //Load previously saved data 
        {
            string jsonText = File.ReadAllText(savePath);

            OptionsData optionsData = JsonConvert.DeserializeObject<OptionsData>(jsonText);

            MasterVolume.value = optionsData.masterVolume;
            PanSensitivity.value = optionsData.panSensitivity;

            MainMixer.SetFloat("Master", (optionsData.masterVolume * 80) - 80);
        }
    }

    public void WriteOptions() //Added to the OnValueChange events on the sliders in the editor
    {
        OptionsData dataToSave = new OptionsData(MasterVolume.value, PanSensitivity.value);

        string jsonText = JsonConvert.SerializeObject(dataToSave);

        File.WriteAllText(savePath, jsonText);

        MainMixer.SetFloat("Master", (dataToSave.masterVolume * 80) - 80);
    }

    public void ExitMenu() 
    {
        mainMenu.SetActive(true);        
        gameObject.SetActive(false);        
    }
}
