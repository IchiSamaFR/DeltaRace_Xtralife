using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Advs : MonoBehaviour, IUnityAdsListener
{
    string gameId = "48157c60-8ec6-4f93-81e0-d20094fb639b";
    string GooglePlay_id = "3885229";
    bool TestMode = true;

    GameMaster gm;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GameMaster.instance;
    }
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(GooglePlay_id, TestMode);
    }

    /* Show an ad to double coins get
     */
    public void ShowDoubleRaceAds()
    {
        Advertisement.Show("rewardedVideo");
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    /* Action by ads view
     */
    public void OnUnityAdsDidFinish(string placementId, ShowResult result)
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
        print("end");
    }
}
