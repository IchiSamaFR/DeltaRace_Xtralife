using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CotcSdk;

using static Tools;

public class TrophyManager : MonoBehaviour
{
    public static TrophyManager instance;

    List<Achievement> achievements = new List<Achievement>();

    Gamer Gamer;

    void Awake()
    {
        if (instance)
        {
            Debug.LogError("Connection Manager already defined.");
            return;
        }
        instance = this;
    }
    

    void Start()
    {

    }

    public void SetGamer(Gamer gamer)
    {
        Gamer = gamer;
        ListAchievements();
    }

    void ListAchievements()
    {
        Gamer.Achievements.Domain("private").List().Done(listAchievementsRes => {
            foreach (var achievement in listAchievementsRes)
            {
                achievements.Add(new Achievement(achievement.Key, 
                                                GetJsonAchievementConfig(achievement.Value.Config.ToString()),
                                                achievement.Value.Progress));
            }
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not list achievements: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void VerifyAchievement(string key, float value)
    {
        foreach (var item in achievements)
        {
            if (item.unit == key && item.progress < 1)
            {
                float maxValue = float.Parse(item.maxValue);

                print(item.maxValue + " " + value);
                if (maxValue <= value)
                {
                    string msg = "";

                    if (item.unit == "distance")
                        msg = "Traveled : " + item.maxValue + " ! ";
                    else if (item.unit == "buy")
                        msg = "Cosmetics purchased : " + item.maxValue + " ! ";
                    else if (item.unit == "level")
                        msg = "Level passed : " + item.maxValue + " ! ";

                    UINotificationManager.instance.CreateNotification("trophy", msg);

                    SetAchievementProgress(item.key, 1);
                }
                else if (item.progress <= value / maxValue)
                {
                    SetAchievementProgress(item.key, value / maxValue);
                }
            }
        }
    }

    void SetAchievementProgress(string achivement, float progress)
    {
        /*
        Bundle value = new Bundle(progress);
        Gamer.Achievements.Domain("private").AssociateData(achivement, Bundle.CreateObject("progress", value))
            .Done(setAchievementDataRes => {
                Debug.Log("Custom information" + setAchievementDataRes.GamerData);
                print("SET UP ACHIEVEMENT " + achivement + " " + progress);
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set custom information: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
        */
    }
}


public class Achievement
{
    public string key;
    public string unit;
    public string maxValue;
    public float progress;

    public Achievement(string key, JsonAchievementConfig config, float progress)
    {
        this.key = key;
        unit = config.unit;
        maxValue = config.maxValue;
        this.progress = progress;
    }
}