using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static ProfileData GetProfileClass(string json)
    {
        return JsonUtility.FromJson<ProfileData>(json);
    }
    public static ProfileData GetCosmetics(string json)
    {
        return JsonUtility.FromJson<ProfileData>(json);
    }
    public static JsonError GetJsonError(string json)
    {
        return JsonUtility.FromJson<JsonError>(json);
    }
    public static JsonPlane GetJsonPlane(string json)
    {
        return JsonUtility.FromJson<JsonPlane>(json);
    }
    public static JsonCharacter GetJsonCharacters(string json)
    {
        return JsonUtility.FromJson<JsonCharacter>(json);
    }
    public static JsonCoin GetJsonCoin(string json)
    {
        return JsonUtility.FromJson<JsonCoin>(json);
    }
    public static JsonAchievementConfig GetJsonAchievementConfig(string json)
    {
        return JsonUtility.FromJson<JsonAchievementConfig>(json);
    }
    public static JsonLevel GetJsonLevel(string json)
    {
        return JsonUtility.FromJson<JsonLevel>(json);
    }

    public static dynamic GetJson(string json)
    {
        return JsonUtility.FromJson<dynamic>(json);
    }
}

public class JsonError
{
    public string name;
    public string message;
}
public class JsonPlane
{
    public string deltaplanes;
}
public class JsonCharacter
{
    public string characters;
}
public class JsonCoin
{
    public string coins;
}
public class JsonAchievementConfig
{
    public string unit;
    public string maxValue;
}
public class JsonLevel
{
    public string level;
}
