using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance; 
    public Transform playerSpawn;
    public GameObject player;
    public int coins;

    [Header("Start Panel")]
    public GameObject canvas;
    public GameObject[] startToHide;
    public GameObject[] startToShow;
    public TextMeshProUGUI levelText;
    public GameObject[] checkUIOver;
    public GameObject[] infiniteButtons = new GameObject[2];

    [Header("Restart Panel")]
    public int raceCoinGet = 0;
    public GameObject loosePanel;
    public TextMeshProUGUI txtCoinsGet;

    [Header("Restart Panel")]
    public GameObject nextPanel;
    public TextMeshProUGUI txtNextCoinsGet;

    [Header("Parameters")]
    public int level;
    public bool started = false;
    public bool infinite = false;

    [Header("UI")]
    public TextMeshProUGUI coinsTextField;


    MapGeneration mg;

    private void Awake()
    {
        instance = this;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("level", 2);
    }

    private void Start()
    {
        coins = PlayerPrefs.GetInt("coins");
        level = PlayerPrefs.GetInt("level");
        mg = MapGeneration.instance;
        coinsTextField.text = coins.ToString();

        GoToMainMenu();
    }

    private void Update()
    {
        IsOverUI();
    }

    public void StartGame()
    {
        started = true;
        raceCoinGet = 0;

        foreach (GameObject obj in startToHide)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in startToShow)
        {
            obj.SetActive(true);
        }
        levelText.text = level.ToString();

    }

    public void PlayerDeath()
    {
        loosePanel.SetActive(true);
        nextPanel.SetActive(false);
        txtCoinsGet.text = "+ " + raceCoinGet.ToString();
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
    }

    public void GoToMainMenu()
    {
        loosePanel.SetActive(false);
        nextPanel.SetActive(false);

        player.transform.position = playerSpawn.transform.position;
        player.transform.rotation = playerSpawn.transform.rotation;
        player.GetComponent<PlayerMovements>()._init_();

        started = false;

        foreach (GameObject obj in startToHide)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in startToShow)
        {
            obj.SetActive(false);
        }

        if (infinite)
        {
            infiniteButtons[0].SetActive(false);
            infiniteButtons[1].SetActive(true);
        }

        mg.GenLevel();
    }

    public void NextLevel()
    {
        if (mg.levels.Count - 1 > level)
        {
            level += 1;
            PlayerPrefs.SetInt("level", level);
            PlayerPrefs.Save();
        }
    }

    public void Win()
    {
        nextPanel.SetActive(true);
        txtNextCoinsGet.text = "+ " + raceCoinGet.ToString();
        NextLevel();
    }

    public int GetLevel()
    {
        return PlayerPrefs.GetInt("level");
    }


    /* Add coin
     */
    public void GetCoin(int amount = 1)
    {
        coins += amount;
        raceCoinGet += amount;
        coinsTextField.text = coins.ToString();

        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
    }

    /* Add coin by looking an ad
     */
    public void GetAdsCoins(int amount)
    {
        coins += amount;
        coinsTextField.text = coins.ToString();
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
    }

    /* Add coin by looking an ad
     */
    public void GetDoubleRaceCoins()
    {
        coins += raceCoinGet;
        coinsTextField.text = coins.ToString();
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
    }

    /* Check if mouse is over ui or not
     * 
     * Usefull to Start the game
     */
    public bool IsOverUI()
    {
        float _mouseX = Input.mousePosition.x;
        float _mouseY = Input.mousePosition.y;
        
        foreach (GameObject obj in checkUIOver)
        {
            RectTransform _transf = obj.GetComponent<RectTransform>();

            float _xMin = _transf.anchorMax.x * Screen.width;
            float _xMax = _transf.anchorMin.x * Screen.width;

            float _yMin = _transf.anchorMax.y * Screen.height;
            float _yMax = _transf.anchorMin.y * Screen.height;

            if (obj.activeSelf
                && _xMin > _mouseX && _xMax < _mouseX
                && _yMin > _mouseY && _yMax < _mouseY)
            {
                return true;
            }
        }

        return false;
    }
}
