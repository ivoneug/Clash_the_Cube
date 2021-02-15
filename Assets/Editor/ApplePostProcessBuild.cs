using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class ApplePostProcessBuild
{
    [PostProcessBuild(1)]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        // Get plist
        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        // Get root
        PlistElementDict rootDict = plist.root;

        rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-7746721415936574~8954039403");

        var array = rootDict.CreateArray("SKAdNetworkItems");
        var dict = array.AddDict();
        dict.SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");

        // Write to file
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
