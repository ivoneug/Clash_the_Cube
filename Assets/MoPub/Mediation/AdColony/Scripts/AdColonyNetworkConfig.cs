#if mopub_manager
using UnityEngine;

public class AdColonyNetworkConfig : MoPubNetworkConfig
{
    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.AdColonyAdapterConfiguration"
                  : "AdColonyAdapterConfiguration"; }
    }

    public override string MediationSettingsClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyGlobalMediationSettings"
                  : "AdColonyGlobalMediationSettings"; }
    }

    [Tooltip("Enter your app ID to be used to initialize the AdColony SDK.")]
    [Config.Optional]
    public PlatformSpecificString appId;

    [Tooltip("Enter an Array of zone IDs to be used to initialize the AdColony SDK.")]
    [Config.Optional]
    public PlatformSpecificStrings allZoneIds;

    [Tooltip("Enter additional options to be used to initialize the AdColony SDK.")]
    [Config.Optional]
    public PlatformSpecificString clientOptions;
}
#endif
