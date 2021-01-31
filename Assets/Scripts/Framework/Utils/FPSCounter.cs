using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        float deltaTime = 0.0f;
        GUIStyle style;
        Rect rect;

        [SerializeField]
        TextAnchor anchor = TextAnchor.LowerLeft;

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
            int w = Screen.width, h = Screen.height;
            if (style == null)
            {
                style = new GUIStyle();
                rect = new Rect(0, 0, w, h);
                style.alignment = anchor;
                style.fontSize = h * 4 / 100;
                style.normal.textColor = Color.white;
            }
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}