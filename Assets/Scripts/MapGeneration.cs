using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    public Image distance;
    float arrival;
    Transform player;

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
        if (gm.started)
        {
            float _zPos = player.position.z;
            distance.fillAmount = _zPos / arrival;
        }
        else
        {
            distance.fillAmount = 0;
        }
    }

    void _init_()
    {
        GenLevel();
    }

    public void RestartGen()
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

        foreach (Transform child in otherContent.transform)
        {
            Destroy(child.gameObject);
        }
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
    }

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

        foreach (Transform child in tilesContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in otherContent.transform)
        {
            Destroy(child.gameObject);
        }
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
                        break;
                    }
                }
            }
            _count += 1;
        }
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
    }
}
