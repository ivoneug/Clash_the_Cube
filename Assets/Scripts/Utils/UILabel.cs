using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ClashTheCube
{
    public class UILabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI shadow;
        private TextMeshProUGUI parent;

        public string text
        {
            get { return label.text; }
            set
            {
                label.text = value;
                if (shadow)
                {
                    shadow.text = value;
                }
                if (parent)
                {
                    parent.text = value;
                }
            }
        }

        private void Awake()
        {
            parent = GetComponent<TextMeshProUGUI>();
        }
    }
}
