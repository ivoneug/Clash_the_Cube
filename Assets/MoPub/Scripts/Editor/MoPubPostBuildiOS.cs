#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

// ReSharper disable once CheckNamespace
namespace MoPubInternal.Editor.Postbuild
{
    public static class MoPubPostBuildiOS
    {
        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            PrepareProject(buildPath);

            // Make sure that the proper location usage string is in Info.plist

            const string locationKey = "NSLocationWhenInUseUsageDescription";

            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElement element = plist.root[locationKey];
            var usage = MoPubConsent.LocationAwarenessUsageDescription;
            // Add or overwrite the key in the info.plist file if necessary.
            // (Note:  does not overwrite if the string has been manually changed in the Xcode project and our string is just the default.)
            if (element == null || usage != element.AsString() && usage != MoPubConsent.DefaultLocationAwarenessUsage) {
                plist.root.SetString(locationKey, usage);
                plist.WriteToFile(plistPath);
            }
        }

        private static void PrepareProject(string buildPath)
        {
            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));
            var target =
#if UNITY_2019_3_OR_NEWER
                project.GetUnityMainTargetGuid();
#else
                project.TargetGuidByName("Unity-iPhone");
#endif
            // The MoPub iOS SDK now includes Swift, so these properties ensure Xcode handles that properly.
            project.AddBuildProperty(target, "SWIFT_VERSION", "5.0");
            project.AddBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            File.WriteAllText(projPath, project.WriteToString());
        }
    }
}
#endif
