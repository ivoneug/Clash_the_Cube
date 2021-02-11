using System.Collections.Generic;

/// <summary>
/// Functionality in this class has been refactored into <see cref="MoPub"/>, <see cref="MoPubPlatformApi"/>,
/// and <see cref="AdSizeMapping"/>, among others. The remaining code will be moved during a major version update, as
/// it would be a backwards-incompatible change.
/// </summary>
public abstract class MoPubBase
{
    // Data structure to register and initialize a mediated network.
    public class MediatedNetwork
    {
        public string AdapterConfigurationClassName { get; set; }
        public string MediationSettingsClassName    { get; set; }

        public Dictionary<string, string> NetworkConfiguration { get; set; }
        public Dictionary<string, object> MediationSettings    { get; set; }
        public Dictionary<string, string> MoPubRequestOptions  { get; set; }
    }

}
