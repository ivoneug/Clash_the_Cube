using UnityEngine;

[ExecuteInEditMode]
public class MoPubNativeAdsToggler : MonoBehaviour
{
    private void Awake()
    {
        // Only run this script in Editor Mode
        if (Application.isPlaying)
            enabled = false;
    }


    private void Update()
    {
        const bool enable =
#if !mopub_native_beta
            false
#else
            true
#endif
        ;
        gameObject.hideFlags = enable ? HideFlags.None : (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
        foreach (Transform child in transform)
            child.gameObject.SetActive(enable);
    }
}
