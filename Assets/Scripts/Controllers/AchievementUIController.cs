﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Framework.Variables;

namespace ClashTheCube
{
    public class AchievementUIController : MonoBehaviour
    {
        [SerializeField] LocalisationController localisationController;

        [SerializeField] private IntReference nextCubeNumber;
        [SerializeField] private UILabel number;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI subtitle;
        [SerializeField] private UILabel gotItButton;

        private void OnEnable()
        {
            int value = nextCubeNumber;

            string titleFormat = localisationController.GetLocalizedText("achievementTitle");
            string subtitleFormat = localisationController.GetLocalizedText("achievementSubtitle");

            number.text = value.ToString();
            title.text = string.Format(titleFormat, value);
            subtitle.text = string.Format(subtitleFormat, value * 2);
            
            localisationController.LocalizeLabel(gotItButton, "achievementButton");
        }
    }
}
