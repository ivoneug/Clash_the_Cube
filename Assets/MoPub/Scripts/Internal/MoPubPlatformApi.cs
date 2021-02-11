/// <summary>
/// Bridge between the MoPub Unity Instance-wide API and platform-specific implementations.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For platform-specific implementations, see
/// <see cref="MoPubUnityEditor"/>, <see cref="MoPubAndroid"/>, and <see cref="MoPubiOS"/>.
/// </para>
internal abstract class MoPubPlatformApi
{


    #region SdkSetup

    internal abstract void InitializeSdk(MoPub.SdkConfiguration sdkConfiguration);

    internal abstract void EnableLocationSupport(bool shouldUseLocation);

    internal abstract void SetEngineInformation(string engineName, string engineVersion);

    internal abstract void ReportApplicationOpen(string iTunesAppId = null);

    internal abstract void OnApplicationPause(bool paused);

    internal abstract void DisableViewability();

    internal abstract string SdkName { get; }

    internal abstract bool IsSdkInitialized { get; }

    internal abstract MoPub.LogLevel SdkLogLevel { get; set; }

    internal abstract bool AllowLegitimateInterest { get; set; }

    #endregion SdkSetup

    #region AndroidOnly

    internal abstract void AddFacebookTestDeviceId(string hashedDeviceId);

    #endregion AndroidOnly

    #region iOSOnly

    internal abstract void ForceWKWebView(bool shouldForce);

    #endregion iOSOnly

    #region UserConsent

    internal abstract bool CanCollectPersonalInfo { get; /* Testing: */ set; }

    internal abstract MoPub.Consent.Status CurrentConsentStatus { get; /* Testing: */ set; }

    internal abstract bool ShouldShowConsentDialog { get; }

    internal abstract void LoadConsentDialog();

    internal abstract bool IsConsentDialogReady { get; /* Testing: */ set; }

    internal abstract void ShowConsentDialog();

    internal abstract bool? IsGdprApplicable { get; }

    internal abstract void ForceGdprApplicable();

    internal abstract void GrantConsent();

    internal abstract void RevokeConsent();

    internal abstract string CurrentConsentPrivacyPolicyUrl { get; /* Testing: */ set;  }

    internal abstract string CurrentVendorListUrl { get; /* Testing: */ set; }

    internal abstract string CurrentConsentIabVendorListFormat { get; /* Testing: */ set; }

    internal abstract string CurrentConsentPrivacyPolicyVersion { get; /* Testing: */ set; }

    internal abstract string CurrentConsentVendorListVersion { get; /* Testing: */ set; }

    internal abstract string PreviouslyConsentedIabVendorListFormat { get; /* Testing: */ set; }

    internal abstract string PreviouslyConsentedPrivacyPolicyVersion { get; /* Testing: */ set; }

    internal abstract string PreviouslyConsentedVendorListVersion { get; /* Testing: */ set; }

    #endregion UserConsent
}
