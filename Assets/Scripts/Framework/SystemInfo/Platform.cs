using System;
using UnityEngine;

namespace Framework.SystemInfo
{
    public class Platform
    {
        [Flags]
        public enum PlatfromType
        {
            None = 0,
            iOS = 1,
            Android = 2,
            Desktop = 4,
            Editor = 8
        }

        public static bool HasFlag(PlatfromType platforms, PlatfromType flag)
        {
            return (platforms & flag) == flag;
        }

        public static PlatfromType CurrentPlatfrom()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return PlatfromType.iOS;

                case RuntimePlatform.Android:
                    return PlatfromType.Android;

                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                    return PlatfromType.Desktop;

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                    return PlatfromType.Editor;

                default:
                    return PlatfromType.None;
            }
        }
    }
}