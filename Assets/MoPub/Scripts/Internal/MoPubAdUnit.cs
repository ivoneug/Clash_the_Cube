using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bridge between the MoPub Unity AdUnit-specific API and platform-specific implementations.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For platform-specific implementations, see
/// <see cref="MoPubUnityEditorAdUnit"/>, <see cref="MoPubAndroidAdUnit"/>, and <see cref="MoPubiOSAdUnit"/>.
/// </para>
internal class MoPubAdUnit
{
    public static readonly MoPubAdUnit NullMoPubAdUnit = new MoPubAdUnit(null);

    protected readonly string AdUnitId;
    protected readonly string AdType;

    protected MoPubAdUnit(string adUnitId, string adType = null)
    {
        AdUnitId = adUnitId;
        AdType = adType;
        SelectedReward = new MoPub.Reward { Label = string.Empty };
    }

    internal static MoPubAdUnit CreateMoPubAdUnit(string adUnitId, string adType = null)
    {
        if (adType != "Banner" && adType != "Interstitial" && adType != "RewardedVideo" && adType != "Native")
            Debug.LogErrorFormat("FATAL ERROR: Invalid ad type for MoPubAdUnit: \"{0}\"", adType);

        // Choose created class based on target platform...
        return new
#if UNITY_EDITOR
            MoPubUnityEditorAdUnit
#elif UNITY_ANDROID
            MoPubAndroidAdUnit
#else
            MoPubiOSAdUnit
#endif
        (adUnitId, adType);
    }


    internal virtual bool IsPluginReady()
    {
        return false;
    }


    #region Banners


    internal virtual void RequestBanner(float width, float height, MoPub.AdPosition position, string keywords = "",
        string userDataKeywords = "") { }


    internal virtual void ShowBanner(bool shouldShow) { }


    internal virtual void RefreshBanner(string keywords = "", string userDataKeywords = "") { }


    internal virtual void DestroyBanner() { }


    internal virtual void SetAutorefresh(bool enabled) { }


    internal virtual void ForceRefresh() { }

    #endregion Banners


    #region Interstitials

    internal virtual void RequestInterstitialAd(string keywords = "", string userDataKeywords = "") { }


    internal virtual bool IsInterstitialReady()
    {
        return false;
    }


    internal virtual void ShowInterstitialAd() { }


    internal virtual void DestroyInterstitialAd() { }

    #endregion Interstitials


    #region RewardedVideos

    protected MoPub.Reward SelectedReward;

    internal virtual void RequestRewardedVideo(List<MoPub.LocalMediationSetting> mediationSettings = null,
        string keywords = null,
        string userDataKeywords = null, double latitude = MoPub.LatLongSentinel,
        double longitude = MoPub.LatLongSentinel, string customerId = null) { }


    // Queries if a rewarded video ad has been loaded for the given ad unit id.
    internal virtual bool HasRewardedVideo() { return false; }


    // Queries all of the available rewards for the ad unit. This is only valid after
    // a successful requestRewardedVideo() call.
    internal virtual List<MoPub.Reward> GetAvailableRewards() { return null; }


    // If a rewarded video ad is loaded this will take over the screen and show the ad
    internal virtual void ShowRewardedVideo(string customData) { }

    internal virtual void SelectReward(MoPub.Reward selectedReward) { }

    #endregion RewardedVideos


#if mopub_native_beta
    #region NativeAds

    internal virtual void RequestNativeAd() { }

    #endregion NativeAds

#endif


    protected bool CheckPluginReady()
    {
        var isReady = IsPluginReady();

        if (!isReady)
            Debug.LogErrorFormat("Invalid call to MoPub API; {0} AdUnit with ID {1} has not been requested yet.",
                AdType, AdUnitId);

        return isReady;

    }
}
