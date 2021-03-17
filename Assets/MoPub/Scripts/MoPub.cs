using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using MJ = MoPubInternal.ThirdParty.MiniJSON;

/// <summary>
/// MoPub Unity API for publishers, including documentation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through this class, and handle any desired MoPub Events from
/// the <see cref="MoPubManager"/> class.
/// </para>
public abstract class MoPub : MoPubBase
{

    /// <summary>
    /// The version for the MoPub Unity SDK, which includes specific versions of the MoPub Android and iOS SDKs.
    /// <para>
    /// Please see <a href="https://github.com/mopub/mopub-unity-sdk">our GitHub repository</a> for details.
    /// </para>
    /// </summary>
    public const string MoPubSdkVersion = "5.16.1";


    #region SdkSetup


    /// <summary>
    /// Asynchronously initializes the relevant (Android or iOS) MoPub SDK.
    /// See <see cref="MoPubManager.OnSdkInitializedEvent"/> for resulting triggered event.
    /// </summary>
    /// <param name="anyAdUnitId">String with any ad unit id used by this app.</param>
    /// <remarks>The MoPub SDK needs to be initialized on Start() to ensure all other objects have been enabled first.
    /// (Start() rather than Awake() so that MoPubManager has had time to Awake() and OnEnable() in order to receive
    /// event callbacks.)</remarks>
    public static void InitializeSdk(string anyAdUnitId)
    {
        InitializeSdk(new SdkConfiguration { AdUnitId = anyAdUnitId });
    }


    /// <summary>
    /// Asynchronously initializes the relevant (Android or iOS) MoPub SDK. Call this before making any rewarded ads or
    /// advanced bidding requests. This will do the rewarded video custom event initialization any number of times, but
    /// the SDK itself can only be initialized once, and the rewarded ads module can only be initialized once.
    /// See <see cref="MoPubManager.OnSdkInitializedEvent"/> for the resulting triggered event.
    /// </summary>
    /// <param name="sdkConfiguration">The configuration including at least an ad unit.
    /// See <see cref="MoPub.SdkConfiguration"/> for details.</param>
    /// <remarks>The MoPub SDK needs to be initialized on Start() to ensure all other objects have been enabled first.
    /// (Start() rather than Awake() so that MoPubManager has had time to Awake() and OnEnable() in order to receive
    /// event callbacks.)</remarks>
    public static void InitializeSdk(SdkConfiguration sdkConfiguration)
    {
        MoPubLog.Log("InitializeSdk", MoPubLog.SdkLogEvent.InitStarted);

        ValidateAdUnitForSdkInit(sdkConfiguration.AdUnitId);

        MoPubManager.MoPubPlatformApi.InitializeSdk(sdkConfiguration);
        MoPubManager.MoPubPlatformApi.SetEngineInformation("unity", Application.unityVersion);
        CachedLogLevel = sdkConfiguration.LogLevel;
    }


    /// <summary>
    /// Initializes a platform-specific MoPub SDK banner plugin for each given ad unit.
    /// </summary>
    /// <param name="adUnitIds">The ad units to initialize plugins for</param>
    public static void LoadBannerPluginsForAdUnits(string[] adUnitIds)
    {
        LoadPluginsForAdUnits(adUnitIds, "Banner");
    }


    /// <summary>
    /// Initializes a platform-specific MoPub SDK interstitial plugin for each given ad unit.
    /// </summary>
    /// <param name="adUnitIds">The ad units to initialize plugins for</param>
    public static void LoadInterstitialPluginsForAdUnits(string[] adUnitIds)
    {
        LoadPluginsForAdUnits(adUnitIds, "Interstitial");
    }


    /// <summary>
    /// Initializes a platform-specific MoPub SDK rewarded video plugin for each given ad unit.
    /// </summary>
    /// <param name="adUnitIds">The ad units to initialize plugins for</param>
    public static void LoadRewardedVideoPluginsForAdUnits(string[] adUnitIds)
    {
        LoadPluginsForAdUnits(adUnitIds, "RewardedVideo");
    }


#if mopub_native_beta
    /// <summary>
    /// Initializes a platform-specific MoPub SDK native plugin for each given ad unit.
    /// </summary>
    /// <param name="adUnitIds">The ad units to initialize plugins for</param>
    public static void LoadNativePluginsForAdUnits(string[] adUnitIds)
    {
        LoadPluginsForAdUnits(adUnitIds, "Native");
    }
#endif


    /// <summary>
    /// Enables or disables location support for banners and interstitials.
    /// </summary>
    /// <param name="shouldUseLocation">Whether location should be enabled or not.</param>
    public static void EnableLocationSupport(bool shouldUseLocation)
    {
        ValidateInit();
        MoPubManager.MoPubPlatformApi.EnableLocationSupport(shouldUseLocation);
    }


