using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Events;
using TMPro;

namespace ClashTheCube
{
    public class DailyRewardPopupUIController : MonoBehaviour
    {
        [SerializeField] LocalisationController localisationController;
        [SerializeField] private AbilityTypeInfo[] infoItems;
        [SerializeField] private AbilityTypeInfo activeInfo;
        [SerializeField] private TextMeshProUGUI subtitle;
        [SerializeField] private UILabel viewAdLabel;
        [SerializeField] private UILabel claimLabel;
        [SerializeField] private Image icon;

        private void OnEnable()
        {
            var info = infoItems[Random.Range(0, infoItems.Length)];
            activeInfo.CloneFrom(info);

            var subtitleText = localisationController.GetLocalizedText("dailyRewardSubtitle");
            var abilityName = localisationController.GetLocalizedText(info.localNameKey);
            subtitle.text = string.Format(subtitleText, abilityName, info.rewardCount, info.adCount);

            icon.sprite = info.icon;

            var adButtonText = localisationController.GetLocalizedText("dailyRewardViewAd");
            viewAdLabel.text = string.Format(adButtonText, info.adCount);

            var claimButtonText = localisationController.GetLocalizedText("dailyRewardClaim");
            claimLabel.text = string.Format(claimButtonText, info.rewardCount);
        }
    }
}
