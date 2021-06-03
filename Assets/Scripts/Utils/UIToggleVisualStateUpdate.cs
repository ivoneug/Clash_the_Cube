using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

namespace ClashTheCube
{
    [RequireComponent(typeof(UIToggle))]
    public class UIToggleVisualStateUpdate : MonoBehaviour
    {
        [SerializeField] private GameObject onState;
        [SerializeField] private GameObject offState;

        private UIToggle toggle;

        private void Start()
        {
            toggle = GetComponent<UIToggle>();
        }

        private void Update()
        {
            onState.SetActive(toggle.IsOn);
            offState.SetActive(!toggle.IsOn);
        }
    }
}
