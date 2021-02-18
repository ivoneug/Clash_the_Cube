using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.SystemInfo
{
    [DefaultExecutionOrder(-1)]
    public class OverrideEditorPlatform : MonoBehaviour
    {
        [SerializeField] private PlatfromType currentPlaform;
        private void Awake()
        {
            Platform.OverrideEditorPlatform(currentPlaform);
        }
    }
}
