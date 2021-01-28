using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapGeneration : MonoBehaviour
{
    public static MapGeneration instance;
    GameMaster gm;
    int currentLevel = 0;
    [SerializeField]
    public List<MapModel> levels = new List<MapModel>();
    [SerializeField]
    public List<TilePrefab> tiles = new List<TilePrefab>();
    [SerializeField]
    public List<TilePrefab> tilesOther = new List<TilePrefab>();
    public GameObject tilesContent;
    public GameObject otherContent;

    /********** Infinite Mode **********/
    [Header("Random")]
    [SerializeField]
    public List<MapModel> randomPref = new List<MapModel>();
    /********** Infinite Mode **********/

    [Header("UI")]
    public Image distance;
    public GameObject distanceBar;
    public TextMeshProUGUI distanceInfinite;
    public GameObject distanceInfiniteBar;
    public TextMeshProUGUI levelText;
    float arrival;
    Transform player;


    /********** Infinite Mode **********/
    string randomGenTiles = "";
    string randomGenOther = "";
    int alreadyGen = 0;
    class structGen
    {
        public int pos;
        public GameObject obj;
        public structGen(int _pos, GameObject _obj)
        {
            pos = _pos;
            obj = _obj;
        }
    }
    List<structGen> mapGenerated = new List<structGen>();
    /********** Infinite Mode **********/

    List<CliffPath> cliffPaths = new List<CliffPath>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gm = GameMaster.instance;
        _init_();
        player = gm.player.transform;
    }

    private void Update()
    {
        if (gm.started && !gm.infinite)
        {
            if (!distanceBar.activeSelf)
            {
                distanceBar.SetActive(true);
            }
            float _zPos = player.position.z;
            distance.fillAmount = _zPos / arrival;
        }
        else
        {
            if (distanceBar.activeSelf)
            {
                distanceBar.SetActive(false);
            }
        }

        if(gm.started && gm.infinite)
        {
            ContinueGenLevel();
            if (!distanceInfiniteBar.activeSelf)
            {
                distanceInfiniteBar.SetActive(true);
            }
            if(player.position.z < 10000)
            {
                distanceInfinite.text = ((int)player.position.z).ToString();
            }
            else
            {
                distanceInfinite.text = ((int)player.position.z / 1000).ToString() + "k";
            }
        }
        else
        {
            if (distanceInfiniteBar.activeSelf)
            {
                distanceInfiniteBar.SetActive(false);
            }
        }
    }

    void _init_()
    {
        GenLevel();
    }

    /* Gen level by string level
     */
    public void GenLevel()
    {
        currentLevel = gm.GetLevel();
        int _currentLevel = 0;
        if (levels.Count - 1 < currentLevel)
        {
            _currentLevel = levels.Count - 1;
        }
        else
        {
            _currentLevel = currentLevel;
        }

        cliffPaths = new List<CliffPath>();

        foreach (Transform child in tilesContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in otherContent.transform)
        {
            Destroy(child.gameObject);
        }
        /****      MAP         ****/
        int _count = 0;
        foreach (char c in levels[_currentLevel].wallTiles)
        {
            if (c != '.')
            {
                foreach (TilePrefab tile in tiles)
                {
                    if (tile.name == c)
                    {
                        GameObject obj = Instantiate(tile.prefab, tilesContent.transform);
                        obj.transform.position = new Vector3(-2f, -0.5f, _count * 10);
                        if (c == 'E')
                        {
                            arrival = _count * 10;
                        }

                        if (obj.GetComponent<CliffPath>())
                        {
                            cliffPaths.Add(obj.GetComponent<CliffPath>());
                        }
                        break;
                    }
                }
            }
            _count += 1;
        }
        /****      OTHER TILES         ****/
        int _countOther = 0;
        foreach (char c in levels[_currentLevel].otherTiles)
        {
            if (c != '.')
            {
                foreach (TilePrefab tile in tilesOther)
                {
                    if (tile.name == c)
                    {
                        GameObject obj = Instantiate(tile.prefab, otherContent.transform);
                        obj.transform.position = new Vector3(0, 0, _countOther * 10);
                        break;
                    }
                }
            }
            _countOther += 1;
        }
        levelText.text = _currentLevel.ToString();
    }
    
    /* Reset of the map and set of the infinite start map
     */
    public void GenRandomLevel()
    {
        foreach (Transform child in tilesContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in otherContent.transform)
        {
            Destroy(child.gameObject);
        }

        randomGenTiles = "S..";
        randomGenOther = "...";
        alreadyGen = 0;
        ContinueGenLevel();
    }

    /* Gen map by random int who pick a string prefab in a list
     */
    void ContinueGenLevel()
    {
        while (player.position.z / 10 > randomGenTiles.Length - 14)
        {
            int rdm = Random.Range(0, randomPref.Count);
            randomGenTiles += randomPref[rdm].wallTiles;
            randomGenOther += randomPref[rdm].otherTiles;
        }
        int _count = 0;
        foreach (char c in randomGenTiles)
        {
            if (_count >= alreadyGen && c != '.')
            {
                foreach (TilePrefab tile in tiles)
                {
                    if (tile.name == c)
                    {
                        GameObject obj = Instantiate(tile.prefab, tilesContent.transform);
                        obj.transform.position = new Vector3(-2f, -0.5f, _count * 10);
                        mapGenerated.Add(new structGen(_count, obj));
                        break;
                    }
                }
            }
            _count += 1;
        }
        int _countOther = 0;
        foreach (char c in randomGenOther)
        {
            if (_countOther >= alreadyGen && c != '.')
            {
                foreach (TilePrefab tile in tilesOther)
                {
                    if (tile.name == c)
                    {
                        GameObject obj = Instantiate(tile.prefab, otherContent.transform);
                        obj.transform.position = new Vector3(0, 0, _countOther * 10);
                        mapGenerated.Add(new structGen(_countOther, obj));
                        break;
                    }
                }
            }
            _countOther += 1;
        }
        
        alreadyGen = randomGenTiles.Length;

        List<structGen> toDelete = new List<structGen>();
        foreach(structGen obj in mapGenerated)
        {
            if(obj.pos < player.position.z / 10 - 10)
            {
                toDelete.Add(obj);
            }
        }

        foreach(structGen obj in toDelete)
        {
            Destroy(obj.obj);
            mapGenerated.Remove(obj);
        }
    }

    public List<Transform> GetIAPath()
    {
        List<Transform> toReturn = new List<Transform>();
        foreach (CliffPath p in cliffPaths)
        {
            List<Transform> tr = new List<Transform>();
            tr = p.GetPath();

            foreach (Transform t in tr)
            {
                toReturn.Add(t);
            }
        }
        return toReturn;
    }
}
