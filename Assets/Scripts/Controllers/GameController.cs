using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Variables;
using Framework.Utils;

namespace ClashTheCube
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoolReference gamePaused;
        [SerializeField] private BoolReference isVibrationOn;

        [SerializeField] private FPSCounter fpsCounter;

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
            if (!isVibrationOn)
            {
                return;
            }
            
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

        public void ReloadScene()
        {
            ResumeGame();
            
            string scene = SceneManager.GetActiveScene().name;
            Initiate.Fade(scene, Color.black, 2f);
        }

        public void ToggleFps()
        {
            fpsCounter.enabled = !fpsCounter.enabled;
        }
    }
}
