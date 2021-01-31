using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        public enum FpsStyle
        {
            Fps,
            Msec,
            FpsMsec
        }

        [SerializeField] private FpsStyle fpsStyle = FpsStyle.FpsMsec;

        float deltaTime = 0.0f;
        GUIStyle style;
        Rect rect;

        [SerializeField] Font font;
        [SerializeField][Range(1, 8)] int fontSize = 4;
        [SerializeField] Color color = Color.white;
        [SerializeField] TextAnchor anchor = TextAnchor.LowerRight;
        [SerializeField] int inset = 0;

        void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (style == null)
            {
                int w = Screen.width - inset * 2;
                int h = Screen.height - inset * 2;

                style = new GUIStyle();
                rect = new Rect(inset, inset, w, h);
                style.alignment = anchor;
                style.fontSize = h * fontSize / 100;
                style.normal.textColor = color;

                if (font)
                {
                    style.font = font;
                }
            }

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            string text = string.Empty;
            switch (fpsStyle)
            {
                case FpsStyle.Fps:
                    text = string.Format("{0:0.} fps", fps);
                    break;

                case FpsStyle.Msec:
                    text = string.Format("{0:0.0} ms", msec);
                    break;

                case FpsStyle.FpsMsec:
                    text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
                    break;
            }

            GUI.Label(rect, text, style);
        }
    }
}