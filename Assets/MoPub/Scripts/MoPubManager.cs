using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using AOT;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handler for MoPub integration across publisher apps and Unity Editor.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from this class.
/// </para>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MoPubManager : MonoBehaviour
{
    #region MoPubEvents


    // Fired when the SDK has finished initializing
    public static event Action<string> OnSdkInitializedEvent;

    // Fired when an ad loads in the banner. Includes the ad height.
    public static event Action<string, float> OnAdLoadedEvent;

    // Fired when an ad fails to load for the banner
    public static event Action<string, string> OnAdFailedEvent;

    // Fired when a banner ad is clicked
    public static event Action<string> OnAdClickedEvent;

    // Fired when a banner ad expands to encompass a greater portion of the screen
    public static event Action<string> OnAdExpandedEvent;

    // Fired when a banner ad collapses back to its initial size
    public static event Action<string> OnAdCollapsedEvent;

    // Fired when an interstitial ad is loaded and ready to be shown
    public static event Action<string> OnInterstitialLoadedEvent;

    // Fired when an interstitial ad fails to load
    public static event Action<string, string> OnInterstitialFailedEvent;

    // Fired when an interstitial ad is dismissed
    public static event Action<string> OnInterstitialDismissedEvent;

    // Fired when an interstitial ad expires
    public static event Action<string> OnInterstitialExpiredEvent;

    // Fired when an interstitial ad is displayed
    public static event Action<string> OnInterstitialShownEvent;

    // Fired when an interstitial ad is clicked
    public static event Action<string> OnInterstitialClickedEvent;

    // Fired when a rewarded video finishes loading and is ready to be displayed
    public static event Action<string> OnRewardedVideoLoadedEvent;

    // Fired when a rewarded video fails to load. Includes the error message.
    public static event Action<string, string> OnRewardedVideoFailedEvent;

    // Fired when a rewarded video expires
    public static event Action<string> OnRewardedVideoExpiredEvent;

    // Fired when an rewarded video is displayed
    public static event Action<string> OnRewardedVideoShownEvent;

    // Fired when an rewarded video is clicked
    public static event Action<string> OnRewardedVideoClickedEvent;

    // Fired when a rewarded video fails to play. Includes the error message.
    public static event Action<string, string> OnRewardedVideoFailedToPlayEvent;

    // Fired when a rewarded video completes. Includes all the data available about the reward.
    public static event Action<string, string, float> OnRewardedVideoReceivedRewardEvent;

    // Fired when a rewarded video closes
    public static event Action<string> OnRewardedVideoClosedEvent;

    // iOS only. Fired when a rewarded video event causes another application to open
    public static event Action<string> OnRewardedVideoLeavingApplicationEvent;

#if mopub_native_beta

    // Fired when a native ad is loaded
    public static event Action<string, AbstractNativeAd.Data> OnNativeLoadEvent;

    // Fired when a native ad is shown
    public static event Action<string> OnNativeImpressionEvent;

    // Fired when a native ad is clicked
    public static event Action<string> OnNativeClickEvent;

    // Fired when a native ad fails to load
    public static event Action<string, string> OnNativeFailEvent;

#endif

    // Fired when the SDK has been notified of a change in the user's consent status for data tracking.
    public static event Action<MoPub.Consent.Status, MoPub.Consent.Status, bool> OnConsentStatusChangedEvent;

    // Fired when the SDK has finished loading (retrieving from the web) the MoPub consent dialog interstitial.
    public static event Action OnConsentDialogLoadedEvent;

    // Fired when an error occurred while attempting to load the MoPub consent dialog.
    public static event Action<string> OnConsentDialogFailedEvent;

    // Fired when the MoPub consent dialog has been presented on screen.
    public static event Action OnConsentDialogShownEvent;

    // Fired when the MoPub consent dialog has been dismissed.
    public static event Action OnConsentDialogDismissedEvent;

    // Fired generally when the ad is dismissed, specifically when the Unity Player resumes after an impression is made.
    // This may happen at other times however, such as when the user taps the ad. In some environments, tapping an ad
    // momentarily resumes the Unity Player before opening the browser, which triggers the impression callback before
    // the ad is explicitly dismissed. For more predictable behavior across platforms, see OnImpressionTrackedEventBg.
    //
    // NOTE: ImpressionData will be empty when also subscribed to OnImpressionTrackedEventBg.
    public static event Action<string, MoPub.ImpressionData> OnImpressionTrackedEvent;

    // Fired immediately when the ad is shown, potentially in a background thread. Unlike OnImpressionTrackedEvent
    // which is triggered on the Unity layer, OnImpressionTrackedEventBg is fired natively from Android and iOS.
    //
    // NOTE: Subscribing to this event will cause ImpressionData to be empty on OnImpressionTrackedEvent.
    public static event Action<string, MoPub.ImpressionData> OnImpressionTrackedEventBg;

    #endregion MoPubEvents


    // Singleton.
    public static MoPubManager Instance { get; protected set; }


    #region MoPubManagerPrefab


    [Header("Initialization")]

    [Tooltip("If enabled, the SDK will be initialized at start, based on the values provided in this script and in any attached NetworkConfig scripts.")]
    public bool AutoInitializeOnStart;

    [Tooltip("(iOS Only) The app id on the App Store.  Used to track conversions.")]
    public string itunesAppId;

    [Tooltip("Any ad unit id, used to identify which MoPub account this app will use.")]
    public string AdUnitId;

    [Tooltip("Enables or disables location support for banners and interstitials.")]
    public bool LocationAware;

    [Tooltip("Indicate that this app has Legitimate Interest for GDPR data tracking.")]
    public bool AllowLegitimateInterest;

    [Tooltip("Set the logging verbosity level for the MoPub SDK.")]
    public MoPub.LogLevel LogLevel = MoPub.LogLevel.Info;

    /// <summary>
    /// Collects the information from the above fields and any attached <see cref="NetworkConfiguration"/> objects into a
    /// single <see cref="MoPub.SdkConfiguration"/> struct.
    /// </summary>
    /// <remarks>
    /// Any script on this gameobject can implement a function called OnSdkConfiguration(config).  This function will
    /// be called with the value of this parameter before this property returns.  This allows you to add more configuration
    /// properties (e.g. entries in the NetworkConfiguration dictionary), if the values of these must be determined at
    /// runtime.
    /// </remarks>
    public MoPub.SdkConfiguration SdkConfiguration
    {
        get {
            var config = new MoPub.SdkConfiguration {
                AdUnitId = AdUnitId,
                AllowLegitimateInterest = AllowLegitimateInterest,
                LogLevel = LogLevel,
                MediatedNetworks = GetComponents<MoPubNetworkConfig>().Where(nc => nc.isActiveAndEnabled).Select(nc => nc.NetworkOptions).ToArray()
            };
            SendMessage("OnSdkConfiguration", config, SendMessageOptions.DontRequireReceiver);
            return config;
        }
    }


    // This enables the event to appear in the inspector panel.
    [Serializable] public class InitializedEvent : UnityEvent<string> { }

    [Header("Callback")]

    // Add any callbacks to this event that must execute once the SDK has initialized.
    public InitializedEvent Initialized;

    // API to make calls to the platform-specific MoPub SDK.
    internal static MoPubPlatformApi MoPubPlatformApi { get; private set; }


    // Whether the consent dialog is being shown
    private static bool consentDialogShown;


    // Forwards invocations of C# event OnSdkInitializedEvent to UnityEvent OnInitialized.
    protected void fwdSdkInitialized(string adunitid)
    {
        if (isActiveAndEnabled && Initialized != null)
            Initialized.Invoke(adunitid);
    }


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (!(Instance is MoPubManagerTesting)) {
            // Only warn for multiple production managers.  The testing one is OK since it is supposed to exist
            // side-by-side with a production one.
            Debug.LogWarning("Another production MoPubManager singleton instance already exists.  That object will initialize the SDK instead of this one.");
        }

        if (MoPubPlatformApi == null)
            MoPubPlatformApi = new
#if UNITY_EDITOR
                MoPubUnityEditor
#elif UNITY_ANDROID
                MoPubAndroid
#else
                MoPubiOS
#endif
                ();

        OnSdkInitializedEvent += fwdSdkInitialized;
        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        if (Instance != this || !AutoInitializeOnStart || MoPub.IsSdkInitialized) return;

        MoPub.InitializeSdk(SdkConfiguration);
        MoPub.ReportApplicationOpen(itunesAppId);
        MoPub.EnableLocationSupport(LocationAware);
    }


    /// <summary>
    /// Inform the platform SDK of a change in the application's pause status.
    /// </summary>
    /// <param name="paused">True when pausing, false when resuming</param>
    void OnApplicationPause(bool paused)
    {
        MoPubPlatformApi.OnApplicationPause(paused);
    }


    void OnDestroy()
    {
        OnSdkInitializedEvent -= fwdSdkInitialized;
        if (Instance == this)
            Instance = null;
    }


    #endregion MoPubManagerPrefab


    #region PlatformCallbacks


    internal class BackgroundEventListener : AndroidJavaProxy
    {
        private BackgroundEventListener() : base("com.mopub.unity.MoPubUnityPlugin$IBackgroundEventListener") { }

        public static readonly BackgroundEventListener Instance = new BackgroundEventListener();

        // Called from Android Wrapper (Java).
        public void onEvent(string eventName, string eventArgsJson)
        {
            SendEvent(eventName, eventArgsJson);
        }

        // For use in DllImport declaration to specify the callback signature.
        public delegate void Delegate(string eventName, string eventArgsJson);

        // Called from iOS Wrapper (Objective-C) directly and from Android Wrapper (Java) via onEvent.
        [MonoPInvokeCallback(typeof(Delegate))]
        public static void SendEvent(string eventName, string eventArgsJson)
        {
            try {
                // Handle events that are supported in the background.
                switch (eventName) {
                    case "EmitImpressionTrackedEvent":
                        EmitImpressionTrackedEventHandler(eventArgsJson, background: true);
                        break;
                    default:
                        throw new ArgumentException("Unrecognized event callback name.");
                }
            }
            catch (Exception e) {
                Debug.LogErrorFormat("Exception while handling background event {0}: {1}", eventName, e);
            }
        }
    }


    public void EmitSdkInitializedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];
        var logLevel = MoPub.LogLevel.None;
        if (args.Length > 1) {
            try {
                logLevel = (MoPub.LogLevel) Enum.Parse(typeof(MoPub.LogLevel), args[1]);
            } catch (ArgumentException) {
                Debug.LogWarning("Invalid LogLevel received: " + args[1]);
            }
        } else {
            Debug.LogWarning("No LogLevel received");
        }

        MoPubLog.Log("EmitSdkInitializedEvent", MoPubLog.SdkLogEvent.InitFinished, logLevel);
        var evt = OnSdkInitializedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitConsentStatusChangedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min:3);
        var oldConsent = MoPub.Consent.FromString(args[0]);
        var newConsent = MoPub.Consent.FromString(args[1]);
        var canCollectPersonalInfo = args[2].ToLower() == "true";

        MoPubLog.Log("EmitConsentStatusChangedEvent", MoPubLog.ConsentLogEvent.Updated, oldConsent, newConsent,
            canCollectPersonalInfo ? "" : "not", "unknown");
        var evt = OnConsentStatusChangedEvent;
        if (evt != null) evt(oldConsent, newConsent, canCollectPersonalInfo);
    }


    public void EmitConsentDialogLoadedEvent()
    {
        MoPubLog.Log("EmitConsentDialogLoadedEvent", MoPubLog.ConsentLogEvent.LoadSuccess);
        var evt = OnConsentDialogLoadedEvent;
        if (evt != null) evt();
    }


    public void EmitConsentDialogFailedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var err = args[0];

        MoPubLog.Log("EmitConsentDialogFailedEvent", MoPubLog.ConsentLogEvent.LoadFailed, err);
        var evt = OnConsentDialogFailedEvent;
        if (evt != null) evt(err);
    }


    public void EmitConsentDialogShownEvent()
    {
        consentDialogShown = true;

        MoPubLog.Log("EmitConsentDialogShownEvent", MoPubLog.ConsentLogEvent.ShowSuccess);
        var evt = OnConsentDialogShownEvent;
        if (evt != null) evt();
    }


    public static void EmitConsentDialogDismissedEvent()
    {
        MoPubLog.Log("EmitConsentDialogDismissedEvent", MoPubLog.ConsentLogEvent.Dismissed);
        var evt = OnConsentDialogDismissedEvent;
        if (evt != null) evt();
    }


    // Banner Listeners


    public void EmitAdLoadedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 3);
        var adUnitId = args[0];
        var width = args[1];
        var height = args[2];
        float parsedHeight;
        var parseSucceeded = float.TryParse(height, NumberStyles.Float, CultureInfo.InvariantCulture,
            out parsedHeight);

        MoPubLog.Log("EmitAdLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        MoPubLog.Log("EmitAdLoadedEvent", "Size received: {0}x{1}", width, height);
        MoPubLog.Log("EmitAdLoadedEvent", MoPubLog.AdLogEvent.ShowSuccess);
        if (!parseSucceeded) {
            EmitAdFailedEvent(MoPubUtils.EncodeArgs(adUnitId, "Failed to parse AdLoadedEvent due to invalid ad " +
                                                              "height: (" + height + ")"));
            return;
        }
        var evt = OnAdLoadedEvent;
        if (evt != null) evt(adUnitId, parsedHeight);
    }


    public void EmitAdFailedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitAdFailedEvent", MoPubLog.AdLogEvent.LoadFailed, adUnitId, error);
        var evt = OnAdFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitAdClickedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnAdClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitAdExpandedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdExpandedEvent", MoPubLog.AdLogEvent.Expanded);
        var evt = OnAdExpandedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitAdCollapsedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdCollapsedEvent", MoPubLog.AdLogEvent.Collapsed);
        var evt = OnAdCollapsedEvent;
        if (evt != null) evt(adUnitId);
    }


    // Interstitial Listeners


    public void EmitInterstitialLoadedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        var evt = OnInterstitialLoadedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialFailedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitInterstitialFailedEvent", MoPubLog.AdLogEvent.LoadFailed, adUnitId, error);
        var evt = OnInterstitialFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitInterstitialDismissedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialDismissedEvent", MoPubLog.AdLogEvent.Dismissed);
        var evt = OnInterstitialDismissedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialDidExpireEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialDidExpireEvent", MoPubLog.AdLogEvent.Expired);
        var evt = OnInterstitialExpiredEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialShownEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialShownEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnInterstitialShownEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialClickedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnInterstitialClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    // Rewarded Video Listeners


    public void EmitRewardedVideoLoadedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        var evt = OnRewardedVideoLoadedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoFailedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitRewardedVideoFailedEvent", MoPubLog.AdLogEvent.LoadFailed, adUnitId, error);
        var evt = OnRewardedVideoFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitRewardedVideoExpiredEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoExpiredEvent", MoPubLog.AdLogEvent.Expired);
        var evt = OnRewardedVideoExpiredEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoShownEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoShownEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnRewardedVideoShownEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoClickedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnRewardedVideoClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoFailedToPlayEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        var evt = OnRewardedVideoFailedToPlayEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitRewardedVideoReceivedRewardEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 3);
        var adUnitId = args[0];
        var label = args[1];
        var amount = args[2];
        float parsedAmount;
        var parseSucceeded = float.TryParse(amount, NumberStyles.Float, CultureInfo.InvariantCulture,
            out parsedAmount);

        MoPubLog.Log("EmitRewardedVideoReceivedRewardEvent", MoPubLog.AdLogEvent.ShouldReward, label, amount);
        if (!parseSucceeded) {
            EmitRewardedVideoFailedEvent(MoPubUtils.EncodeArgs(adUnitId,
                "Failed to parse RewardedVideoReceivedRewardEvent due to invalid rewarded amount: (" + amount + ")." +
                " Please ensure the reward info is configured properly on your advertising dashboard."));
            return;
        }
        var evt = OnRewardedVideoReceivedRewardEvent;
        if (evt != null) evt(adUnitId, label, parsedAmount);
    }


    public void EmitRewardedVideoClosedEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoClosedEvent", MoPubLog.AdLogEvent.Dismissed);
        var evt = OnRewardedVideoClosedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoLeavingApplicationEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        var evt = OnRewardedVideoLeavingApplicationEvent;
        if (evt != null) evt(adUnitId);
    }


