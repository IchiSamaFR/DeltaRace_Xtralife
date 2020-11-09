using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Advs : MonoBehaviour
{
    string GooglePlay_id = "3885229";
    bool TestMode = true;

    GameMaster gm;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GameMaster.instance;
    }

    /* Show an ad to double coins get
     */
    public void ShowDoubleRaceAds()
    {
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show("rewardedVideo", options);
    }
    /* Action by ads view
     */
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                gm.GetDoubleRaceCoins();
                break;
            case ShowResult.Skipped:
                gm.GetDoubleRaceCoins();
                break;
            case ShowResult.Failed:
                break;
        }
    }
}
