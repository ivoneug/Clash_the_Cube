#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bridge between the MoPub Unity AdUnit-specific API and In-Editor implementation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For other platform-specific implementations, see
/// <see cref="MoPubAndroidAdUnit"/> and <see cref="MoPubiOSAdUnit"/>.
/// </para>
internal class MoPubUnityEditorAdUnit : MoPubAdUnit
{
    private bool _requested;

    internal MoPubUnityEditorAdUnit(string adUnitId, string adType = null) : base(adUnitId, adType) { }

    internal override bool IsPluginReady()
    {
        return _requested;
    }

    #region Banners

    internal override void RequestBanner(float width, float height, MoPub.AdPosition position, string keywords = "",
        string userDataKeywords = "")
    {
        RequestAdUnit();
        ForceRefresh();
    }

    internal override void ShowBanner(bool shouldShow)
    {
        CheckAdUnitRequested();
    }

    internal override void RefreshBanner(string keywords = "", string userDataKeywords = "")
    {
        if (CheckAdUnitRequested())
            ForceRefresh();
    }

    internal override void DestroyBanner()
    {
        CheckAdUnitRequested();
    }

    internal override void SetAutorefresh(bool enabled)
    {
        CheckAdUnitRequested();
    }

    internal override void ForceRefresh()
    {
        if (!CheckAdUnitRequested()) return;

        MoPubUnityEditor.WaitOneFrame(() => {
            MoPubManager.Instance.EmitAdLoadedEvent(MoPubUnityEditor.ArgsToJson(AdUnitId, "320", "50"));
        });
    }

    #endregion

    #region Interstitials

    internal override void RequestInterstitialAd(string keywords = "", string userDataKeywords = "")
    {
        RequestAdUnit();
        MoPubUnityEditor.WaitOneFrame(() => {
            MoPubManager.Instance.EmitInterstitialLoadedEvent(MoPubUnityEditor.ArgsToJson(AdUnitId));
        });
    }

    internal override bool IsInterstitialReady()
    {
        return _requested;
    }

    internal override void ShowInterstitialAd()
    {
        if (!CheckAdUnitRequested()) return;

        MoPubUnityEditor.WaitOneFrame(() => {
            var json = MoPubUnityEditor.ArgsToJson(AdUnitId);
            MoPubManager.Instance.EmitInterstitialShownEvent(json);
            MoPubUnityEditor.WaitOneFrame(() => {
                MoPubManager.Instance.EmitInterstitialDismissedEvent(json);
                MoPubUnityEditor.SimulateApplicationResume();
            });
        });
    }

    internal override void DestroyInterstitialAd()
    {
        CheckAdUnitRequested();
    }

    #endregion

    #region RewardedVideos

    internal override void RequestRewardedVideo(List<MoPub.LocalMediationSetting> mediationSettings = null, string keywords = null, string userDataKeywords = null,
        double latitude = MoPub.LatLongSentinel, double longitude = MoPub.LatLongSentinel,
        string customerId = null)
    {
        RequestAdUnit();
        MoPubUnityEditor.WaitOneFrame(() => {
            MoPubManager.Instance.EmitRewardedVideoLoadedEvent(MoPubUnityEditor.ArgsToJson(AdUnitId));
        });
    }

    internal override bool HasRewardedVideo()
    {
        return _requested;
    }

    internal override List<MoPub.Reward> GetAvailableRewards()
    {
        CheckAdUnitRequested();
        return new List<MoPub.Reward>();
    }

    internal override void ShowRewardedVideo(string customData)
    {
        if (!CheckAdUnitRequested()) return;

        MoPubUnityEditor.WaitOneFrame(() => {
            var json = MoPubUnityEditor.ArgsToJson(AdUnitId);
            MoPubManager.Instance.EmitRewardedVideoShownEvent(json);
            MoPubUnityEditor.WaitOneFrame(() => {
                MoPubManager.Instance.EmitRewardedVideoClosedEvent(json);
                MoPubUnityEditor.SimulateApplicationResume();
            });
        });
    }

    internal override void SelectReward(MoPub.Reward selectedReward)
    {
        if (CheckAdUnitRequested())
            SelectedReward = selectedReward;
    }

    #endregion

#if mopub_native_beta
    #region NativeAds

    internal override void RequestNativeAd()
    {
        MoPubLog.Log("RequestNativeAd", MoPubLog.AdLogEvent.LoadAttempted);
        RequestAdUnit();
        MoPubUnityEditor.WaitOneFrame(() => {
            if (!"1".Equals(AdUnitId))  {
                Debug.Log("Native ad unit was requested: " + AdUnitId);
                return;
            }
            MoPubManager.Instance.EmitNativeLoadEvent(AdUnitId, new AbstractNativeAd.Data {
                MainImageUrl =
                    new Uri("https://d30x8mtr3hjnzo.cloudfront.net/creatives/8d0a2ba02b2b485f97e1867366762951"),
                IconImageUrl =
                    new Uri("https://d30x8mtr3hjnzo.cloudfront.net/creatives/6591163c525f4720b99abf831ca247f6"),
                ClickDestinationUrl = new Uri("https://www.mopub.com/click-test"),
                CallToAction = "Go",
                Title = "MoPub",
                Text = "Success! Your integration is ready to go. Tap to test this ad.",
                PrivacyInformationIconClickThroughUrl = new Uri("https://www.mopub.com/optout/")
            });
        });
    }

    #endregion
#endif

    private void RequestAdUnit()
    {
        _requested = true;
    }

    private bool CheckAdUnitRequested()
    {
        return CheckPluginReady() && _requested;
    }
}
#endif
