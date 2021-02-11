using System.Collections;
using MJ = MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;
#if UNITY_EDITOR
using System;

/// <summary>
/// Bridge between the MoPub Unity Instance-wide API and In-Editor implementation.
/// </summary>
/// <para>
/// Publishers integrating with MoPub should make all calls through the <see cref="MoPub"/> class, and handle any
/// desired MoPub Events from the <see cref="MoPubManager"/> class.
/// </para>
/// <para>
/// For other platform-specific implementations, see <see cref="MoPubAndroid"/> and <see cref="MoPubiOS"/>.
/// </para>
/// <remarks>
/// Some properties have added public setters in order to facilitate testing in Play mode.
/// </remarks>
internal class MoPubUnityEditor : MoPubPlatformApi
{
    #region SdkSetup

    private static bool _isInitialized;

    private bool _isGdprApplicable;


    internal override void InitializeSdk(MoPub.SdkConfiguration sdkConfiguration)
    {
        WaitOneFrame(() => {
            _isInitialized = true;
            MoPubManager.Instance.EmitSdkInitializedEvent(ArgsToJson(sdkConfiguration.AdUnitId,
                                                              ((int) sdkConfiguration.LogLevel).ToString()));
        });
    }


    internal override void EnableLocationSupport(bool shouldUseLocation) { }


    internal override void SetEngineInformation(string engineName, string engineVersion) { }


    internal override void ReportApplicationOpen(string iTunesAppId = null) { }


    internal override void OnApplicationPause(bool paused)
    {
        MoPubManager.EmitConsentDialogDismissedIfApplicable(paused);
    }


    internal override void DisableViewability()
    {
        Debug.Log("Viewability measurement would now be disabled on a real device.");
    }


    internal override bool AllowLegitimateInterest { get; set; }


    internal override string SdkName
    {
        get { return "no SDK loaded (not on a mobile device)"; }
    }


    internal override bool IsSdkInitialized
    {
        get { return _isInitialized; }
    }


    internal override MoPub.LogLevel SdkLogLevel { get; set; }


    #endregion SdkSetup


    #region AndroidOnly


    internal override void AddFacebookTestDeviceId(string hashedDeviceId) { }


    #endregion AndroidOnly


    #region iOSOnly


    internal override void ForceWKWebView(bool shouldForce) { }


    #endregion iOSOnly


    #region UserConsent


    internal override bool CanCollectPersonalInfo
    {
        get { return false; }
        set { }
    }


    private static MoPub.Consent.Status _currentConsentStatus = MoPub.Consent.Status.Unknown;

    internal override MoPub.Consent.Status CurrentConsentStatus
    {
        get { return _currentConsentStatus; }
        set {
            if (value == _currentConsentStatus)
                return;
            WaitOneFrame(() => {
                var old = _currentConsentStatus;
                _currentConsentStatus = value;
                MoPubManager.Instance.EmitConsentStatusChangedEvent(
                    ArgsToJson(old.ToString(), value.ToString(), CanCollectPersonalInfo ? "true" : "false"));
            });
        }
    }


    internal override bool ShouldShowConsentDialog
    {
        get {
            // Note: the logic below is for testing purposes.  It does not reflect the runtime production logic
            // (in particular, the call to a PartnerApi method.)
            return (IsGdprApplicable ?? false) && _currentConsentStatus == MoPub.Consent.Status.Unknown;
        }
    }


    public void SetShouldShowConsentDialog(bool isGdprApplicable) {
        // HACK for testing purposes only.  Use it to reinitialize an unknown consent state, or to skip the consent dialog.
        _isGdprApplicable = isGdprApplicable;
        _currentConsentStatus = isGdprApplicable ? MoPub.Consent.Status.Unknown : MoPub.Consent.Status.Consented;
    }


    internal override void LoadConsentDialog()
    {
        WaitOneFrame(() => {
            IsConsentDialogReady = true;
            MoPubManager.Instance.EmitConsentDialogLoadedEvent();
        });
    }


    internal override bool IsConsentDialogReady { get; /* Testing: */ set; }


    internal override void ShowConsentDialog()
    {
        if (!IsConsentDialogReady) {
            Debug.LogError("Called ShowConsentDialog before consent dialog loaded!");
            return;
        }
        WaitOneFrame(() => {
            Debug.Log("When running on a mobile device, the consent dialog would appear now.");
            MoPubManager.Instance.EmitConsentDialogShownEvent();
            SimulateApplicationResume();
        });
    }


    internal override bool? IsGdprApplicable
    {
        get { return _isGdprApplicable; }
    }


    internal override void ForceGdprApplicable() { _isGdprApplicable = true; }


    internal override void GrantConsent()
    {
        CurrentConsentStatus = MoPub.Consent.Status.Consented;
        CanCollectPersonalInfo = true;
    }


    internal override void RevokeConsent()
    {
        CurrentConsentStatus = MoPub.Consent.Status.Denied;
        CanCollectPersonalInfo = false;
    }


    internal override string CurrentConsentPrivacyPolicyUrl { get; /* Testing: */ set; }


    internal override string CurrentVendorListUrl { get; /* Testing: */ set; }


    internal override string CurrentConsentIabVendorListFormat { get; /* Testing: */ set; }


    internal override string CurrentConsentPrivacyPolicyVersion { get; /* Testing: */ set; }


    internal override string CurrentConsentVendorListVersion { get; /* Testing: */ set; }


    internal override string PreviouslyConsentedIabVendorListFormat { get; /* Testing: */ set; }


    internal override string PreviouslyConsentedPrivacyPolicyVersion { get; /* Testing: */ set; }


    internal override string PreviouslyConsentedVendorListVersion { get; /* Testing: */ set; }

    #endregion UserConsent


    #region MockEditor


    // Emulate the one-frame delay inherent in calling UnitySendMessage from Java/Objective-C
    // code in our native SDKs.
    private static IEnumerator WaitOneFrameCoroutine(Action action)
    {
        yield return null;
        action();
    }


    public static void WaitOneFrame(Action action)
    {
        MoPubManager.Instance.StartCoroutine(WaitOneFrameCoroutine(action));
    }

    public static void SimulateApplicationResume()
    {
        WaitOneFrame(() => {
            Debug.Log("Simulating application resume.");
            MoPubManager.EmitConsentDialogDismissedIfApplicable(false);
        });
    }


    public static string ArgsToJson(params string[] args)
    {
        return MJ.Json.Serialize(args);
    }


    #endregion MockEditor
}
#endif
