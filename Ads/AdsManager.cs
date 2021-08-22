using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOsGameId;
    private string _gameId;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";
    string _adUnitId;
    public GameRules gameRules;
    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
        InitializeAds();
        Advertisement.AddListener(this);
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, Debug.isDebugBuild);
    }
     
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    //---------------------- video script
    public void PlayRewardedAd()
    {
        Advertisement.Show(_adUnitId);
        if (!Advertisement.IsReady(_adUnitId))
        {
            Debug.Log("Cant Play the ads");
        }
    }
    public void PlayInterstitialAd()
    {
        Advertisement.Show("Interstitial_Android");
        if (!Advertisement.IsReady("Interstitial_Android"))
        {
            Debug.Log("Cant Play the ads");
        }
    }
    public void InterstitialLoadAd()
    {
        Advertisement.Load("Interstitial_Android");
    }
    public bool InterstitialAdsReady()
    {
        if (Advertisement.IsReady("Interstitial_Android"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //---------------------------------------------- Player Reward
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == _adUnitId && showResult == ShowResult.Finished)
        {
            if (gameRules.canPlay == true)
            {
                gameRules.canContinue = true;
            }
            if (gameRules.storeScene == true)
            {
                gameRules.completeTheAdsForSkin = true;
            }
        }
    }
    //adaptando script
    public void LoadAd()
    {
        Advertisement.Load(_adUnitId);
    }
    public bool AdsReady()
    {
        if (Advertisement.IsReady(_adUnitId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //------------------------------------ LOGS
    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("ads is ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("ERROR" + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Video started");
    }
}