using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Framework.Variables;

namespace ClashTheCube
{
    public class AchievementUIController : MonoBehaviour
    {
        [SerializeField] private IntReference nextCubeNumber;
        [SerializeField] private UILabel number;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI subtitle;

        private void OnEnable()
        {
            int value = nextCubeNumber;
            
            number.text = value.ToString();
            title.text = string.Format("You've got {0}!", value);
            subtitle.text = string.Format("How about getting {0}?", value * 2);
        }
    }
}
