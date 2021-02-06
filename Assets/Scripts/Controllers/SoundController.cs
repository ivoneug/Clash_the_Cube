using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Soundy;

namespace ClashTheCube
{
    public class SoundController : MonoBehaviour
    {
        [Header("Cube Merge Sound")]
        [SerializeField] private SoundyData cubeMergeSound;

        [Header("Achievement Sound")]
        [SerializeField] private SoundyData achievementSound;

        [Header("Game Over Sound")]
        [SerializeField] private SoundyData gameOverSound;

        public void PlayCubeMergeSound()
        {
            SoundyManager.Play(cubeMergeSound);
        }

        public void PlayAchievementSound()
        {
            SoundyManager.Play(achievementSound);
        }

        public void PlayGameOverSound()
        {
            SoundyManager.Play(gameOverSound);
        }
    }
}