#if mopub_native_beta
    public void EmitNativeLoadEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var data = AbstractNativeAd.Data.FromJson(args[1]);

        MoPubLog.Log("EmitNativeLoadEvent", MoPubLog.AdLogEvent.LoadSuccess);
        EmitNativeLoadEvent(adUnitId, data);
    }


    public void EmitNativeFailEvent(string argsJson)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitNativeFailEvent", MoPubLog.AdLogEvent.LoadFailed, adUnitId, error);
        var evt = OnNativeFailEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitNativeLoadEvent(string adUnitId, AbstractNativeAd.Data nativeAdData)
    {
        var evt = OnNativeLoadEvent;
        if (evt != null) evt(adUnitId, nativeAdData);
    }


    public void EmitNativeImpressionEvent(string adUnitId)
    {
        MoPubLog.Log("EmitNativeImpressionEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnNativeImpressionEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitNativeClickEvent(string adUnitId)
    {
        MoPubLog.Log("EmitNativeClickEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnNativeClickEvent;
        if (evt != null) evt(adUnitId);
    }
#endif

    private static void EmitImpressionTrackedEventHandler(string argsJson, bool background = false)
    {
        var args = MoPubUtils.DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];
        var impressionData = new MoPub.ImpressionData();
        if (args.Length > 1) {
            if (background || OnImpressionTrackedEventBg == null) // Only include data on *one* event.
                impressionData = MoPub.ImpressionData.FromJson(args[1]);
            else if (OnImpressionTrackedEvent != null)  // Only log if user is subscribed to *both* events.
                Debug.Log("Suppressing impression data from `OnImpressionTrackedEvent` since it was already delivered" +
                          " to `OnImpressionTrackedEventBg`.");
        }

        var evt = background ? OnImpressionTrackedEventBg : OnImpressionTrackedEvent;
        if (evt != null) evt(adUnitId, impressionData);
    }

    public void EmitImpressionTrackedEvent(string argsJson)
    {
        EmitImpressionTrackedEventHandler(argsJson);
    }


    #endregion PlatformCallbacks


    #region SpecialCallbacks


    /// <summary>
    /// Fires the ConsentDialogDismissed event if the application is resuming after a consent dialog was shown.
    /// </summary>
    /// <param name="applicationPaused">True when the application is pausing; False when the application is resuming.</param>
    public static void EmitConsentDialogDismissedIfApplicable(bool applicationPaused)
    {
        if (applicationPaused || !consentDialogShown) return;

        EmitConsentDialogDismissedEvent();
        consentDialogShown = false;
    }


    #endregion SpecialCallbacks
}
