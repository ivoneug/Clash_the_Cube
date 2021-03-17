using System.Collections.Generic;

public class AdColonyPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AdColony"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.2.6"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"4.4.1" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"4.5.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.AdColony" },
                { Platform.IOS, "AdColony" }
            };
        }
    }
}
