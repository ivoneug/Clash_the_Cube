using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Bridge between the MoPub Unity Instance-wide API and Android implementation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For other platform-specific implementations, see <see cref="MoPubUnityEditor"/> and <see cref="MoPubiOS"/>.
/// </para>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal class MoPubAndroid : MoPubPlatformApi
{
    private static readonly AndroidJavaClass PluginClass = new AndroidJavaClass("com.mopub.unity.MoPubUnityPlugin");

    #region SdkSetup


    internal override void InitializeSdk(MoPub.SdkConfiguration sdkConfiguration)
    {
        PluginClass.CallStatic(
            "initializeSdk", sdkConfiguration.AdUnitId,
            sdkConfiguration.AdditionalNetworksString,
            sdkConfiguration.MediationSettingsJson,
            sdkConfiguration.AllowLegitimateInterest,
            (int) sdkConfiguration.LogLevel,
            sdkConfiguration.NetworkConfigurationsJson,
            sdkConfiguration.MoPubRequestOptionsJson,
            MoPubManager.BackgroundEventListener.Instance);
    }


    internal override void EnableLocationSupport(bool shouldUseLocation)
    {
        PluginClass.CallStatic("setLocationAwareness", shouldUseLocation ?
            LocationAwareness.NORMAL.ToString() : LocationAwareness.DISABLED.ToString());
    }


    internal override void SetEngineInformation(string engineName, string engineVersion)
    {
        PluginClass.CallStatic("setEngineInformation", engineName, engineVersion);
    }


    internal override void ReportApplicationOpen(string iTunesAppId = null)
    {
        PluginClass.CallStatic("reportApplicationOpen");
    }


    internal override void OnApplicationPause(bool paused)
    {
        PluginClass.CallStatic("onApplicationPause", paused);
        MoPubManager.EmitConsentDialogDismissedIfApplicable(paused);
    }


    internal override void DisableViewability()
    {
        PluginClass.CallStatic("disableViewability");
    }


    internal override bool AllowLegitimateInterest
    {
        get { return PluginClass.CallStatic<bool>("shouldAllowLegitimateInterest"); }
        set { PluginClass.CallStatic("setAllowLegitimateInterest", value); }
    }


    internal override string SdkName
    {
        get { return "Android SDK v" + PluginClass.CallStatic<string>("getSDKVersion"); }
    }


    internal override bool IsSdkInitialized
    {
        get { return PluginClass.CallStatic<bool>("isSdkInitialized"); }
    }


    internal override MoPub.LogLevel SdkLogLevel
    {
        get { return (MoPub.LogLevel) PluginClass.CallStatic<int>("getLogLevel"); }
        set { PluginClass.CallStatic("setLogLevel", (int) value); }
    }


    #endregion SdkSetup


    #region AndroidOnly


    /// <summary>
    /// The different kinds of location awareness that can be enabled for the SDK.
    /// </summary>
    /// <remarks>These enum names need to match the ones in the MoPub Android SDK, since we pass them by string value.
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LocationAwareness
    {
        TRUNCATED,
        DISABLED,
        NORMAL
    }


    internal override void AddFacebookTestDeviceId(string hashedDeviceId)
    {
        PluginClass.CallStatic("addFacebookTestDeviceId", hashedDeviceId);
    }


    #endregion AndroidOnly


    #region iOSOnly


    internal override void ForceWKWebView(bool shouldForce)
    {
        Debug.LogWarning("Attempted to call iOS-only method from Android device.");
    }


    #endregion iOSOnly


    #region UserConsent

    internal override bool CanCollectPersonalInfo
    {
        get { return PluginClass.CallStatic<bool>("canCollectPersonalInfo"); }
        set { }
    }


    internal override MoPub.Consent.Status CurrentConsentStatus
    {
        get { return MoPub.Consent.FromString(PluginClass.CallStatic<string>("getPersonalInfoConsentState")); }
        set { }
    }


    internal override bool ShouldShowConsentDialog
    {
        get { return PluginClass.CallStatic<bool>("shouldShowConsentDialog"); }
    }


    internal override void LoadConsentDialog()
    {
        PluginClass.CallStatic("loadConsentDialog");
    }


    internal override bool IsConsentDialogReady
    {
        get { return PluginClass.CallStatic<bool>("isConsentDialogReady"); }
        set { }
    }


    internal override void ShowConsentDialog()
    {
        PluginClass.CallStatic("showConsentDialog");
    }


    internal override bool? IsGdprApplicable {
        get {
            var gdpr = PluginClass.CallStatic<int>("gdprApplies");
            return gdpr == 0 ? null : gdpr > 0 ? (bool?) true : false;
        }
    }


    internal override void ForceGdprApplicable() {
        PluginClass.CallStatic("forceGdprApplies");
    }


    internal override void GrantConsent()
    {
        PluginClass.CallStatic("grantConsent");
    }


    internal override void RevokeConsent()
    {
        PluginClass.CallStatic("revokeConsent");
    }


    internal override string CurrentConsentPrivacyPolicyUrl
    {
        get {
            return PluginClass.CallStatic<string>("getCurrentPrivacyPolicyLink", MoPub.ConsentLanguageCode);
        }
        set { }
    }


    internal override string CurrentVendorListUrl
    {
        get {
            return PluginClass.CallStatic<string>("getCurrentVendorListLink", MoPub.ConsentLanguageCode);
        }
        set { }
    }


    internal override string CurrentConsentIabVendorListFormat
    {
        get { return PluginClass.CallStatic<string>("getCurrentVendorListIabFormat"); }
        set { }
    }


    internal override string CurrentConsentPrivacyPolicyVersion
    {
        get { return PluginClass.CallStatic<string>("getCurrentPrivacyPolicyVersion"); }
        set { }
    }


    internal override string CurrentConsentVendorListVersion
    {
        get { return PluginClass.CallStatic<string>("getCurrentVendorListVersion"); }
        set { }
    }


    internal override string PreviouslyConsentedIabVendorListFormat
    {
        get { return PluginClass.CallStatic<string>("getConsentedVendorListIabFormat"); }
        set { }
    }


    internal override string PreviouslyConsentedPrivacyPolicyVersion
    {
        get { return PluginClass.CallStatic<string>("getConsentedPrivacyPolicyVersion"); }
        set { }
    }


    internal override string PreviouslyConsentedVendorListVersion
    {
        get { return PluginClass.CallStatic<string>("getConsentedVendorListVersion"); }
        set { }
    }

    #endregion UserConsent
}
