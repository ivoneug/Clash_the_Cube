using UnityEngine;

using MaxAdSize = MoPub.MaxAdSize;

public static class AdSizeMapping
{
    public static float Width(this MaxAdSize adSize)
    {
        switch (adSize) {
            case MaxAdSize.Width300Height50:
            case MaxAdSize.Width300Height250:
                return 300;
            case MaxAdSize.Width320Height50:
                return 320;
            case MaxAdSize.Width336Height280:
                return 336;
            case MaxAdSize.Width468Height60:
                return 468;
            case MaxAdSize.Width728Height90:
                return 728;
            case MaxAdSize.Width970Height90:
            case MaxAdSize.Width970Height250:
                return 970;
            case MaxAdSize.ScreenWidthHeight50:
            case MaxAdSize.ScreenWidthHeight90:
            case MaxAdSize.ScreenWidthHeight250:
            case MaxAdSize.ScreenWidthHeight280:
                var pixels = Screen.width;
                var dpi = Screen.dpi;
                var dips = pixels / (dpi / 160.0f);
                return dips;
            default:

                // fallback to default size: Width320Height50
                return 300;
        }
    }


    public static float Height(this MaxAdSize adSize)
    {
        switch (adSize) {
            case MaxAdSize.Width300Height50:
            case MaxAdSize.Width320Height50:
            case MaxAdSize.ScreenWidthHeight50:
                return 50;
            case MaxAdSize.Width468Height60:
                return 60;
            case MaxAdSize.Width728Height90:
            case MaxAdSize.Width970Height90:
            case MaxAdSize.ScreenWidthHeight90:
                return 90;
            case MaxAdSize.Width300Height250:
            case MaxAdSize.Width970Height250:
            case MaxAdSize.ScreenWidthHeight250:
                return 250;
            case MaxAdSize.Width336Height280:
            case MaxAdSize.ScreenWidthHeight280:
                return 280;
            default:
                // fallback to default size: Width320Height50
                return 50;
        }
    }
}
