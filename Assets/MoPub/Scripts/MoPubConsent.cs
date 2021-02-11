using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the GDPR consent state and dialog.
/// </summary>
public class MoPubConsent : MonoBehaviour
{
    public const string DefaultLocationAwarenessUsage = "Your location is used to provide more targeted advertising.";

    [Tooltip("If set to true, this script will handle loading and showing the GDPR consent dialog automatically.")]
    public bool AutoShowConsentDialog;

    [Tooltip("(iOS Only) Indicate the reason that your app needs to be location aware.  Required for App Store acceptance.")]
    public string LocationAwarenessUsage;

    // Need these declarations for the events to appear in the inspector.
    [Serializable] public class ConsentStatusChangedEvent : UnityEvent<MoPub.Consent.Status, MoPub.Consent.Status, bool> { }
    [Serializable] public class ConsentDialogLoadedEvent : UnityEvent { }
    [Serializable] public class ConsentDialogFailedEvent : UnityEvent<string> { }
    [Serializable] public class ConsentDialogShownEvent : UnityEvent { }
    [Serializable] public class ConsentDialogDismissedEvent : UnityEvent { }


    [Header("Callbacks")]
    public ConsentStatusChangedEvent ConsentStatusChanged;
    public ConsentDialogLoadedEvent ConsentDialogLoaded;
    public ConsentDialogFailedEvent ConsentDialogFailed;
    public ConsentDialogShownEvent ConsentDialogShown;
    public ConsentDialogDismissedEvent ConsentDialogDismissed;


    void Reset()
    {
        LocationAwarenessUsage = DefaultLocationAwarenessUsage;
    }


    public static string LocationAwarenessUsageDescription
    {
        get {
            if (MoPubManager.Instance != null) {
                var consent = MoPubManager.Instance.GetComponent<MoPubConsent>();
                if (consent != null) {
                    var usage = consent.LocationAwarenessUsage;
                    if (!string.IsNullOrEmpty(usage))
                        return usage;
                }
            }
            return DefaultLocationAwarenessUsage;
        }
    }


    public static bool? IsGdprApplicable
    {
        get { return MoPub.IsGdprApplicable; }
    }


    public static MoPub.Consent.Status CurrentConsentStatus
    {
        get { return MoPub.CurrentConsentStatus; }
    }


    public static bool CanCollectPersonalInfo
    {
        get { return MoPub.CanCollectPersonalInfo; }
    }


    public static bool ShouldShowConsentDialog
    {
        get { return MoPub.ShouldShowConsentDialog; }
    }


    public static bool IsConsentDialogReady
    {
        get { return MoPub.IsConsentDialogReady; }
    }


    public static void LoadConsentDialog()
    {
        MoPub.LoadConsentDialog();
    }


    public static void ShowConsentDialog()
    {
        MoPub.ShowConsentDialog();
    }


    // Forward from the C# events to the unity events.

    private void fwdConsentStatusChanged(MoPub.Consent.Status oldConsent, MoPub.Consent.Status newConsent,
                                         bool canCollectPersonalInfo)
    {
        if (ConsentStatusChanged != null)
            ConsentStatusChanged.Invoke(oldConsent, newConsent, canCollectPersonalInfo);
        ShowConsentDialogIfNeeded();
    }

    private void fwdConsentDialogLoaded()
    {
        if (ConsentDialogLoaded != null)
            ConsentDialogLoaded.Invoke();
        ShowConsentDialogIfNeeded();
    }

    private void fwdConsentDialogFailed(string error)
    {
        if (ConsentDialogFailed != null)
            ConsentDialogFailed.Invoke("Consent dialog failed to load: " + error);
    }

    private void fwdConsentDialogShown()
    {
        if (ConsentDialogShown != null)
            ConsentDialogShown.Invoke();
    }

    private void fwdConsentDialogDismissed()
    {
        if (ConsentDialogDismissed != null)
            ConsentDialogDismissed.Invoke();
    }

    private void OnSdkInitialized(string adUnitId)
    {
        ShowConsentDialogIfNeeded();
    }

    private void ShowConsentDialogIfNeeded()
    {
        if (!AutoShowConsentDialog || !ShouldShowConsentDialog) return;

        if (IsConsentDialogReady)
            ShowConsentDialog();
        else
            LoadConsentDialog();
    }


    void OnEnable()
    {
        MoPubManager.OnConsentStatusChangedEvent += fwdConsentStatusChanged;
        MoPubManager.OnConsentDialogLoadedEvent += fwdConsentDialogLoaded;
        MoPubManager.OnConsentDialogFailedEvent += fwdConsentDialogFailed;
        MoPubManager.OnConsentDialogShownEvent += fwdConsentDialogShown;
        MoPubManager.OnConsentDialogDismissedEvent += fwdConsentDialogDismissed;
        MoPubManager.OnSdkInitializedEvent += OnSdkInitialized;

        if (MoPubManager.Instance != null && MoPub.IsSdkInitialized)
            ShowConsentDialogIfNeeded();
    }


    // Required to get enablement checkbox in the inspector
    void Start() { }


    void OnDisable()
    {
        MoPubManager.OnConsentStatusChangedEvent -= fwdConsentStatusChanged;
        MoPubManager.OnConsentDialogLoadedEvent -= fwdConsentDialogLoaded;
        MoPubManager.OnConsentDialogFailedEvent -= fwdConsentDialogFailed;
        MoPubManager.OnConsentDialogShownEvent -= fwdConsentDialogShown;
        MoPubManager.OnConsentDialogDismissedEvent -= fwdConsentDialogDismissed;
        MoPubManager.OnSdkInitializedEvent -= OnSdkInitialized;
    }
}
