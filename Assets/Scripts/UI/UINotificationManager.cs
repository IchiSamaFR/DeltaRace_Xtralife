using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINotificationManager : MonoBehaviour
{
    public static UINotificationManager instance;

    [System.Serializable]
    public struct Notifications
    {
        public string id;
        public GameObject prefab;
    }
    
    public List<Notifications> notifications = new List<Notifications>();

    void Awake()
    {
        instance = this;
    }

    GameObject GetNotification(string id)
    {
        foreach (var item in notifications)
        {
            if(item.id == id)
            {
                return item.prefab;
            }
        }
        return null;
    }

    public void CreateNotification(string id, string text)
    {
        GameObject obj = Instantiate(GetNotification(id), transform);

        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
    }
}
