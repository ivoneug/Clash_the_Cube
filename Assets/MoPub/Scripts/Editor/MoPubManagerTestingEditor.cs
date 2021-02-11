using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoPubManagerTesting))]
public class MoPubManagerTestingEditor : MoPubManagerEditor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(5);
        GUILayout.Label("This is a testing configuration.  If the gameobject is active (and this script is enabled)\n" +
                        "then the MoPub SDK and networks will be initialized with this configuration.");
        GUILayout.Space(2);

        var testManager = (MoPubManagerTesting) target;
        // If this manager is a child of the production manager, then show a button allowing the parent component
        // values to be copied down.
        var prodManager = testManager.transform.parent != null ? testManager.transform.parent.GetComponentInParent<MoPubManager>() : null;
        if (prodManager != null && GUILayout.Button("Copy from parent configuration", GUILayout.ExpandWidth(false))) {
            Undo.RecordObject(testManager, "Copy Production Configuration");
            EditorUtility.CopySerializedIfDifferent(prodManager, testManager);
            testManager.Initialized.RemoveAllListeners();
            foreach (var prodConfig in prodManager.GetComponents<MoPubNetworkConfig>()) {
                var testConfig = testManager.GetComponent(prodConfig.GetType());
                if (testConfig == null)
                    testConfig = Undo.AddComponent(testManager.gameObject, prodConfig.GetType());
                Undo.RecordObject(testConfig, "Copy Production Configuration");
                EditorUtility.CopySerializedIfDifferent(prodConfig, testConfig);
            }
        }

        GUILayout.Space(4);
        base.OnInspectorGUI();
    }
}
