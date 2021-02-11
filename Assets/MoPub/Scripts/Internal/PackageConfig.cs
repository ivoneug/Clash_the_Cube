using System;
using System.Collections.Generic;

public abstract class PackageConfig
{
    public enum Platform
    {
        ANDROID, IOS
    }


    /// <summary>
    /// The name of the Network Adapter in this package.
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract string Name { get; }


    /// <summary>
    /// A dot-separated version number such as "5", "5.0", or "5.0.5".
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract string Version { get; }


    /// <summary>
    /// The Network SDK version this adapter supports for each platform.
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract Dictionary<Platform, string> NetworkSdkVersions { get; }


    //[Obsolete("Now handled in MoPubNetworkConfig.NetworkOptions")]
    public virtual Dictionary<Platform, string> AdapterClassNames { get { return null; } }
}
