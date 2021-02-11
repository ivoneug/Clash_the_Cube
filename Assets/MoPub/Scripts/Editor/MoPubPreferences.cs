using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class MoPubPreferences
{
    public const string MoPubMenuOffDefine = "mopub_disable_menu";

    public const string MoPubNativeAdsDefine = "mopub_native_beta";


    // Return whether the specified compilation define is enabled in the given target group.
    private static bool IsDefined(string entry, BuildTargetGroup group)
    {
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Contains(entry);
    }


    private static void UpdateDefines(string entry, bool enabled, BuildTargetGroup group)
    {
        // First remove any and all instances of the entry.
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                                    .Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(d => d != entry);
        // Now add the entry back to the list (once) if it has been enabled.
        if (enabled)
            defines = defines.Concat(new[] {entry});
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
    }


    [PreferenceItem("MoPub")]
    public static void PreferencesGui()
    {
        EditorGUILayout.LabelField("Per Project Settings");
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        var enableMenu = !IsDefined(MoPubMenuOffDefine, BuildTargetGroup.Android)
                      // These are supposed to be in sync, but just in case they aren't...
                      && !IsDefined(MoPubMenuOffDefine, BuildTargetGroup.iOS);
        enableMenu = EditorGUILayout.ToggleLeft(new GUIContent {
                         text = "Enable MoPub menu",
                         tooltip = "Adds a MoPub menu to the main menubar.  " +
                                   "\n\nNOTE: When disabling, the MoPub main menu bar entry will remain until " +
                                   "Unity does an asset refresh."
                     }, enableMenu);
        if (EditorGUI.EndChangeCheck()) {
            UpdateDefines(MoPubMenuOffDefine, !enableMenu, BuildTargetGroup.Android);
            UpdateDefines(MoPubMenuOffDefine, !enableMenu, BuildTargetGroup.iOS);
        }

        EditorGUI.BeginChangeCheck();
        var enableNativeAds = IsDefined(MoPubNativeAdsDefine, BuildTargetGroup.Android);
        enableNativeAds = EditorGUILayout.ToggleLeft(new GUIContent {
                              text = "Enable MoPub native ads (BETA)",
                              tooltip = "Enables the MoPub Native Ads SDK (Android only)."
                          }, enableNativeAds);
        if (EditorGUI.EndChangeCheck())
            UpdateDefines(MoPubNativeAdsDefine, enableNativeAds, BuildTargetGroup.Android);
    }
}
