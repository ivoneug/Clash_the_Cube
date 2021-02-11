using System;
using System.Collections.Generic;
using MJ = MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractNativeAd : MonoBehaviour
{
    public struct Data
    {
        public Uri MainImageUrl;
        public Uri IconImageUrl;
        public Uri ClickDestinationUrl;

        public string CallToAction;
        public string Title;
        public string Text;
        public double StarRating;

        public Uri PrivacyInformationIconClickThroughUrl;
        public Uri PrivacyInformationIconImageUrl;


        public static Uri ToUri(object value)
        {
            var uri = value as Uri;
            if (uri != null)
                return uri;
            var urlstr = value as string;
            if (string.IsNullOrEmpty(urlstr))
                return null;
            if (Uri.IsWellFormedUriString(urlstr, UriKind.Absolute))
                return new Uri(urlstr, UriKind.Absolute);
            Debug.LogError("Invalid URL: " + urlstr);
            return null;
        }


        public static Data FromJson(string json)
        {
            var obj = MJ.Json.Deserialize(json) as Dictionary<string, object> ?? new Dictionary<string, object>();

            object value;
            return new Data {
                MainImageUrl = obj.TryGetValue("mainImageUrl", out value) ? ToUri(value) : null,
                IconImageUrl = obj.TryGetValue("iconImageUrl", out value) ? ToUri(value) : null,
                ClickDestinationUrl = obj.TryGetValue("clickDestinationUrl", out value) ? ToUri(value) : null,

                CallToAction = obj.TryGetValue("callToAction", out value) ? value as string : string.Empty,
                Title = obj.TryGetValue("title", out value) ? value as string : string.Empty,
                Text = obj.TryGetValue("text", out value) ? value as string : string.Empty,
                StarRating = obj.TryGetValue("starRating", out value) ? (double)value : 0,

                PrivacyInformationIconClickThroughUrl =
                    obj.TryGetValue("privacyInformationIconClickThroughUrl", out value) ? ToUri(value) : null,
                PrivacyInformationIconImageUrl =
                    obj.TryGetValue("privacyInformationIconImageUrl", out value) ? ToUri(value) : null
            };
        }


        public override string ToString()
        {
            return string.Format(
                "mainImageUrl: {0}\n" + "iconImageUrl: {1}\n" + "clickDestinationUrl: {2}\n"
                + "callToAction: {3}\n" + "title: {4}\n" + "text: {5}\n" + "starRating: {6}\n"
                + "privacyInformationIconClickThroughUrl: {7}\n" + "privacyInformationIconImageUrl: {8}",
                MainImageUrl, IconImageUrl, ClickDestinationUrl, CallToAction, Title, Text, StarRating,
                PrivacyInformationIconClickThroughUrl, PrivacyInformationIconImageUrl);
        }
    }

    // Ad fields in the GameObject
    public string AdUnitId;

    [Header("Text")]
    public Text Title;
    public Text Text;
    public Text CallToAction;

    [Header("Images")]
    public Renderer MainImage;
    public Renderer IconImage;
    public Renderer PrivacyInformationIconImage;

#if mopub_native_beta
    private NativeAdClickHandler[] _clickHandlers;
    protected bool IsLoaded;
    private const float InvisibleScale = 1e-10f;

    protected abstract void AddEventHandlers();
    protected abstract void RemoveEventHandlers();


    public abstract void LoadAd();


    protected virtual void OptionalInit() { }


    private void OnEnable()
    {
        if (AdUnitId == null) {
            Debug.LogWarning("NativeAd is missing adUnitId; please set it from its inspector panel.");
            gameObject.SetActive(false);
            return;
        }

        _clickHandlers = gameObject.transform.GetComponentsInChildren<NativeAdClickHandler>();
        if (_clickHandlers.Length < 1) {
            Debug.LogWarning(
                "NativeAd is missing NativeAdClickHandlers; please ensure each clickable element has one.");
            gameObject.SetActive(false);
            return;
        }

        OptionalInit();
        SetAdUnitIdOnClickHandlers();
        AddEventHandlers();
    }


    private void OnDisable()
    {
        StopAllCoroutines();
        RemoveEventHandlers();
    }


    public void Hide()
    {
        transform.localScale = InvisibleScale * Vector3.one;
    }


    public void Show()
    {
        if (IsLoaded)
            transform.localScale = Vector3.one;
        else
            Debug.Log("Attempted to show before ad was loaded; ad unit ID: " + AdUnitId);
    }


    protected void OnNativeLoadHandler(string adUnitId, Data nativeAdData)
    {
        if (adUnitId != AdUnitId)
            return;

        Debug.Log("NativeAd loaded!");

        Debug.Log("Preparing ad game object...");
        Title.text = nativeAdData.Title;
        Text.text = nativeAdData.Text;
        CallToAction.text = nativeAdData.CallToAction;
        SetDestinationUrlOnClickHandlers(nativeAdData.ClickDestinationUrl);

        Debug.Log("Loading images...");
        StartCoroutine(LoadNativeAdImage(MainImage, nativeAdData.MainImageUrl));
        StartCoroutine(LoadNativeAdImage(IconImage, nativeAdData.IconImageUrl));
        StartCoroutine(LoadNativeAdImage(PrivacyInformationIconImage, nativeAdData.PrivacyInformationIconImageUrl));

        IsLoaded = true;
        Debug.Log("NativeAd is ready!");
    }


    protected void OnNativeImpressionHandler(string triggeredAdUnitId)
    {
        if (triggeredAdUnitId != AdUnitId)
            return;
        Debug.Log("NativeAd impressed");
    }


    protected void OnNativeClickHandler(string triggeredAdUnitId)
    {
        if (triggeredAdUnitId != AdUnitId)
            return;
        Debug.Log("NativeAd clicked");
    }


    protected void OnNativeFailHandler(string triggeredAdUnitId, string error)
    {
        if (triggeredAdUnitId != AdUnitId)
            return;
        Debug.Log("NativeAd failed to load: " + AdUnitId + " error: " + error);
    }


    private void SetAdUnitIdOnClickHandlers()
    {
        foreach (var clickHandler in _clickHandlers)
            clickHandler.SetAdUnitId(AdUnitId);
    }


    private void SetDestinationUrlOnClickHandlers(Uri url)
    {
        foreach (var clickHandler in _clickHandlers)
            clickHandler.SetDestinationUrl(url);
    }


    private IEnumerator LoadNativeAdImage(Renderer imageRenderer, Uri url)
    {
        if (url == null)
            yield break;
        using (var www = new WWW(url.ToString())) {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
                imageRenderer.material.mainTexture = www.texture;
            else
                Debug.LogWarning(
                    "Error downlading image for ad unit " + AdUnitId + " for asset for " + imageRenderer.name + " from "
                    + url + ":\n" + www.error);
        }
    }
#endif
}
