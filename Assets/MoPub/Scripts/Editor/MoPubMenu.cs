using UnityEditor;
using UnityEngine;

public class MoPubMenu
{
#if !mopub_disable_menu

    [MenuItem("MoPub/About MoPub SDK", false, 0)]
    public static void About()
    {
        MoPubAboutDialog.ShowDialog();
    }

    [MenuItem("MoPub/Documentation...", false, 1)]
    public static void Documentation()
    {
        Application.OpenURL("https://developers.mopub.com/docs/unity/");
    }

    [MenuItem("MoPub/Report Issue...", false, 2)]
    public static void ReportIssue()
    {
        Application.OpenURL("https://github.com/mopub/mopub-unity-sdk/issues");
    }

#if mopub_developer
    [MenuItem("MoPub/Manage SDKs (Staging)...", false, 3)]
    public static void SdkManagerStaging()
    {
        MoPubSDKManager.ShowSDKManager(stage:true);
    }
#endif

    [MenuItem("MoPub/Manage SDKs...", false, 4)]
    public static void SdkManagerProd()
    {
        MoPubSDKManager.ShowSDKManager(stage:false);
    }

#endif // mopub_disable_menu
}
