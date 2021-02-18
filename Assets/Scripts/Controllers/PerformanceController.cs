using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.SystemInfo;
using BeautifyEffect;

namespace ClashTheCube
{
    public class PerformanceController : MonoBehaviour
    {
        public enum PerformanceType
        {
            Auto,
            BestQuality,
            BestPerformance
        }

        [SerializeField] private PerformanceType performanceType = PerformanceType.Auto;
        [SerializeField] private TextAnchor anchor = TextAnchor.LowerLeft;
        [SerializeField] private bool isDebug = true;

        private GUIStyle style;
        private Rect rect;

        private void Start()
        {
            SetPerformance();
        }

        private void SetPerformance()
        {
            PerformanceType type = CalcPerformanceType();

            if (type == PerformanceType.BestPerformance)
            {
                Beautify.instance.quality = BEAUTIFY_QUALITY.BestPerformance;
                return;
            }
            else if (type == PerformanceType.BestQuality)
            {
                Beautify.instance.quality = BEAUTIFY_QUALITY.BestQuality;
                return;
            }
        }

        private PerformanceType CalcPerformanceType()
        {
            PerformanceType type = performanceType;
            if (type == PerformanceType.Auto)
            {
                type = Platform.IsMobilePlatform() ? PerformanceType.BestPerformance : PerformanceType.BestQuality;
            }

            return type;
        }

        private void OnGUI()
        {
            if (!isDebug)
            {
                return;
            }

            int w = Screen.width, h = Screen.height;
            if (style == null)
            {
                style = new GUIStyle();
                rect = new Rect(0, 0, w, h);
                style.alignment = anchor;
                style.fontSize = h * 4 / 100;
                style.normal.textColor = Color.white;
            }
            string text = CalcPerformanceType() == PerformanceType.BestQuality ? "Best Quality" : "Best Performance";

            GUI.Label(rect, text, style);
        }
    }
}
