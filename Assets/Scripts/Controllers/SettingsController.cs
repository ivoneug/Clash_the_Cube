using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Framework.Variables;
using Framework.Events;
using Doozy.Engine.UI;
using Databox;

namespace ClashTheCube
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;
        [SerializeField] private GameEvent settingsChangedEvent;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] [Range(-80f, 40f)] private float musicVolume = -20f;
        [SerializeField] [Range(-80f, 40f)] private float sfxVolume = 0f;

        private float zeroVolume = -80f;

        [SerializeField] private BoolReference isMusicOn;
        [SerializeField] private BoolReference isSfxOn;
        [SerializeField] private BoolReference isVibrationOn;
        [SerializeField] private UIToggle musicToggle;
        [SerializeField] private UIToggle sfxToggle;
        [SerializeField] private UIToggle vibrationToggle;

        private bool settingsLoaded = false;

        // [SerializeField]
        // private SettingsInfo settingsInfo;

        // [SerializeField]
        // private GameEvent settingsLoadedEvent;

        // [SerializeField] private TextMeshProUGUI splashStart;

        // [SerializeField] private TextMeshProUGUI totalWordCount;
        // [SerializeField] private TextMeshProUGUI purchases;
        // [SerializeField] private TextMeshProUGUI restorePurchases;
        // [SerializeField] private TextMeshProUGUI generalSettings;
        // [SerializeField] private TextMeshProUGUI language;
        // [SerializeField] private TextMeshProUGUI voiceType;
        // [SerializeField] private TextMeshProUGUI voiceTypeFemale;
        // [SerializeField] private TextMeshProUGUI voiceTypeMale;
        // [SerializeField] private TextMeshProUGUI remindMeToStudy;
        // [SerializeField] private TextMeshProUGUI onceADay;
        // [SerializeField] private TextMeshProUGUI onceAWeek;
        // [SerializeField] private TextMeshProUGUI never;
        // [SerializeField] private TextMeshProUGUI buyPopupCancelNormal;
        // [SerializeField] private TextMeshProUGUI buyPopupCancelHighlighted;

        // [SerializeField] private TextMeshProUGUI swipeLeftHint;
        // [SerializeField] private TextMeshProUGUI swipeRightHint;

        // [SerializeField] private TextMeshProUGUI onboardingWelcome;
        // [SerializeField] private TextMeshProUGUI onboardingNext1;
        // [SerializeField] private TextMeshProUGUI onboardingNext2;
        // [SerializeField] private TextMeshProUGUI onboardingNext3;
        // [SerializeField] private TextMeshProUGUI onboardingFinish;
        // [SerializeField] private Image onboardingPage1ImageEN;
        // [SerializeField] private Image onboardingPage2ImageEN;
        // [SerializeField] private Image onboardingPage3ImageEN;
        // [SerializeField] private Image onboardingPage1ImageRU;
        // [SerializeField] private Image onboardingPage2ImageRU;
        // [SerializeField] private Image onboardingPage3ImageRU;

        private void Start()
        {
            LoadSettings();
            // settingsInfo.Load();
            // UpdateUI();

            // settingsLoadedEvent.Raise();
        }

        // public void UpdateUI()
        // {
        //     LocalizeLabel(splashStart, nameof(splashStart));
        //     LocalizeLabel(totalWordCount, nameof(totalWordCount));
        //     LocalizeLabel(purchases, nameof(purchases));
        //     LocalizeLabel(restorePurchases, nameof(restorePurchases));
        //     LocalizeLabel(generalSettings, nameof(generalSettings));
        //     LocalizeLabel(language, nameof(language));
        //     LocalizeLabel(voiceType, nameof(voiceType));
        //     LocalizeLabel(voiceTypeFemale, nameof(voiceTypeFemale));
        //     LocalizeLabel(voiceTypeMale, nameof(voiceTypeMale));
        //     LocalizeLabel(remindMeToStudy, nameof(remindMeToStudy));
        //     LocalizeLabel(onceADay, nameof(onceADay));
        //     LocalizeLabel(onceAWeek, nameof(onceAWeek));
        //     LocalizeLabel(never, nameof(never));
        //     LocalizeLabel(buyPopupCancelNormal, "buyPopupCancel");
        //     LocalizeLabel(buyPopupCancelHighlighted, "buyPopupCancel");

        //     LocalizeLabel(swipeLeftHint, nameof(swipeLeftHint));
        //     LocalizeLabel(swipeRightHint, nameof(swipeRightHint));

        //     LocalizeLabel(onboardingWelcome, nameof(onboardingWelcome));
        //     LocalizeLabel(onboardingNext1, "onboardingNext");
        //     LocalizeLabel(onboardingNext2, "onboardingNext");
        //     LocalizeLabel(onboardingNext3, "onboardingNext");
        //     LocalizeLabel(onboardingFinish, nameof(onboardingFinish));

        //     var lang = settingsInfo.languageCode;

        //     if (onboardingPage1ImageEN && onboardingPage2ImageEN && onboardingPage3ImageEN &&
        //         onboardingPage1ImageRU && onboardingPage2ImageRU && onboardingPage3ImageRU)
        //     {
        //         onboardingPage1ImageEN.gameObject.SetActive(lang == "en");
        //         onboardingPage2ImageEN.gameObject.SetActive(lang == "en");
        //         onboardingPage3ImageEN.gameObject.SetActive(lang == "en");

        //         onboardingPage1ImageRU.gameObject.SetActive(lang == "ru");
        //         onboardingPage2ImageRU.gameObject.SetActive(lang == "ru");
        //         onboardingPage3ImageRU.gameObject.SetActive(lang == "ru");
        //     }
        // }

        // private void LocalizeLabel(TextMeshProUGUI label, string key)
        // {
        //     if (!label)
        //     {
        //         return;
        //     }

        //     string text = settingsInfo.GetLocalizedText(key);
        //     if (text == string.Empty)
        //     {
        //         Debug.LogWarning("Unable to localize \"" + key + "\" to \"" + settingsInfo.languageCode + "\": no localized text found");
        //         return;
        //     }

        //     label.text = text;
        // }

        #region Settings

        private void LoadSettings()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Settings_Entry;
            var isMusicOnField = DataBaseController.Settings_isMusicOnField;
            var isSfxOnField = DataBaseController.Settings_isSfxOnField;
            var isVibrationOnField = DataBaseController.Settings_isVibrationOnField;

            if (!databox.EntryExists(table, entry))
            {
                isMusicOn.Variable.SetValue(true);
                isSfxOn.Variable.SetValue(true);
                isVibrationOn.Variable.SetValue(true);

                SaveSettings();
            }
            else
            {
                isMusicOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isMusicOnField).Value);
                isSfxOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isSfxOnField).Value);
                isVibrationOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isVibrationOnField).Value);
            }

            UpdateAudioState();

            musicToggle.IsOn = isMusicOn;
            sfxToggle.IsOn = isSfxOn;
            vibrationToggle.IsOn = isVibrationOn;

            RaiseSettingsEvent();
            settingsLoaded = true;
        }

        private void SaveSettings()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Settings_Entry;
            var isMusicOnField = DataBaseController.Settings_isMusicOnField;
            var isSfxOnField = DataBaseController.Settings_isSfxOnField;
            var isVibrationOnField = DataBaseController.Settings_isVibrationOnField;

            var isMusicOnValue = new BoolType(isMusicOn);
            var isSfxOnValue = new BoolType(isSfxOn);
            var isVibrationOnValue = new BoolType(isVibrationOn);

            if (!databox.EntryExists(table, entry))
            {
                databox.AddData(table, entry, isMusicOnField, isMusicOnValue);
                databox.AddData(table, entry, isSfxOnField, isSfxOnValue);
                databox.AddData(table, entry, isVibrationOnField, isVibrationOnValue);
            }
            else
            {
                databox.SetData<BoolType>(table, entry, isMusicOnField, isMusicOnValue);
                databox.SetData<BoolType>(table, entry, isSfxOnField, isSfxOnValue);
                databox.SetData<BoolType>(table, entry, isVibrationOnField, isVibrationOnValue);
            }

            databox.SaveDatabase();
        }

        private void UpdateAudioState()
        {
            audioMixer.SetFloat("Music_Volume", isMusicOn ? musicVolume : zeroVolume);
            audioMixer.SetFloat("SFX_Volume", isSfxOn ? sfxVolume : zeroVolume);
        }

        private void RaiseSettingsEvent()
        {
            if (settingsChangedEvent)
            {
                settingsChangedEvent.Raise();
            }
        }

        public void MuteMusic()
        {
            if (!settingsLoaded)
            {
                return;
            }

            isMusicOn.Variable.SetValue(false);

            UpdateAudioState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        public void UnmuteMusic()
        {
            if (!settingsLoaded)
            {
                return;
            }

            isMusicOn.Variable.SetValue(true);

            UpdateAudioState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        public void MuteSFX()
        {
            if (!settingsLoaded)
            {
                return;
            }

            isSfxOn.Variable.SetValue(false);

            UpdateAudioState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        public void UnmuteSFX()
        {
            if (!settingsLoaded)
            {
                return;
            }

            isSfxOn.Variable.SetValue(true);

            UpdateAudioState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        public void SetVibrationOn()
        {
            if (!settingsLoaded)
            {
                return;
            }

            isVibrationOn.Variable.SetValue(true);

            SaveSettings();
            RaiseSettingsEvent();
        }

        public void SetVibrationOff()
        {
            if (!settingsLoaded)
            {
                return;
            }
            
            isVibrationOn.Variable.SetValue(false);

            SaveSettings();
            RaiseSettingsEvent();
        }

        #endregion
    }
}
