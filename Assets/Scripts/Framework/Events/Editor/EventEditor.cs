using UnityEditor;
using UnityEngine;

namespace Framework.Events
{
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class EventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEvent e = target as GameEvent;
            
            if (GUILayout.Button("Raise")) {
                e.Raise();
            }
        }
    }
}
