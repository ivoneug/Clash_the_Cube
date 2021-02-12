using UnityEngine;
using UnityEngine.Events;

namespace IngameDebugConsole
{
    public class DebugConsoleCommand : MonoBehaviour
    {
        [SerializeField] private string command;
        [SerializeField] private string description;
        [SerializeField] private UnityEvent action;

        private void OnEnable()
        {
            DebugLogConsole.AddCommand(command, description, CommandAction);
        }

        private void OnDisable()
        {
            DebugLogConsole.RemoveCommand(command);
        }

        private void CommandAction()
        {
            action.Invoke();
        }
    }
}