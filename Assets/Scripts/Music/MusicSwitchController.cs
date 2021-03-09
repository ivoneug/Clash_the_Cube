using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;
using TMPro;

namespace ClashTheCube
{
    public class MusicSwitchController : MonoBehaviour
    {
        [SerializeField] private IntReference targetIndex;
        [SerializeField] private GameObject musicTargetContainer;
        [SerializeField] private TextMeshProUGUI label;

        private MusicTarget[] targets;

        private void Start()
        {
            RefreshTargets();
        }

        public void PreviousMusic()
        {
            targetIndex.Variable.SetValue(targetIndex.Value - 1);

            if (targetIndex < 0)
            {
                targetIndex.Variable.SetValue(targets.Length - 1);
            }

            RefreshTargets();
        }

        public void NextMusic()
        {
            targetIndex.Variable.SetValue(targetIndex.Value + 1);

            if (targetIndex >= targets.Length)
            {
                targetIndex.Variable.SetValue(0);
            }

            RefreshTargets();
        }

        public void RefreshTargets()
        {
            targets = musicTargetContainer.GetComponentsInChildren<MusicTarget>(true);

            for (int idx = 0; idx < targets.Length; idx++)
            {
                var target = targets[idx];

                if (idx == targetIndex)
                {
                    target.gameObject.SetActive(true);
                    label.text = target.gameObject.name;
                }
                else
                {
                    target.gameObject.SetActive(false);
                }
            }
        }
    }
}
