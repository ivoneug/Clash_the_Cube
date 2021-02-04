using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public class VFXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] helpArrows;
        [SerializeField][Range(0, 45)] private int showHelpArrowsDelay = 15;

        void Start()
        {

        }

        void Update()
        {
            
        }
        
        public void ShowArrows()
        {
            SetArrowsActive(true);
            CancelInvoke("ShowArrows");
        }

        public void HideArrows()
        {
            SetArrowsActive(false);
            CancelInvoke("ShowArrows");
            Invoke("ShowArrows", showHelpArrowsDelay);
        }

        private void SetArrowsActive(bool active)
        {
            foreach (var arrow in helpArrows)
            {
                if (active && !arrow.isPlaying)
                {
                    arrow.Play();
                }
                else if (!active && arrow.isPlaying)
                {
                    arrow.Stop();
                }
            }
        }
    }
}
