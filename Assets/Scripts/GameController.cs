using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;

namespace ClashTheCube
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoolReference gamePaused;

        private void Start()
        {
            ResumeGame();

            try
            {
                Vibration.Init();
            }
            catch { }
        }

        public void Vibrate()
        {
            try
            {
                Vibration.VibratePop();
            }
            catch { }
        }

        public void PauseGame()
        {
            gamePaused.Variable.SetValue(true);
            UpdateGamePause();
        }

        public void ResumeGame()
        {
            gamePaused.Variable.SetValue(false);
            UpdateGamePause();
        }

        private void UpdateGamePause()
        {
            Time.timeScale = gamePaused ? 0 : 1;
        }
    }
}
