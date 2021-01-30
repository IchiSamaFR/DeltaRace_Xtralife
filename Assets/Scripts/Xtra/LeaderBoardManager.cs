using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using CotcSdk;
using static Tools;

public class LeaderBoardManager : MonoBehaviour
{
    public Transform content;
    public GameObject boardItem;


    public static LeaderBoardManager instance;

    List<Achievement> achievements = new List<Achievement>();

    Gamer Gamer;

    void Awake()
    {
        if (instance)
        {
            Debug.LogError("LeaderBoard Manager already defined.");
            return;
        }
        instance = this;
    }

    public void SetGamer(Gamer gamer)
    {
        Gamer = gamer;
        RefreshBoard();
    }

    public void RefreshBoard()
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        Gamer.Scores.Domain("private").BestHighScores("infinite", 10)
        .Done(ResultScores => {
            Debug.Log("----- Scores");
            foreach (var score in ResultScores)
            {
                Transform obj = Instantiate(boardItem, content).transform;
                obj.GetChild(1).GetComponent<TextMeshProUGUI>().text = score.GamerInfo["profile"]["displayName"] + "\n" + score.Value;
            }
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            if (GetJsonError(error.ServerData.ToString()).name == "MissingScore")
            {
                PostScore(0);
            }
        });
    }

    public void PostScore(int value)
    {
        Gamer.Scores.Domain("private").Post(value, "infinite", ScoreOrder.HighToLow,
        "Highest score in infinite mode.", false)
        .Done(postScoreRes => {
            Debug.Log("Post score: " + postScoreRes.ToString());
            RefreshBoard();
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

}