    /// <summary>
    /// Reports an app download to MoPub.
    /// </summary>
    /// <param name="iTunesAppId">The app id on the App Store (only applicable to iOS).</param>
    public static void ReportApplicationOpen(string iTunesAppId = null)
    {
        ValidateInit();
        MoPubManager.MoPubPlatformApi.ReportApplicationOpen(iTunesAppId);
    }


    /// <summary>
    /// Disables viewability measurement for the rest of the app session.
    /// </summary>
    public static void DisableViewability()
    {
        MoPubManager.MoPubPlatformApi.DisableViewability();
    }


    /// <summary>
    /// Returns a human-readable string of the MoPub SDK being used.
    /// </summary>
    /// <returns>A string with the MoPub SDK platform and version.</returns>
    public static string SdkName
    {
        get { return MoPubManager.MoPubPlatformApi.SdkName; }
    }


    /// <summary>
    /// Flag indicating if the SDK has been initialized.
    /// </summary>
    /// <returns>true if a call to initialize the SDK has been made; false otherwise.</returns>
    public static bool IsSdkInitialized {
        get { return MoPubManager.MoPubPlatformApi != null && MoPubManager.MoPubPlatformApi.IsSdkInitialized; }
    }


    /// <summary>
    /// MoPub SDK log level. The default value is: `MPLogLevelInfo` before SDK init, `MPLogLevelNone` after SDK init.
    /// See MoPub.<see cref="MoPub.LogLevel"/> for all possible options. Can also be set via
    /// MoPub.<see cref="MoPub.SdkConfiguration"/> on
    /// MoPub.<see cref="MoPub.InitializeSdk(MoPub.SdkConfiguration)"/>
    /// </summary>
    public static LogLevel SdkLogLevel
    {
        get {
            var logLevel = MoPubManager.MoPubPlatformApi.SdkLogLevel;
            CachedLogLevel = logLevel;
            return logLevel;
        }
        set {
            MoPubManager.MoPubPlatformApi.SdkLogLevel = value;
            CachedLogLevel = value;
        }
    }


    /// <summary>
    /// Allow supported SDK networks to collect user information on the basis of legitimate interest.
    /// Can also be set via MoPub.<see cref="MoPub.SdkConfiguration"/> on
    /// MoPub.<see cref="MoPubUnityEditor.InitializeSdk(MoPub.SdkConfiguration)"/>
    /// </summary>
    public static bool AllowLegitimateInterest
    {
        get { return MoPubManager.MoPubPlatformApi.AllowLegitimateInterest; }
        set { MoPubManager.MoPubPlatformApi.AllowLegitimateInterest = value; }
    }


    #endregion SdkSetup


    #region AndroidOnly


    /// <summary>
    /// Registers the given device as a Facebook Ads test device.
    /// </summary>
    /// <param name="hashedDeviceId">String with the hashed ID of the device.</param>
    /// <remarks>See https://developers.facebook.com/docs/reference/android/current/class/AdSettings/ for details
    /// </remarks>
    public static void AddFacebookTestDeviceId(string hashedDeviceId)
    {
        MoPubManager.MoPubPlatformApi.AddFacebookTestDeviceId(hashedDeviceId);
    }


    #endregion AndroidOnly


    #region iOSOnly


    /// <summary>
    /// Forces the usage of WKWebView, if able.
    /// </summary>
    /// <param name="shouldForce">Whether to attempt to force the usage of WKWebView or not.</param>
    public static void ForceWKWebView(bool shouldForce)
    {
        MoPubManager.MoPubPlatformApi.ForceWKWebView(shouldForce);
    }


    #endregion iOSOnly


    #region Banners


    /// <summary>
    /// Requests a banner ad and immediately shows it once loaded.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="position">Where in the screen to position the loaded ad. See <see cref="MoPub.AdPosition"/>.
    /// </param>
    /// <param name="maxAdSize">The maximum size of the banner to load. See <see cref="MoPub.MaxAdSize"/>.</param>
    /// <param name="keywords">An optional comma-separated string with the desired non-PII keywords for this ad.</param>
    /// <param name="userDataKeywords">An optional comma-separated string with the desired PII keywords for this ad.
    /// </param>
    public static void RequestBanner(string adUnitId, AdPosition position,
        MaxAdSize maxAdSize = MaxAdSize.Width320Height50, string keywords = "", string userDataKeywords = "")
    {
        var width = maxAdSize.Width();
        var height = maxAdSize.Height();
        MoPubLog.Log("RequestBanner", MoPubLog.AdLogEvent.LoadAttempted);
        MoPubLog.Log("RequestBanner", "Size requested: " + width + "x" + height);
        AdUnitManager.GetAdUnit(adUnitId).RequestBanner(width, height, position, keywords, userDataKeywords);
    }


    /// <summary>
    /// Shows or hides an already-loaded banner ad.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="shouldShow">A bool with `true` to show the ad, or `false` to hide it.</param>
    /// <remarks>Banners are automatically shown after first loading.</remarks>
    public static void ShowBanner(string adUnitId, bool shouldShow)
    {
        if (shouldShow) MoPubLog.Log("ShowBanner", MoPubLog.AdLogEvent.ShowAttempted);
        AdUnitManager.GetAdUnit(adUnitId).ShowBanner(shouldShow);
    }


