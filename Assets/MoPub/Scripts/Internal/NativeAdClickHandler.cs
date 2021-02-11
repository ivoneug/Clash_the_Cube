using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.

public class NativeAdClickHandler : MonoBehaviour, IPointerDownHandler
{
#if mopub_native_beta
    private string _adUnitId;
    private Uri _clickDestinationUrl;


    public void SetAdUnitId(string id)
    {
        _adUnitId = id;
    }


    public void SetDestinationUrl(Uri url)
    {
        _clickDestinationUrl = url;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        OnAdClicked();
    }


    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public void OnAdClicked()
    {
        MoPubManager.Instance.EmitNativeClickEvent(_adUnitId);
        Application.OpenURL(_clickDestinationUrl.ToString());
    }
#else
    public void OnPointerDown(PointerEventData eventData) { }
#endif
}
