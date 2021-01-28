using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public Transform playerSpawn;
    public GameObject player;
    public GameObject playerCamera;
    public bool win = false;
    public bool loose = false;
    public int coins;

    [Header("IA")]
    public Transform enemySpawn;
    public GameObject enemyPref;
    public GameObject enemy;

    [Header("Start Panel")]
    public GameObject canvas;
    public GameObject[] checkUIOver;
    public GameObject[] infiniteButtons = new GameObject[2];
    public GameObject shopMenu;
    public GameObject startMenu;

    [Header("Restart Panel")]
    public int raceCoinGet = 0;
    public GameObject loosePanel;
    public TextMeshProUGUI txtCoinsGet;
    public TextMeshProUGUI txtEndInfos;

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
        if (Application.isEditor)
        {
            PlayerPrefs.SetInt("level", 0);
            /*
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("level", 0);
            PlayerPrefs.SetInt("level", 10);
            */
        }
    }

    private void Start()
    {
        coins = PlayerPrefs.GetInt("coins");
        level = PlayerPrefs.GetInt("level");
        mg = MapGeneration.instance;
        coinsTextField.text = coins.ToString();
        
        GoToMainMenu();
    }

    /* Launch the game
     */
    public void StartGame()
    {
        started = true;
        raceCoinGet = 0;
        
        startMenu.SetActive(false);
    }

    /* Show loosed menu
     */
    public void PlayerDeath()
    {
        loose = true;
        loosePanel.SetActive(true);
        nextPanel.SetActive(false);
        txtCoinsGet.text = "+ " + raceCoinGet.ToString();
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();

        if (infinite)
        {
            if((int)player.transform.position.z > PlayerPrefs.GetInt("bestRace"))
            {
                PlayerPrefs.SetInt("bestRace", (int)player.transform.position.z);
                txtEndInfos.text = "New best score : " + ((int)player.transform.position.z).ToString();
            }
            else
            {
                txtEndInfos.text = "Actual best score : " + PlayerPrefs.GetInt("bestRace").ToString();
            }
        }
        else
        {
            txtEndInfos.text = "Unlucky you loose ! Try again !";
        }
    }

    /* Close all panel except menu
     */
    public void GoToMainMenu()
    {
        loosePanel.SetActive(false);
        nextPanel.SetActive(false);
        shopMenu.SetActive(false);

        player.transform.position = playerSpawn.transform.position;
        player.transform.rotation = playerSpawn.transform.rotation;
        player.GetComponent<PlayerMovements>()._init_();

        started = false;

        startMenu.SetActive(true);

        SetMode();
    }

    /* Change level value
     */
    public void NextLevel()
    {
        if (mg.levels.Count - 1 > level)
        {
            level += 1;
            PlayerPrefs.SetInt("level", level);
            PlayerPrefs.Save();
        }
    }

    /* Show panel of win
     */
    public void Win()
    {
        win = true;
        nextPanel.SetActive(true);
        txtNextCoinsGet.text = "+ " + raceCoinGet.ToString();
        txtEndInfos.text = "Good job ! You won !";
        NextLevel();
    }

    /* Return actual level
     */
    public int GetLevel()
    {
        return PlayerPrefs.GetInt("level");
    }


    /* Add coin
     */
    public void GetCoin(int amount = 1)
    {
        //player.GetComponent<PlayerMovements>().GetCoins();
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
        GoToMainMenu();
    }

    /* Add coin by looking an ad
     */
    public void GetDoubleRaceCoins()
    {
        coins += raceCoinGet;
        coinsTextField.text = coins.ToString();
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
        GoToMainMenu();
    }

    public bool Buy(int amount)
    {
        if(coins >= amount)
        {
            coins -= amount;
            coinsTextField.text = coins.ToString();
            PlayerPrefs.SetInt("coins", coins);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    /* Check if mouse is over ui or not
     * Used to Start the game
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

    /* Spawn AI if it's a normal level and set of panels
     */
    void SetMode()
    {
        Destroy(enemy);
        if (PlayerPrefs.GetInt("mode") == 0)
        {
            infinite = false;
            infiniteButtons[0].SetActive(true);
            infiniteButtons[1].SetActive(false);
            mg.GenLevel();
            enemy = Instantiate(enemyPref);
            enemy.transform.position = enemySpawn.transform.position;
        }
        else
        {
            infinite = true;
            infiniteButtons[0].SetActive(false);
            infiniteButtons[1].SetActive(true);
            mg.GenRandomLevel();
        }
    }

    /* Change the mode and saved it
     */
    public void ChangeMode(string mode)
    {
        if(mode == "infinite")
        {
            PlayerPrefs.SetInt("mode", 1);
        }
        else
        {
            PlayerPrefs.SetInt("mode", 0);
        }
        SetMode();
    }
}
