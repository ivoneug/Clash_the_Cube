using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Databox;
using Framework.Variables;

namespace ClashTheCube
{
    public class LocalisationController : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;
        [SerializeField] private StringReference languageCode;

        [SerializeField] private UILabel settingsTitle;
        [SerializeField] private TextMeshProUGUI soundFx;
        [SerializeField] private TextMeshProUGUI music;
        [SerializeField] private TextMeshProUGUI vibration;
        [SerializeField] private UILabel restartGameSettings;

        [SerializeField] private UILabel removeAdsTitle;
        [SerializeField] private TextMeshProUGUI removeAdsInfo;

        [SerializeField] private UILabel gameOverTitle;
        [SerializeField] private TextMeshProUGUI gameOverInfo;
        [SerializeField] private TextMeshProUGUI gameOverTextContinue;
        [SerializeField] private UILabel gameOverContinueButton;
        [SerializeField] private UILabel gameOverRestartGameButton;
        [SerializeField] private TextMeshProUGUI restorePurchases;

        [SerializeField] private UILabel dailyRewardTitle;

        public void UpdateUI()
        {
            LocalizeLabel(settingsTitle, nameof(settingsTitle));
            LocalizeLabel(soundFx, nameof(soundFx));
            LocalizeLabel(music, nameof(music));
            LocalizeLabel(vibration, nameof(vibration));
            LocalizeLabel(restartGameSettings, "restartGameButton");

            LocalizeLabel(removeAdsTitle, nameof(removeAdsTitle));
            LocalizeLabel(removeAdsInfo, nameof(removeAdsInfo));

            LocalizeLabel(gameOverTitle, nameof(gameOverTitle));
            LocalizeLabel(gameOverInfo, nameof(gameOverInfo));
            LocalizeLabel(gameOverTextContinue, nameof(gameOverTextContinue));
            LocalizeLabel(gameOverContinueButton, nameof(gameOverContinueButton));
            LocalizeLabel(gameOverRestartGameButton, "restartGameButton");

            LocalizeLabel(restorePurchases, nameof(restorePurchases));

            LocalizeLabel(dailyRewardTitle, nameof(dailyRewardTitle));
        }

        public void LocalizeLabel(TextMeshProUGUI label, string key)
        {
            if (!label)
            {
                return;
            }

            string text = GetLocalizedText(key);
            if (text == string.Empty)
            {
                Debug.LogWarning("Unable to localize \"" + key + "\" to \"" + languageCode + "\": no localized text found");
                return;
            }

            label.text = text;
        }

        public void LocalizeLabel(UILabel label, string key)
        {
            if (!label)
            {
                return;
            }

            string text = GetLocalizedText(key);
            if (text == string.Empty)
            {
                Debug.LogWarning("Unable to localize \"" + key + "\" to \"" + languageCode + "\": no localized text found");
                return;
            }

            label.text = text;
        }

        public string GetLocalizedText(string key)
        {
            if (!databox)
            {
                return string.Empty;
            }

            if (!databox.EntryExists(DataBaseController.Localisation_Table, key))
            {
                return string.Empty;
            }

            return databox.GetData<StringType>(DataBaseController.Localisation_Table, key, languageCode.Value).Value;
        }
    }
}
