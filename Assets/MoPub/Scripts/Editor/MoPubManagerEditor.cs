using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoPubManager))]
public class MoPubManagerEditor : Editor
{
    // Enable the contents of each adapter's NetworkConfig script.  (It is disabled by default for backwards
    // compatibility with older versions of this SDK.)
    private const string mopub_manager_define = "mopub_manager";

    private void OnEnable()
    {
        // Add the #define symbol to the Android and iOS build settings if it is not already there.  Also Standalone
        // since exporting unitypackages switches the current build platform to Standalone.
        foreach (var group in new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS }) {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                                        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            if (!defines.Contains(mopub_manager_define))
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group,
                    string.Join(";", defines.Concat(new[] {mopub_manager_define}).ToArray()));
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(3);
        // A drop-menu for conveniently adding one or more NetworkConfig objects (found in mediation adapters)
        // to the MoPubManager gameobject.
        if (EditorGUILayout.DropdownButton(new GUIContent("Add Network Configuration"),
                                           FocusType.Keyboard, GUILayout.ExpandWidth(false)))
            ShowNetworksMenu();
        GUILayout.Space(3);
        DrawDefaultInspector();
    }

    private void ShowNetworksMenu()
    {
        var manager = (MoPubManager) target;

        // Find all NetworkConfig-derived classes.
        var configs = System.AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(a => a.GetTypes(), (a, t) => t)
                      .Where(t => t.IsSubclassOf(typeof(MoPubNetworkConfig)) && !t.IsAbstract)
                      .OrderBy(t => t.Name)
                      .ToArray();

        // Build a menu out of the list of networks.
        var menu = new GenericMenu();
        var any = false;  // Are any more network configs available to add to the manager gameobject?
        foreach (var config in configs) {
            if (manager.GetComponent(config) == null) {
                menu.AddItem(new GUIContent(config.Name), false, () => Undo.AddComponent(manager.gameObject, config));
                any = true;  // Enable the "add all" menu option too.
            } else
                menu.AddDisabledItem(new GUIContent(config.Name));
        }

        var allOfThem = new GUIContent("All Networks");
        if (any)
            menu.AddItem(allOfThem, false, () => {
                foreach (var config in configs)
                    if (manager.GetComponent(config) == null)
                        Undo.AddComponent(manager.gameObject, config);
            });
        else if (configs.Length > 0)
            menu.AddDisabledItem(allOfThem);
        else
            menu.AddDisabledItem(new GUIContent("No networks installed"));

        menu.ShowAsContext();
    }
}