    /// <summary>
    /// Sets the desired keywords and reloads the banner ad.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="keywords">A comma-separated string with the desired keywords for this ad.</param>
    /// <param name="userDataKeywords">An optional comma-separated string with user data for this ad.</param>
    /// <remarks>If a user is in a General Data Protection Regulation (GDPR) region and MoPub doesn't obtain consent
    /// from the user, "keywords" will be sent to the server but "userDataKeywords" will be excluded.
    /// (See <see cref="CanCollectPersonalInfo"/>).</remarks>
    public static void RefreshBanner(string adUnitId, string keywords, string userDataKeywords = "")
    {
        MoPubLog.Log("RefreshBanner", MoPubLog.AdLogEvent.ShowAttempted);
        AdUnitManager.GetAdUnit(adUnitId).RefreshBanner(keywords, userDataKeywords);
    }


    /// <summary>
    /// Enables or disables banners automatically refreshing every 30 seconds.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="enabled">Whether to enable or disable autorefresh.</param>
    public static void SetAutorefresh(string adUnitId, bool enabled)
    {
        AdUnitManager.GetAdUnit(adUnitId).SetAutorefresh(enabled);

    }


    /// <summary>
    /// Refreshes the banner ad regardless of whether autorefresh is enabled or not.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    public static void ForceRefresh(string adUnitId)
    {
        MoPubLog.Log("ForceRefresh", MoPubLog.AdLogEvent.ShowAttempted);
        AdUnitManager.GetAdUnit(adUnitId).ForceRefresh();
    }


    /// <summary>
    /// Destroys the banner ad and removes it from the view.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    public static void DestroyBanner(string adUnitId)
    {
        AdUnitManager.GetAdUnit(adUnitId).DestroyBanner();
    }


    #endregion


    #region Interstitials


    /// <summary>
    /// Requests an interstitial ad with the given (optional) keywords to be loaded. The two possible resulting events
    /// are <see cref="MoPubManager.OnInterstitialLoadedEvent"/> and
    /// <see cref="MoPubManager.OnInterstitialFailedEvent"/>.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="keywords">An optional comma-separated string with the desired keywords for this ad.</param>
    /// <param name="userDataKeywords">An optional comma-separated string with user data for this ad.</param>
    /// <remarks>If a user is in a General Data Protection Regulation (GDPR) region and MoPub doesn't obtain consent
    /// from the user, "keywords" will be sent to the server but "userDataKeywords" will be excluded.
    /// (See <see cref="CanCollectPersonalInfo"/>).</remarks>
    public static void RequestInterstitialAd(string adUnitId, string keywords = "", string userDataKeywords = "")
    {
        MoPubLog.Log("RequestInterstitialAd", MoPubLog.AdLogEvent.LoadAttempted);
        AdUnitManager.GetAdUnit(adUnitId).RequestInterstitialAd(keywords, userDataKeywords);
    }


    /// <summary>
    /// If the interstitial ad has loaded, this will take over the screen and show the ad.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <remarks><see cref="MoPubManager.OnInterstitialLoadedEvent"/> must have been triggered already.</remarks>
    public static void ShowInterstitialAd(string adUnitId)
    {
        MoPubLog.Log("ShowInterstitialAd", MoPubLog.AdLogEvent.ShowAttempted);
        AdUnitManager.GetAdUnit(adUnitId).ShowInterstitialAd();
    }


    /// <summary>
    /// Whether the interstitial ad is ready to be shown or not.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    public static bool IsInterstitialReady(string adUnitId)
    {
        return AdUnitManager.GetAdUnit(adUnitId).IsInterstitialReady();
    }


    /// <summary>
    /// Destroys an already-loaded interstitial ad.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    public static void DestroyInterstitialAd(string adUnitId)
    {
        AdUnitManager.GetAdUnit(adUnitId).DestroyInterstitialAd();
    }


    #endregion Interstitials


    #region RewardedVideos


    /// <summary>
    /// Requests an rewarded video ad with the given (optional) configuration to be loaded. The two possible resulting
    /// events are <see cref="MoPubManager.OnRewardedVideoLoadedEvent"/> and
    /// <see cref="MoPubManager.OnRewardedVideoFailedEvent"/>.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="mediationSettings">See <see cref="MoPub.SdkConfiguration.MediationSettings"/>.</param>
    /// <param name="keywords">An optional comma-separated string with the desired keywords for this ad.</param>
    /// <param name="userDataKeywords">An optional comma-separated string with user data for this ad.</param>
    /// <param name="latitude">An optional location latitude to be used for this ad.</param>
    /// <param name="longitude">An optional location longitude to be used for this ad.</param>
    /// <param name="customerId">An optional string to indentify this user within this app. </param>
    /// <remarks>If a user is in a General Data Protection Regulation (GDPR) region and MoPub doesn't obtain consent
    /// from the user, "keywords" will be sent to the server but "userDataKeywords" will be excluded.
    /// (See <see cref="CanCollectPersonalInfo"/>).</remarks>
    public static void RequestRewardedVideo(string adUnitId, List<LocalMediationSetting> mediationSettings = null,
                                            string keywords = null, string userDataKeywords = null,
                                            double latitude = LatLongSentinel, double longitude = LatLongSentinel,
                                            string customerId = null)
    {
        MoPubLog.Log("RequestRewardedVideo", MoPubLog.AdLogEvent.LoadAttempted);
        AdUnitManager.GetAdUnit(adUnitId).RequestRewardedVideo(mediationSettings, keywords, userDataKeywords, latitude,
            longitude, customerId);
    }


