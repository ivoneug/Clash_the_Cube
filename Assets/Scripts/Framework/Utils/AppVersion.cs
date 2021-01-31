using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class AppVersion : MonoBehaviour
    {
        float deltaTime = 0.0f;
        GUIStyle style;
        Rect rect;

        [SerializeField]
        Font font;

        [SerializeField]
        [Range(1, 8)]
        int fontSize = 4;

        [SerializeField]
        Color color = Color.white;

        [SerializeField]
        TextAnchor anchor = TextAnchor.LowerRight;

        [SerializeField]
        int inset = 0;

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

            string text = string.Format("v{0}", Application.version);

            GUI.Label(rect, text, style);
        }
    }
}