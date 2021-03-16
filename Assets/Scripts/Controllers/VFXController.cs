using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;

namespace ClashTheCube
{
    public class VFXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem cubeMergeVFX;
        [SerializeField] private GameObject explosionVFX;
        [SerializeField] private Vector3Variable vfxPosition;
        [SerializeField] private FloatReference cubeMergeVFXPositionShift;
        [SerializeField] private ParticleSystem[] helpArrows;
        [SerializeField][Range(0, 45)] private int showHelpArrowsDelay = 15;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void ShowMergeVFX()
        {
            var direction = (vfxPosition.Value - mainCamera.transform.position).normalized;
            var position = vfxPosition.Value - direction * cubeMergeVFXPositionShift;

            var vfx = Instantiate(cubeMergeVFX, position, Quaternion.identity).GetComponent<ParticleSystem>();
            vfx.Play();
            Destroy(vfx.gameObject, vfx.main.duration);
        }

        public void ShowExplosionVFX()
        {
            var vfx = Instantiate(explosionVFX, vfxPosition.Value, Quaternion.identity).GetComponent<ParticleSystem>();
            vfx.Play();
            Destroy(vfx.gameObject, vfx.main.duration);
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