    /// <summary>
    /// If the rewarded video ad has loaded, this will take over the screen and show the ad.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="customData">An optional string with custom data for the ad.</param>
    /// <remarks><see cref="MoPubManager.OnRewardedVideoLoadedEvent"/> must have been triggered already.</remarks>
    public static void ShowRewardedVideo(string adUnitId, string customData = null)
    {
        MoPubLog.Log("ShowRewardedVideo", MoPubLog.AdLogEvent.ShowAttempted);
        AdUnitManager.GetAdUnit(adUnitId).ShowRewardedVideo(customData);

    }


    /// <summary>
    /// Whether a rewarded video is ready to play for this ad unit.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <returns>`true` if a rewarded ad for the given ad unit id is loaded and ready to be shown; false othewise
    /// </returns>
    public static bool HasRewardedVideo(string adUnitId)
    {
        return AdUnitManager.GetAdUnit(adUnitId).HasRewardedVideo();
    }


    /// <summary>
    /// Retrieves a list of available rewards for the given ad unit id.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <returns>A list of <see cref="MoPub.Reward"/>s for the given ad unit id.</returns>
    public static List<Reward> GetAvailableRewards(string adUnitId)
    {
        var rewards = AdUnitManager.GetAdUnit(adUnitId).GetAvailableRewards();
        Debug.Log(String.Format("GetAvailableRewards found {0} rewards for ad unit {1}", rewards.Count, adUnitId));
        return rewards;
    }


    /// <summary>
    /// Selects the reward to give the user when the ad has finished playing.
    /// </summary>
    /// <param name="adUnitId">A string with the ad unit id.</param>
    /// <param name="selectedReward">See <see cref="MoPub.Reward"/>.</param>
    public static void SelectReward(string adUnitId, Reward selectedReward)
    {
        AdUnitManager.GetAdUnit(adUnitId).SelectReward(selectedReward);
    }


    #endregion RewardedVideos


#if mopub_native_beta
    #region NativeAds


    public static void RequestNativeAd(string adUnitId)
    {
        MoPubLog.Log("RequestNativeAd", MoPubLog.AdLogEvent.LoadAttempted);
        AdUnitManager.GetAdUnit(adUnitId).RequestNativeAd();
    }


    #endregion NativeAds
#endif

    #region UserConsent

    /// <summary>
    /// Whether or not this app is allowed to collect Personally Identifiable Information (PII) from the user.
    /// </summary>
    public static bool CanCollectPersonalInfo {
        get { return MoPubManager.MoPubPlatformApi.CanCollectPersonalInfo; }
    }


    /// <summary>
    /// The user's current consent state for the app to collect Personally Identifiable Information (PII).
    /// <see cref="MoPub.Consent.Status"> for the values and their meanings.
    /// </summary>
    public static Consent.Status CurrentConsentStatus {
        get { return MoPubManager.MoPubPlatformApi.CurrentConsentStatus; }
    }


    /// <summary>
    /// Checks to see if a publisher should load and then show a consent dialog.
    /// </summary>
    public static bool ShouldShowConsentDialog {
        get { return MoPubManager.MoPubPlatformApi.ShouldShowConsentDialog; }
    }


    /// <summary>
    /// Sends off an asynchronous network request to load the MoPub consent dialog. The two possible resulting events
    /// are <see cref="MoPubManager.OnConsentDialogLoadedEvent"/> and
    /// <see cref="MoPubManager.OnConsentDialogFailedEvent"/>.
    /// </summary>
    public static void LoadConsentDialog()
    {
        if (IsGdprApplicable ?? false) {
            MoPubLog.Log("LoadConsentDialog", MoPubLog.ConsentLogEvent.LoadAttempted);
            MoPubManager.MoPubPlatformApi.LoadConsentDialog();
        } else {
            MoPubManager.Instance.EmitConsentDialogFailedEvent(MoPubUtils.EncodeArgs("GDPR does not apply."));
        }
    }


    /// <summary>
    /// Flag indicating whether the MoPub consent dialog is currently loaded and showable.
    /// </summary>
    public static bool IsConsentDialogReady {
        get { return MoPubManager.MoPubPlatformApi.IsConsentDialogReady; }
    }


