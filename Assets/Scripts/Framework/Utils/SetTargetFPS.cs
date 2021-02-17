using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetFPS : MonoBehaviour
{
    [System.Flags]
    public enum PlatfromType
    {
        None = 0,
        iOS = 1,
        Android = 2,
        Desktop = 4,
        Editor = 8
    }

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

    private bool HasFlag(PlatfromType flag)
    {
        return (platforms & flag) == flag;
    }

    private PlatfromType PlatfromToFlag()
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

    private bool IsPlaformActive()
    {
        return HasFlag(PlatfromToFlag());
    }
}
