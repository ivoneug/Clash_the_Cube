using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable AccessToStaticMemberViaDerivedType

public static class MoPubLog
{
    public static class SdkLogEvent
    {
        public const string InitStarted = "SDK initialization started";
        public const string InitFinished = "SDK initialized and ready to display ads.  Log Level: {0}";
    }

    public static class ConsentLogEvent
    {
        public const string Updated = "Consent changed from {0} to {1} : PII can{2} be collected.";
        public const string LoadAttempted = "Attempting to load consent dialog";
        public const string LoadSuccess = "Consent dialog loaded";
        public const string LoadFailed = "Consent dialog failed: {0}";
        public const string ShowAttempted = "Consent dialog attempting to show";
        public const string ShowSuccess = "Sucessfully showed consent dialog";
        public const string Dismissed = "Consent dialog dismissed";

    }

    public static class AdLogEvent
    {
        public const string LoadAttempted = "Attempting to load ad";
        public const string LoadSuccess = "Ad loaded";
        public const string LoadFailed = "Ad failed to load: ({0}) {1}";
        public const string ShowAttempted = "Attempting to show ad";
        public const string ShowSuccess = "Ad shown";
        public const string Tapped = "Ad tapped";
        public const string Expanded = "Ad expanded";
        public const string Collapsed = "Ad collapsed";
        public const string Dismissed = "Ad did disappear";
        public const string ShouldReward = "Ad should reward user with {0} {1}";
        public const string Expired = "Ad expired since it was not shown fast enough";
    }

    private static readonly Dictionary<string, MoPub.LogLevel> logLevelMap =
        new Dictionary<string, MoPub.LogLevel>
    {
        { SdkLogEvent.InitStarted, MoPub.LogLevel.Debug },
        { SdkLogEvent.InitFinished, MoPub.LogLevel.Info },
        { ConsentLogEvent.Updated, MoPub.LogLevel.Debug },
        { ConsentLogEvent.LoadAttempted, MoPub.LogLevel.Debug },
        { ConsentLogEvent.LoadSuccess, MoPub.LogLevel.Debug },
        { ConsentLogEvent.LoadFailed, MoPub.LogLevel.Debug },
        { ConsentLogEvent.ShowAttempted, MoPub.LogLevel.Debug },
        { ConsentLogEvent.ShowSuccess, MoPub.LogLevel.Debug },
        { AdLogEvent.LoadAttempted, MoPub.LogLevel.Info },
        { AdLogEvent.LoadSuccess, MoPub.LogLevel.Info },
        { AdLogEvent.LoadFailed, MoPub.LogLevel.Info },
        { AdLogEvent.ShowAttempted, MoPub.LogLevel.Info },
        { AdLogEvent.ShowSuccess, MoPub.LogLevel.Info },
        { AdLogEvent.Tapped, MoPub.LogLevel.Debug },
        { AdLogEvent.Expanded, MoPub.LogLevel.Debug },
        { AdLogEvent.Collapsed, MoPub.LogLevel.Debug },
        { AdLogEvent.Dismissed, MoPub.LogLevel.Debug },
        { AdLogEvent.ShouldReward, MoPub.LogLevel.Debug },
        { AdLogEvent.Expired, MoPub.LogLevel.Debug },
    };

    public static void Log(string callerMethod, string message, params object[] args)
    {
        MoPub.LogLevel messageLogLevel;
        if (!logLevelMap.TryGetValue(message, out messageLogLevel))
            messageLogLevel = MoPub.LogLevel.Debug;

        if (MoPub.CachedLogLevel > messageLogLevel) return;

        var formattedMessage = "[MoPub-Unity] [" + callerMethod + "] " + message;
        try {
            Debug.LogFormat(formattedMessage, args);
        } catch (FormatException) {
            Debug.Log("Format exception while logging message { " + formattedMessage + " } with arguments { " +
                       string.Join(",", args.Select(a => a.ToString()).ToArray()) + " }");
        }
    }
}