    /// <summary>
    /// If the MoPub consent dialog is loaded, this will take over the screen and show it.
    /// </summary>
    public static void ShowConsentDialog()
    {
        MoPubLog.Log("ShowConsentDialog", MoPubLog.ConsentLogEvent.ShowAttempted);
        MoPubManager.MoPubPlatformApi.ShowConsentDialog();
    }


    /// <summary>
    /// Flag indicating whether data collection is subject to the General Data Protection Regulation (GDPR).
    /// </summary>
    /// <returns>
    /// True for Yes, False for No, Null for Unknown (from startup until server responds during SDK initialization).
    /// </returns>
    public static bool? IsGdprApplicable {
        get { return MoPubManager.MoPubPlatformApi.IsGdprApplicable; }
    }


    /// <summary>
    /// Forces the SDK to treat this app as in a GDPR region. Setting this will permanently force GDPR rules for this
    /// user unless this app is uninstalled or the data for this app is cleared.
    /// </summary>
    public static void ForceGdprApplicable() {
        MoPubManager.MoPubPlatformApi.ForceGdprApplicable();
    }


    /// <summary>
    /// API calls to be used by whitelisted publishers who are implementing their own consent dialog.
    /// </summary>
    public static class PartnerApi
    {


        /// <summary>
        /// Notifies the MoPub SDK that this user has granted consent to this app.
        /// </summary>
        public static void GrantConsent()
        {
            MoPubManager.MoPubPlatformApi.GrantConsent();
        }


        /// <summary>
        /// Notifies the MoPub SDK that this user has denied consent to this app.
        /// </summary>
        public static void RevokeConsent()
        {
            MoPubManager.MoPubPlatformApi.RevokeConsent();
        }


        /// <summary>
        /// The URL for the privacy policy this user has consented to.
        /// </summary>
        public static Uri CurrentConsentPrivacyPolicyUrl {
            get {
                return MoPubUtils.UrlFromString(MoPubManager.MoPubPlatformApi.CurrentConsentPrivacyPolicyUrl);
            }
        }

        /// <summary>
        /// The URL for the list of vendors this user has consented to.
        /// </summary>
        public static Uri CurrentVendorListUrl {
            get {
                return MoPubUtils.UrlFromString(MoPubManager.MoPubPlatformApi.CurrentVendorListUrl);
            }
        }


        /// <summary>
        /// The list of vendors this user has consented to in IAB format.
        /// </summary>
        public static string CurrentConsentIabVendorListFormat {
            get { return MoPubManager.MoPubPlatformApi.CurrentConsentIabVendorListFormat; }
        }


        /// <summary>
        /// The version for the privacy policy this user has consented to.
        /// </summary>
        public static string CurrentConsentPrivacyPolicyVersion {
            get { return MoPubManager.MoPubPlatformApi.CurrentConsentPrivacyPolicyVersion; }
        }


        /// <summary>
        /// The version for the list of vendors this user has consented to.
        /// </summary>
        public static string CurrentConsentVendorListVersion {
            get { return MoPubManager.MoPubPlatformApi.CurrentConsentVendorListVersion; }
        }


        /// <summary>
        /// The list of vendors this user has previously consented to in IAB format.
        /// </summary>
        public static string PreviouslyConsentedIabVendorListFormat {
            get { return MoPubManager.MoPubPlatformApi.PreviouslyConsentedIabVendorListFormat; }
        }


        /// <summary>
        /// The version for the privacy policy this user has previously consented to.
        /// </summary>
        public static string PreviouslyConsentedPrivacyPolicyVersion {
            get { return MoPubManager.MoPubPlatformApi.PreviouslyConsentedPrivacyPolicyVersion; }
        }


        /// <summary>
        /// The version for the vendor list this user has previously consented to.
        /// </summary>
        public static string PreviouslyConsentedVendorListVersion {
            get { return MoPubManager.MoPubPlatformApi.PreviouslyConsentedVendorListVersion; }
        }
    }

    #endregion UserConsent


    #region Support Classes

