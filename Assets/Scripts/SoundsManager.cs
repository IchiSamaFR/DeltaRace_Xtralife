using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    public GameObject[] sounds;
    
    public static bool active;

    private void Awake()
    {
        int var = PlayerPrefs.GetInt("sounds_active");

        if(var == 1)
        {
            active = true;
        }
        else
        {
            active = false;
        }

        instance = this;
        Refresh();
    }

    public void Set(bool val)
    {
        active = val;

        if (val)
        {
            PlayerPrefs.SetInt("sounds_active", 1);
        }
        else
        {
            PlayerPrefs.SetInt("sounds_active", 0);
        }

        Refresh();
    }

    public void Change()
    {
        Set(!active);
    }

    void Refresh()
    {
        if (active)
        {
            sounds[0].SetActive(true);
            sounds[1].SetActive(false);
        }
        else
        {
            sounds[0].SetActive(false);
            sounds[1].SetActive(true);
        }
    }
}
