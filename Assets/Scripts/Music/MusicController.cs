using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.Soundy;

namespace ClashTheCube
{
    public class MusicController : MonoBehaviour
    {
        private SoundyData _activeMusic;
        private SoundyController _activeMusicController;

        void Awake()
        {
            SceneManager.sceneUnloaded += (Scene scene) => {
                StopMusic();
            };
        }

        private void Start()
        {
            ActivateMusic();
        }

        public void ActivateMusic()
        {
            var target = FindObjectOfType<MusicTarget>();

            if (!target)
            {
                return;
            }

            if (_activeMusic != null && _activeMusic.SoundName != target.music.SoundName)
            {
                StopMusic();
            }

            if (_activeMusic == null)
            {
                _activeMusicController = SoundyManager.Play(target.music);
                _activeMusic = target.music;
            }
        }

        private void StopMusic()
        {
            if (_activeMusic != null)
            {
                if (_activeMusicController != null)
                {
                    _activeMusicController.Stop();
                }

                _activeMusic = null;
                _activeMusicController = null;
            }
        }
    }
}
