using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsMenu : MonoBehaviour
{
    string savePath;
    ResultsData simResults;

    public TMP_Text totalYears;
    public TMP_Text totalPopulation;
    public TMP_Text totalHousesBuilt;
    public TMP_Text totalDeaths;
    public TMP_Text totalBirths;

    // Displayed as a lit of items
    public TMP_Text populationByYear;

    public TMP_Text housesBuiltByYear;
    public TMP_Text housesByYear;
    
    public TMP_Text deathsByYear;
    public TMP_Text deathsByCauses;

    public TMP_Text birthsByYear;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/simResults.json";

        string jsonText = File.ReadAllText(savePath);

        simResults = JsonConvert.DeserializeObject<ResultsData>(jsonText);

        // Update the single number stats
        totalYears.text += simResults.totalYears.ToString();
        totalPopulation.text += simResults.totalPopulation.ToString();
        totalHousesBuilt.text += simResults.totalHousesBuilt.ToString();
        totalDeaths.text += simResults.totalDeaths.ToString();
        totalBirths.text += simResults.totalBirths.ToString();

        // Update the lists
        deathsByYear.text = "";
        foreach (KeyValuePair<int, int> pair in simResults.deathsByYear) 
        {
            deathsByYear.text += "- Year " + pair.Key + ": " + pair.Value + "\n";
        }
        
        deathsByCauses.text = "";
        foreach (KeyValuePair<string, int> pair in simResults.deathsByCauses) 
        {
            deathsByCauses.text += "- " + pair.Key + ": " + pair.Value + "\n";
        }

        birthsByYear.text = "";
        foreach (KeyValuePair<int, int> pair in simResults.birthsByYear) 
        {
            birthsByYear.text += "- Year " + pair.Key + ": " + pair.Value + "\n";
        }

        housesBuiltByYear.text = "";
        foreach (KeyValuePair<int, int> pair in simResults.housesBuiltByYear) 
        {
            housesBuiltByYear.text += "- Year " + pair.Key + ": " + pair.Value + "\n";
        }

        housesByYear.text = "";
        foreach (KeyValuePair<int, int> pair in simResults.housesByYear) 
        {
            housesByYear.text += "- Year " + pair.Key + ": " + pair.Value + "\n";
        }

        populationByYear.text = "";
        foreach (KeyValuePair<int, int> pair in simResults.populationByYear) 
        {
            populationByYear.text += "- Year " + pair.Key + ": " + pair.Value + "\n";
        }

    }

    public void ExportFile() //Ad as a button if I have time
    {
        // File.WriteAllText(savepath, )
    }

    public void BackToMainMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
