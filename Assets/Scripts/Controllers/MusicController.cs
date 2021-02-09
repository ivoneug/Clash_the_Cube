using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.Soundy;

namespace ClashTheCube
{
    public class MusicController : MonoBehaviour
    {
        public static MusicController Instance;

        private SoundyData _activeMusic;
        private SoundyController _activeMusicController;

        public bool IsSingleton
        {
            get
            {
                return this == Instance;
            }
        }

        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            if (!IsSingleton) return;

            ActivateMusic();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
                if (_activeMusicController != null)
                {
                    _activeMusicController.Stop();
                }

                _activeMusic = null;
                _activeMusicController = null;
            }

            if (_activeMusic == null)
            {
                _activeMusicController = SoundyManager.Play(target.music);
                _activeMusic = target.music;
            }
        }
    }
}
