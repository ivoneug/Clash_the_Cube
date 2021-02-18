using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.SystemInfo;

namespace Framework.Utils
{
    public class SetTargetFPS : MonoBehaviour
    {
        public enum TargetFpsType
        {
            fps30,
            fps60
        }

        [SerializeField] private PlatfromType platforms;
        [SerializeField] TargetFpsType targetFPS = TargetFpsType.fps60;

        private void Awake()
        {
            if (platforms == PlatfromType.None)
            {
                return;
            }
            if (!IsPlaformActive())
            {
                return;
            }

            switch (targetFPS)
            {
                case TargetFpsType.fps60:
                    Application.targetFrameRate = 60;
                    Debug.Log("Set target FPS to 60");
                    break;

                case TargetFpsType.fps30:
                    Application.targetFrameRate = 30;
                    Debug.Log("Set target FPS to 30");
                    break;

                default:
                    break;
            }
        }

        private bool IsPlaformActive()
        {
            return Platform.HasFlag(platforms, Platform.CurrentPlatfrom());
        }
    }
}
