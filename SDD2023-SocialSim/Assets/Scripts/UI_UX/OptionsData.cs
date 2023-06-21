using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsData
{
    public float masterVolume;
    public float panSensitivity;

    public OptionsData(float volume, float sense) 
    {
        masterVolume = volume;

        panSensitivity = sense;
    }
}
