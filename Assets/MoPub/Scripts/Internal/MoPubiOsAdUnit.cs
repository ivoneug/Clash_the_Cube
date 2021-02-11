using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// Bridge between the MoPub Unity AdUnit-specific API and iOS implementation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For platform-specific implementations, see
/// <see cref="MoPubUnityEditorAdUnit"/> and <see cref="MoPubAndroidAdUnit"/>.
/// </para>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal class MoPubiOSAdUnit : MoPubAdUnit
{
    internal MoPubiOSAdUnit(string adUnitId, string adType = null) : base (adUnitId, adType) { }


    internal override bool IsPluginReady()
    {
        return _moPubIsPluginReady(AdUnitId);
    }


    #region Banners

    internal override void RequestBanner(float width, float height, MoPub.AdPosition position, string keywords = "",
        string userDataKeywords = "")
    {
        _moPubRequestBanner(width, height, (int) position, AdUnitId, keywords, userDataKeywords);
    }


    internal override void DestroyBanner()
    {
        if (!CheckPluginReady()) return;

        _moPubDestroyBanner(AdUnitId);
    }


    internal override void ShowBanner(bool shouldShow)
    {
        if (!CheckPluginReady()) return;

        _moPubShowBanner(AdUnitId, shouldShow);
    }


    internal override void RefreshBanner(string keywords = "", string userDataKeywords = "")
    {
        if (!CheckPluginReady()) return;

        _moPubRefreshBanner(AdUnitId, keywords, userDataKeywords);
    }


    internal override void SetAutorefresh(bool enabled)
    {
        if (!CheckPluginReady()) return;

        _moPubSetAutorefreshEnabled(AdUnitId, enabled);
    }


    internal override void ForceRefresh()
    {
        if (!CheckPluginReady()) return;

        _moPubForceRefresh(AdUnitId);
    }

    #endregion

    #region Interstitials

    internal override void RequestInterstitialAd(string keywords = "", string userDataKeywords = "")
    {
        _moPubRequestInterstitialAd(AdUnitId, keywords, userDataKeywords);
    }


    internal override bool IsInterstitialReady() {
        return _moPubIsInterstitialReady(AdUnitId);
    }


    internal override void ShowInterstitialAd()
    {
        if (!CheckPluginReady()) return;

        _moPubShowInterstitialAd(AdUnitId);
    }


    internal override void DestroyInterstitialAd()
    {
        if (!CheckPluginReady()) return;

        _moPubDestroyInterstitialAd(AdUnitId);
    }

    #endregion

    #region RewardedVideos

    internal override void RequestRewardedVideo(List<MoPub.LocalMediationSetting> mediationSettings = null, string keywords = null,
                                     string userDataKeywords = null, double latitude = MoPub.LatLongSentinel,
                                     double longitude = MoPub.LatLongSentinel, string customerId = null)
    {
        var json = MoPub.LocalMediationSetting.ToJson(mediationSettings);
        _moPubRequestRewardedVideo(AdUnitId, json, keywords, userDataKeywords, latitude, longitude, customerId);
    }


    // If a rewarded video ad is loaded this will take over the screen and show the ad
    internal override void ShowRewardedVideo(string customData)
    {
        if (!CheckPluginReady()) return;

        _moPubShowRewardedVideo(AdUnitId, SelectedReward.Label, SelectedReward.Amount, customData);
    }


    // Queries if a rewarded video ad has been loaded for the given ad unit id.
    internal override bool HasRewardedVideo()
    {
        return _mopubHasRewardedVideo(AdUnitId);
    }


    // Queries all of the available rewards for the ad unit. This is only valid after
    // a successful requestRewardedVideo() call.
    internal override List<MoPub.Reward> GetAvailableRewards()
    {
        if (!CheckPluginReady()) return new List<MoPub.Reward>();

        var amount = 0;
        var rewardList = _mopubGetAvailableRewards(AdUnitId) ?? string.Empty;
        var rewards = from rewardString in rewardList.Split(',')
                      select rewardString.Split(':')
                      into rewardComponents
                      where rewardComponents.Length == 2
                      where int.TryParse(rewardComponents[1], NumberStyles.Any, CultureInfo.InvariantCulture, out amount)
                      select new MoPub.Reward { Label = rewardComponents[0], Amount = amount };
        return rewards.ToList();
    }

    internal override void SelectReward(MoPub.Reward selectedReward)
    {
        if (!CheckPluginReady()) return;

        SelectedReward = selectedReward;
    }

    #endregion

#if mopub_native_beta
    #region NativeAds

    internal override void RequestNativeAd()
    {
        throw new NotImplementedException("Native ads are not available for iOS yet.");
    }

    #endregion
#endif

    #region DllImports

#if ENABLE_IL2CPP && UNITY_ANDROID
    // IL2CPP on Android scrubs DllImports, so we need to provide stubs to unblock compilation
    private static bool _moPubIsPluginReady(string adUnitId) { return false; }
    private static void _moPubRequestBanner(float width, float height, int position, string adUnitId,
                                            string keywords, string userDataKeywords) {}
    private static void _moPubDestroyBanner(string adUnitId) {}
    private static void _moPubShowBanner(string adUnitId, bool shouldShow) {}
    private static void _moPubRefreshBanner(string adUnitId, string keywords, string userDataKeywords) {}
    private static void _moPubSetAutorefreshEnabled(string adUnitId, bool enabled) {}
    private static void _moPubForceRefresh(string adUnitId) {}
    private static void _moPubRequestInterstitialAd(string adUnitId, string keywords, string userDataKeywords) {}
    private static bool _moPubIsInterstitialReady(string adUnitId) { return false; }
    private static void _moPubShowInterstitialAd(string adUnitId) {}
    private static void _moPubDestroyInterstitialAd(string adUnitId) {}
    private static void _moPubRequestRewardedVideo(string adUnitId, string json, string keywords,
                                                   string userDataKeywords, double latitude, double longitude,
                                                   string customerId) {}
    private static bool _mopubHasRewardedVideo(string adUnitId) { return false; }
    private static string _mopubGetAvailableRewards(string adUnitId) { return null; }
    private static void _moPubShowRewardedVideo(string adUnitId, string currencyName, int currencyAmount,
                                                string customData) {}
#else
    [DllImport("__Internal")]
    private static extern bool _moPubIsPluginReady(string adUnitId);

    [DllImport("__Internal")]
    private static extern void _moPubRequestBanner(float width, float height, int position, string adUnitId,
                                                   string keywords, string userDataKeywords);


    [DllImport("__Internal")]
    private static extern void _moPubDestroyBanner(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubShowBanner(string adUnitId, bool shouldShow);


    [DllImport("__Internal")]
    private static extern void _moPubRefreshBanner(string adUnitId, string keywords, string userDataKeywords);


    [DllImport("__Internal")]
    private static extern void _moPubSetAutorefreshEnabled(string adUnitId, bool enabled);


    [DllImport("__Internal")]
    private static extern void _moPubForceRefresh(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubRequestInterstitialAd(string adUnitId, string keywords, string userDataKeywords);


    [DllImport("__Internal")]
    private static extern bool _moPubIsInterstitialReady(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubShowInterstitialAd(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubDestroyInterstitialAd(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubRequestRewardedVideo(string adUnitId, string json, string keywords,
                                                          string userDataKeywords, double latitude, double longitude,
                                                          string customerId);


    [DllImport("__Internal")]
    private static extern bool _mopubHasRewardedVideo(string adUnitId);


    [DllImport("__Internal")]
    private static extern string _mopubGetAvailableRewards(string adUnitId);


    [DllImport("__Internal")]
    private static extern void _moPubShowRewardedVideo(string adUnitId, string currencyName, int currencyAmount,
                                                       string customData);
#endif

    #endregion
}
