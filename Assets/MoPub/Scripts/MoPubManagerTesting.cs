using UnityEngine;

// A slight modification of MoPubManager that supports test setups.  It should go on a separate object from the main
// MoPubManager script.  (The MoPubManager prefab includes it on a child object, so if you are using that, it is already
// present in the scene.)  The idea is to leave the testing object disabled except for when making a test build.
//
// If the object is active (and this script is enabled), then the script intercepts the SDK initialization.
// Configure the testing object's MoPub SDK and network options for your test environment instead of your production
// environment.  Note that both scripts respond to SDK events.

public class MoPubManagerTesting : MoPubManager
{
    void Awake()
    {
        // Don't set Instance to this here.  Wait for OnEnable() to do that.  That is so that this object
        // can live in the scene disabled and it won't "steal" the Instance pointer.

        OnSdkInitializedEvent += fwdSdkInitialized;
    }


    // Wait till this object is enabled, and then take over the Instance reference if necessary.
    void OnEnable()
    {
        if (Instance == this)
            return;
        if (Instance is MoPubManagerTesting) {
            Debug.LogWarning("Another testing MoPubManager singleton instance already exists.  That object will initialize the SDK instead of this one.");
            return;
        }
        Instance = this;
        Debug.LogWarning("Activating MoPubManager test configuration!");
        if (MoPub.IsSdkInitialized)
            Debug.LogError("The MoPub SDK was already initialized!");
    }
}
