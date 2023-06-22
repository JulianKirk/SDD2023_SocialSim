using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsData
{
    public int totalYears;

    public int totalPopulation; //Over the whole simulation
    public Dictionary<int, int> populationByYear;

    public int totalHousesBuilt;
    public Dictionary<int, int> housesBuiltByYear;
    public Dictionary<int, int> housesByYear;

    public int totalDeaths;
    public Dictionary<int, int> deathsByYear;
    public Dictionary<string, int> deathsByCauses;

    public int totalBirths;
    public Dictionary<int, int> birthsByYear;

    //With "t" indicating a total, "y" indicating by years 
    public ResultsData(int t_years, int t_population, Dictionary<int, int> y_population, 
        int t_housesBuilt, Dictionary<int, int> y_housesBuilt, Dictionary<int, int> y_houses, 
        int t_deaths, Dictionary<int, int> y_deaths, Dictionary<string, int> causes_deaths,
        int t_births, Dictionary<int, int> y_births)
    {
        totalYears = t_years;

        totalPopulation = t_population;
        populationByYear = y_population;

        totalHousesBuilt = t_housesBuilt;
        housesBuiltByYear = y_housesBuilt;
        housesByYear = y_houses;

        totalDeaths = t_deaths;
        deathsByYear = y_deaths;
        deathsByCauses = causes_deaths;

        totalBirths = t_births;
        birthsByYear = y_births;
    }

    public ResultsData() 
    {
        totalYears = 0;

        totalPopulation = 0;
        populationByYear = new Dictionary<int, int>();

        totalHousesBuilt = 0;
        housesBuiltByYear = new Dictionary<int, int>();
        housesByYear = new Dictionary<int, int>();

        totalDeaths = 0;
        deathsByYear = new Dictionary<int, int>();
        deathsByCauses = new Dictionary<string, int>();

        totalBirths = 0;
        birthsByYear = new Dictionary<int, int>();
    }
}