    public enum AdPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        Centered,
        BottomLeft,
        BottomCenter,
        BottomRight
    }


    public static class Consent
    {
        /// <summary>
        /// User's consent for providing personal tracking data for ad tailoring.
        /// </summary>
        /// <remarks>
        /// The enum values match the iOS SDK enum.
        /// </remarks>
        public enum Status
        {
            /// <summary>
            /// Status is unknown. Either the status is currently updating or the SDK initialization has not completed.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Consent is denied.
            /// </summary>
            Denied,

            /// <summary>
            /// Advertiser tracking is disabled.
            /// </summary>
            DoNotTrack,

            /// <summary>
            /// Your app has attempted to grant consent on the user's behalf, but your whitelist status is not verfied
            /// with the ad server.
            /// </summary>
            PotentialWhitelist,

            /// <summary>
            /// User has consented.
            /// </summary>
            Consented
        }


        // The Android SDK uses these strings to indicate consent status.
        private static class Strings
        {
            public const string ExplicitYes = "explicit_yes";
            public const string ExplicitNo = "explicit_no";
            public const string Unknown = "unknown";
            public const string PotentialWhitelist = "potential_whitelist";
            public const string Dnt = "dnt";
        }


        // Helper string to convert Android SDK consent status strings to our consent enum.
        // Also handles integer values.
        public static Status FromString(string status)
        {
            switch (status) {
                case Strings.ExplicitYes:
                    return Status.Consented;
                case Strings.ExplicitNo:
                    return Status.Denied;
                case Strings.Dnt:
                    return Status.DoNotTrack;
                case Strings.PotentialWhitelist:
                    return Status.PotentialWhitelist;
                case Strings.Unknown:
                    return Status.Unknown;
                default:
                    try {
                        return (Status) Enum.Parse(typeof(Status), status);
                    }
                    catch {
                        Debug.LogError("Unknown consent status string: " + status);
                        return Status.Unknown;
                    }
            }
        }
    }


    /// <summary>
    /// The maximum size, in density-independent pixels (DIPs), an ad should have.
    /// </summary>
    public enum MaxAdSize
    {
        Width300Height50,
        Width300Height250,
        Width320Height50,
        Width336Height280,
        Width468Height60,
        Width728Height90,
        Width970Height90,
        Width970Height250,
        ScreenWidthHeight50,
        ScreenWidthHeight90,
        ScreenWidthHeight250,
        ScreenWidthHeight280
    }


    public enum LogLevel
    {
        Debug = 20,
        Info = 30,
        None = 70
    }


    /// <summary>
    /// Data object holding any SDK initialization parameters.
    /// </summary>
    public class SdkConfiguration
    {
        /// <summary>
        /// Any ad unit that your app uses.
        /// </summary>
        public string AdUnitId;

        /// <summary>
        /// Used for rewarded video initialization. This holds each custom event's unique settings.
        /// </summary>
        public MediatedNetwork[] MediatedNetworks;

        /// <summary>
        /// Allow supported SDK networks to collect user information on the basis of legitimate interest.
        /// Can also be set via MoPub.<see cref="MoPub.SdkConfiguration"/> on
        /// MoPub.<see cref="MoPubUnityEditor.InitializeSdk(MoPub.SdkConfiguration)"/>
        /// </summary>
        public bool AllowLegitimateInterest;

        /// <summary>
        /// MoPub SDK log level. Defaults to MoPub.<see cref="MoPub.LogLevel.None"/>
        /// </summary>
        public LogLevel LogLevel
        {
            get { return _logLevel != 0 ? _logLevel : LogLevel.None; }
            set { _logLevel = value; }
        }

        private LogLevel _logLevel;


        public string AdditionalNetworksString
        {
            get {
                var cn = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n is MediatedNetwork && !(n is SupportedNetwork)
                         where !String.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n.AdapterConfigurationClassName;
                return String.Join(",", cn.ToArray());
            }
        }


        public string NetworkConfigurationsJson
        {
            get {
                var nc = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.NetworkConfiguration != null
                         where !String.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n;
                return MJ.Json.Serialize(nc.ToDictionary(n => n.AdapterConfigurationClassName,
                                                      n => n.NetworkConfiguration));
            }
        }

        public string MediationSettingsJson
        {
            get {
                var ms = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.MediationSettings != null
                         where !String.IsNullOrEmpty(n.MediationSettingsClassName)
                         select n;
                return MJ.Json.Serialize(ms.ToDictionary(n => n.MediationSettingsClassName,
                                                      n => n.MediationSettings));
            }
        }


        public string MoPubRequestOptionsJson
        {
            get {
                var ro = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.MoPubRequestOptions != null
                         where !String.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n;
                return MJ.Json.Serialize(ro.ToDictionary(n => n.AdapterConfigurationClassName,
                                                      n => n.MoPubRequestOptions));
            }
        }


        // Allow looking up an entry in the MediatedNetwork array using the network name, which is presumed to be
        // part of the AdapterConfigurationClassName value.
        public MediatedNetwork this[string networkName]
        {
            get {
                return MediatedNetworks.FirstOrDefault(mn =>
                    mn.AdapterConfigurationClassName == networkName ||
                    mn.AdapterConfigurationClassName == networkName + "AdapterConfiguration" ||
                    mn.AdapterConfigurationClassName.EndsWith("." + networkName) ||
                    mn.AdapterConfigurationClassName.EndsWith("." + networkName + "AdapterConfiguration"));
            }
        }
    }


    public class LocalMediationSetting : Dictionary<string, object>
    {
        public string MediationSettingsClassName { get; set; }

        public LocalMediationSetting() { }

        public LocalMediationSetting(string adVendor)
        {
#if UNITY_IOS
            MediationSettingsClassName = adVendor + "InstanceMediationSettings";
#else
            MediationSettingsClassName = "com.mopub.mobileads." + adVendor + "RewardedVideo$" + adVendor + "MediationSettings";
#endif
        }

        public LocalMediationSetting(string android, string ios) :
#if UNITY_IOS
            this(ios)
#else
            this(android)
#endif
            {}


        public static string ToJson(IEnumerable<LocalMediationSetting> localMediationSettings)
        {
            var ms = from n in localMediationSettings ?? Enumerable.Empty<LocalMediationSetting>()
                     where n != null && !String.IsNullOrEmpty(n.MediationSettingsClassName)
                     select n;
            return MJ.Json.Serialize(ms.ToDictionary(n => n.MediationSettingsClassName, n => n));
        }


        // Shortcut class names so you don't have to remember the right ad vendor string (also to not misspell it).
        public class AdColony : LocalMediationSetting { public AdColony() : base("AdColony") {
#if UNITY_ANDROID
                MediationSettingsClassName = "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyInstanceMediationSettings";
#endif
            }
        }
        public class AdMob      : LocalMediationSetting { public AdMob()      : base(android: "GooglePlayServices",
                                                                                     ios:     "MPGoogle") { } }
        public class Chartboost : LocalMediationSetting { public Chartboost() : base("Chartboost") { } }
        public class Vungle     : LocalMediationSetting { public Vungle()     : base("Vungle") { } }
    }


    // Networks that are supported by MoPub.
    public class SupportedNetwork : MediatedNetwork
    {
        protected SupportedNetwork(string adVendor)
        {
#if UNITY_IOS
            AdapterConfigurationClassName = adVendor + "AdapterConfiguration";
            MediationSettingsClassName    = adVendor + "GlobalMediationSettings";
#else
            AdapterConfigurationClassName = "com.mopub.mobileads." + adVendor + "AdapterConfiguration";
            MediationSettingsClassName    = "com.mopub.mobileads." + adVendor + "RewardedVideo$" + adVendor + "MediationSettings";
#endif
        }

        public class AdColony   : SupportedNetwork { public AdColony()   : base("AdColony") {
#if UNITY_ANDROID
               MediationSettingsClassName = "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyGlobalMediationSettings";
#endif
            }
        }
        public class AdMob      : SupportedNetwork { public AdMob()      : base("GooglePlayServices") {
#if UNITY_IOS
               AdapterConfigurationClassName = "GoogleAdMobAdapterConfiguration";
               MediationSettingsClassName    = "MPGoogleGlobalMediationSettings";
#endif
            }
        }
        public class AppLovin   : SupportedNetwork { public AppLovin()   : base("AppLovin") { } }
        public class Chartboost : SupportedNetwork { public Chartboost() : base("Chartboost") { } }
        public class Facebook   : SupportedNetwork { public Facebook()   : base("Facebook") { } }
        public class Flurry     : SupportedNetwork { public Flurry()     : base("Flurry") { } }
        public class IronSource : SupportedNetwork { public IronSource() : base("IronSource") { } }
        public class Pangle     : SupportedNetwork { public Pangle()     : base("Pangle") { } }
        public class Snap       : SupportedNetwork { public Snap()       : base("SnapAd") { } }
        public class Tapjoy     : SupportedNetwork { public Tapjoy()     : base("Tapjoy") { } }
        public class Unity      : SupportedNetwork { public Unity()      : base("UnityAds") { } }
        public class Verizon    : SupportedNetwork { public Verizon()    : base("Verizon") { } }
        public class Vungle     : SupportedNetwork { public Vungle()     : base("Vungle") { } }
    }


    public struct Reward
    {
        public string Label;
        public int Amount;


        public override string ToString()
        {
            return String.Format("\"{0} {1}\"", Amount, Label);
        }


        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Label) && Amount > 0;
        }
    }


    public struct ImpressionData
    {
        public string AppVersion;
        public string AdUnitId;
        public string AdUnitName;
        public string AdUnitFormat;
        public string ImpressionId;
        public string Currency;
        public double? PublisherRevenue;
        public string AdGroupId;
        public string AdGroupName;
        public string AdGroupType;
        public int? AdGroupPriority;
        public string Country;
        public string Precision;
        public string NetworkName;
        public string NetworkPlacementId;
        public string JsonRepresentation;

        public static ImpressionData FromJson(string json)
        {
            var impData = new ImpressionData();
            if (string.IsNullOrEmpty(json)) return impData;

            var fields = MJ.Json.Deserialize(json) as Dictionary<string, object>;
            if (fields == null) return impData;

            object obj;
            double parsedDouble;
            int parsedInt;

            if (fields.TryGetValue("app_version", out obj) && obj != null)
                impData.AppVersion = obj.ToString();

            if (fields.TryGetValue("adunit_id", out obj) && obj != null)
                impData.AdUnitId = obj.ToString();

            if (fields.TryGetValue("adunit_name", out obj) && obj != null)
                impData.AdUnitName = obj.ToString();

            if (fields.TryGetValue("adunit_format", out obj) && obj != null)
                impData.AdUnitFormat = obj.ToString();

            if (fields.TryGetValue("id", out obj) && obj != null)
                impData.ImpressionId = obj.ToString();

            if (fields.TryGetValue("currency", out obj) && obj != null)
                impData.Currency = obj.ToString();

            if (fields.TryGetValue("publisher_revenue", out obj) && obj != null
                && double.TryParse(MoPubUtils.InvariantCultureToString(obj), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out parsedDouble))
                impData.PublisherRevenue = parsedDouble;

            if (fields.TryGetValue("adgroup_id", out obj) && obj != null)
                impData.AdGroupId = obj.ToString();

            if (fields.TryGetValue("adgroup_name", out obj) && obj != null)
                impData.AdGroupName = obj.ToString();

            if (fields.TryGetValue("adgroup_type", out obj) && obj != null)
                impData.AdGroupType = obj.ToString();

            if (fields.TryGetValue("adgroup_priority", out obj) && obj != null
                && int.TryParse(MoPubUtils.InvariantCultureToString(obj), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out parsedInt))
                impData.AdGroupPriority = parsedInt;

            if (fields.TryGetValue("country", out obj) && obj != null)
                impData.Country = obj.ToString();

            if (fields.TryGetValue("precision", out obj) && obj != null)
                impData.Precision = obj.ToString();

            if (fields.TryGetValue("network_name", out obj) && obj != null)
                impData.NetworkName = obj.ToString();

            if (fields.TryGetValue("network_placement_id", out obj) && obj != null)
                impData.NetworkPlacementId = obj.ToString();

            impData.JsonRepresentation = json;

            return impData;
        }
    }

    #endregion


    /// <summary>
    /// Set this to an ISO language code (e.g., "en-US") if you wish the next two URL properties to point
    /// to a web resource that is localized to a specific language.
    /// </summary>
    public static string ConsentLanguageCode { get; set; }

    public const double LatLongSentinel = 99999.0;

    private static string _pluginName;
    private static bool _validInit;
    private static bool _allowLegitimateInterest;

    public static string PluginName {
        get { return _pluginName ?? (_pluginName = "MoPub Unity Plugin v" + MoPubSdkVersion); }
    }


    #region Internal


    private static void LoadPluginsForAdUnits(string[] adUnitIds, string adType = null)
    {
        AdUnitManager.InitAdUnits(adUnitIds, adType);

        Debug.LogFormat("{0} {1} AdUnits loaded for plugins:\n{2}", adUnitIds.Length, adType ?? "",
            string.Join(", ", adUnitIds));
    }


    private static void ValidateAdUnitForSdkInit(string adUnitId)
    {
        _validInit = !string.IsNullOrEmpty(adUnitId);
        ValidateInit("FATAL ERROR: A valid ad unit ID is needed to initialize the MoPub SDK.\n" +
                     "Please enter an ad unit ID from your app into the MoPubManager GameObject.");
    }


    private static void ValidateInit(string message = null)
    {
        if (_validInit) return;

        message = message ?? "FATAL ERROR: MoPub SDK has not been successfully initialized.";
        Debug.LogError(message);
        throw new Exception("0xDEADDEAD");
    }


    private static void ReportAdUnitNotFound(string adUnitId)
    {
        Debug.LogWarning(string.Format("AdUnit {0} not found: no plugin was initialized", adUnitId));
    }


    static MoPub()
    {
        InitManager();
    }


    // Allocate the MoPubManager singleton, which receives all callback events from the native SDKs.
    // This is done in case the app is not using the new MoPubManager prefab, for backwards compatibility.
    private static void InitManager()
    {
        if (MoPubManager.Instance == null)
            new GameObject("MoPubManager", typeof(MoPubManager));
    }



    public static LogLevel CachedLogLevel;

    private static class AdUnitManager
    {
        private static readonly Dictionary<string, MoPubAdUnit> AdUnits = new Dictionary<string, MoPubAdUnit>();

        public static void InitAdUnits(string[] adUnitIds, string adType)
        {
            foreach (var adUnitId in adUnitIds) AdUnits[adUnitId] = MoPubAdUnit.CreateMoPubAdUnit(adUnitId, adType);
        }

        public static void InitAdUnits(string adType, params string[] adUnitIds)
        {
            InitAdUnits(adUnitIds, adType);
        }

        public static MoPubAdUnit GetAdUnit(string adUnitId)
        {
            ValidateInit();
            MoPubAdUnit adUnit;
            if (AdUnits.TryGetValue(adUnitId, out adUnit))
                return adUnit;

            ReportAdUnitNotFound(adUnitId);
            return MoPubAdUnit.NullMoPubAdUnit;
        }
    }


    #endregion Internal


    #region Legacy


    /// <summary>
    /// This method is just here for legacy support and will be deprecated soon.
    /// Please use <see cref="MoPubUtils.CompareVersions(string,string)"/> instead.
    /// </summary>
    public static int CompareVersions(string a, string b)
    {
        return MoPubUtils.CompareVersions(a, b);
    }


    #endregion Legacy
}
