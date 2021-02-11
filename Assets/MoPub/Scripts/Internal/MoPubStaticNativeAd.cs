public class MoPubStaticNativeAd : AbstractNativeAd
{
#if mopub_native_beta
    protected override void AddEventHandlers()
    {
        MoPubManager.OnNativeLoadEvent += OnNativeLoadHandler;
        MoPubManager.OnNativeImpressionEvent += OnNativeImpressionHandler;
        MoPubManager.OnNativeClickEvent += OnNativeClickHandler;
        MoPubManager.OnNativeFailEvent += OnNativeFailHandler;
    }

    protected override void RemoveEventHandlers()
    {
        MoPubManager.OnNativeLoadEvent -= OnNativeLoadHandler;
        MoPubManager.OnNativeImpressionEvent -= OnNativeImpressionHandler;
        MoPubManager.OnNativeClickEvent -= OnNativeClickHandler;
        MoPubManager.OnNativeFailEvent -= OnNativeFailHandler;
    }

    public override void LoadAd()
    {
        MoPub.RequestNativeAd(AdUnitId);
    }
#endif
}
