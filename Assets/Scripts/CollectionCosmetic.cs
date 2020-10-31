using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionCosmetic : MonoBehaviour
{
    GameMaster gm;
    public static CollectionCosmetic instance;
    [System.Serializable]
    public class plane
    {
        public string name;
        public GameObject pref;
        public GameObject unavailable;
        public int value;
    }
    [System.Serializable]
    public class character
    {
        public string name;
        public GameObject pref;
        public GameObject unavailable;
        public int value;
    }

    public List<plane> planes = new List<plane>();
    public List<character> characters = new List<character>();

    [Header("Buttons UI")]
    public List<Image> buttons = new List<Image>();
    public List<GameObject> panels = new List<GameObject>();
    public List<GameObject> menus = new List<GameObject>();

    public DeltaCosmetic playerShop;
    public GameObject shopCamera;
    bool OnShop = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gm = GameMaster.instance;
        _init_();

        shopCamera.SetActive(OnShop);
        gm.playerCamera.SetActive(!OnShop);

        RefreshShop();
    }

    void _init_()
    {
        PlayerPrefs.SetInt("characters_grey", 1);
        PlayerPrefs.SetInt("planes_grey", 1);

        string aPl = PlayerPrefs.GetString("actual_plane");
        if (aPl != "")
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangePlane(aPl);
            playerShop.ChangePlane(aPl);
        }
        else
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangePlane("grey");
            playerShop.ChangePlane("grey");
            PlayerPrefs.SetString("actual_plane", "grey");
        }

        string aCh = PlayerPrefs.GetString("actual_character");
        if (aCh != "")
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangeCharacter(aCh);
            playerShop.ChangeCharacter(aCh);
        }
        else
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangeCharacter("grey");
            playerShop.ChangeCharacter("grey");
            PlayerPrefs.SetString("actual_character", "grey");
        }
    }
    
    void RefreshShop()
    {
        foreach (plane p in planes)
        {
            if (!p.unavailable)
            {
                return;
            }
            if (PlayerPrefs.GetInt("planes_" + p.name) > 0)
            {
                p.unavailable.SetActive(false);
            }
            else
            {
                p.unavailable.SetActive(true);
                p.unavailable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = p.value.ToString();
            }
        }
        
        foreach (character c in characters)
        {
            if (!c.unavailable)
            {
                return;
            }
            if (PlayerPrefs.GetInt("characters_" + c.name) > 0)
            {
                c.unavailable.SetActive(false);
            }
            else
            {
                c.unavailable.SetActive(true);
                c.unavailable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = c.value.ToString();
            }
        }
    }

    public GameObject GetPlane(string name)
    {
        if(name == "random")
        {
            int index = Random.Range(0, planes.Count);
            return planes[index].pref;
        }

        foreach(plane p in planes)
        {
            if(p.name == name)
            {
                return p.pref;
            }
        }
        return planes[0].pref;
    }
    public int GetPlanePrice(string name)
    {
        foreach (plane p in planes)
        {
            if (p.name == name)
            {
                return p.value;
            }
        }
        return -1;
    }

    public GameObject GetCharacter(string name)
    {
        if (name == "random")
        {
            int index = Random.Range(0, characters.Count);
            return characters[index].pref;
        }

        foreach (character c in characters)
        {
            if (c.name == name)
            {
                return c.pref;
            }
        }
        return characters[0].pref;
    }
    public int GetCharacterPrice(string name)
    {
        foreach (character c in characters)
        {
            if (c.name == name)
            {
                return c.value;
            }
        }
        return -1;
    }



    public void PlanesAction(string name)
    {
        if (PlayerPrefs.GetInt("planes_" + name) > 0)
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangePlane(name);
            playerShop.ChangePlane(name);
            PlayerPrefs.SetString("actual_plane", name);
            PlayerPrefs.Save();
        }
        else
        {
            int price = GetPlanePrice(name);
            if(price > -1 && gm.Buy(price))
            {
                PlayerPrefs.SetInt("planes_" + name, 1);
                PlayerPrefs.SetString("actual_plane", name);
                gm.player.GetComponent<DeltaCosmetic>().ChangePlane(name);
                playerShop.ChangePlane(name);
                PlayerPrefs.Save();
                RefreshShop();
            }
        }
        print(name);
    }

    public void CharacterAction(string name)
    {
        if (PlayerPrefs.GetInt("characters_" + name) > 0)
        {
            gm.player.GetComponent<DeltaCosmetic>().ChangeCharacter(name);
            playerShop.ChangeCharacter(name);
            PlayerPrefs.SetString("actual_character", name);
            PlayerPrefs.Save();
        }
        else
        {
            int price = GetCharacterPrice(name);
            if (price > -1 && gm.Buy(price))
            {
                PlayerPrefs.SetInt("characters_" + name, 1);
                gm.player.GetComponent<DeltaCosmetic>().ChangeCharacter(name);
                playerShop.ChangeCharacter(name);
                PlayerPrefs.SetString("actual_character", name);
                PlayerPrefs.Save();
                RefreshShop();
            }
        }
    }

    public void SetPanel(string type)
    {
        if(type == "plane")
        {
            buttons[0].color = new Color(0.4f, 0.4f, 0.4f);
            buttons[1].color = new Color(1, 1, 1);
            panels[0].SetActive(true);
            panels[1].SetActive(false);
        }
        else
        {
            buttons[0].color = new Color(1, 1, 1);
            buttons[1].color = new Color(0.4f, 0.4f, 0.4f);
            panels[0].SetActive(false);
            panels[1].SetActive(true);
        }
    }

    public void Open()
    {
        if (OnShop)
        {
            menus[0].SetActive(true);
            menus[1].SetActive(false);
            OnShop = false;
        }
        else
        {
            menus[0].SetActive(false);
            menus[1].SetActive(true);
            SetPanel("plane");
            OnShop = true;
        }
        shopCamera.SetActive(OnShop);
        gm.playerCamera.SetActive(!OnShop);
    }
}
