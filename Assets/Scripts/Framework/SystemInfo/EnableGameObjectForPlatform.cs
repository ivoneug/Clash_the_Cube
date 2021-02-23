using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.SystemInfo
{
    [DefaultExecutionOrder(100)]
    public class EnableGameObjectForPlatform : MonoBehaviour
    {
        [SerializeField] private PlatfromType platforms;
        [SerializeField] private bool checkInUpdate = false;

        private void Start()
        {
            gameObject.SetActive(Platform.HasFlag(platforms, Platform.CurrentPlatfrom()));
        }

        private void Update()
        {
            if (!checkInUpdate)
            {
                return;
            }

            bool isActive = Platform.HasFlag(platforms, Platform.CurrentPlatfrom());
            if (isActive != gameObject.activeSelf)
            {
                gameObject.SetActive(isActive);
                Debug.Log("CHANGE ACTIVE");
            }
        }
    }
}
