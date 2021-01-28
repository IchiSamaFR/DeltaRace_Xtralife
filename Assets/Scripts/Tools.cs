using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static ProfileData GetProfileClass(string json)
    {
        return JsonUtility.FromJson<ProfileData>(json);
    }
}
