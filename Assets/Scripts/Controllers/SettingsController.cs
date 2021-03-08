using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using Framework.Variables;
using Framework.Events;
using Doozy.Engine.UI;
using Databox;

namespace ClashTheCube
{
    public class SettingsController : MonoBehaviour
    {
        private const string enLanguageCode = "en";
        private const string ruLanguageCode = "ru";

        [SerializeField] private DataboxObject databox;
        [SerializeField] private GameEvent settingsChangedEvent;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] [Range(-80f, 40f)] private float musicVolume = -20f;
        [SerializeField] [Range(-80f, 40f)] private float sfxVolume = 0f;

        private float zeroVolume = -80f;

        [SerializeField] private BoolReference isMusicOn;
        [SerializeField] private IntReference musicIndex;
        [SerializeField] private BoolReference isSfxOn;
        [SerializeField] private BoolReference isVibrationOn;
        [SerializeField] private StringReference languageCode;
        [SerializeField] private UIToggle musicToggle;
        [SerializeField] private UIToggle sfxToggle;
        [SerializeField] private UIToggle vibrationToggle;
        [SerializeField] private GameObject ruFlagCheckmark;
        [SerializeField] private GameObject enFlagCheckmark;

        private bool settingsLoaded = false;

        private void Start()
        {
            LoadSettings();
        }

        #region Settings

        private void LoadSettings()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Settings_Entry;
            var isMusicOnField = DataBaseController.Settings_isMusicOnField;
            var musicIndexField = DataBaseController.Settings_musicIndexField;
            var isSfxOnField = DataBaseController.Settings_isSfxOnField;
            var isVibrationOnField = DataBaseController.Settings_isVibrationOnField;
            var languageCodeField = DataBaseController.Settings_LanguageCodeField;

            // default values
            isMusicOn.Variable.SetValue(true);
            musicIndex.Variable.SetValue(0);
            isSfxOn.Variable.SetValue(true);
            isVibrationOn.Variable.SetValue(true);
            languageCode.Variable.SetValue(GetLanguage());

            if (!databox.EntryExists(table, entry))
            {
                SaveSettings();
            }
            else
            {
                isMusicOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isMusicOnField).Value);
                musicIndex.Variable.SetValue(databox.GetData<IntType>(table, entry, musicIndexField).Value);
                isSfxOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isSfxOnField).Value);
                isVibrationOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isVibrationOnField).Value);
                languageCode.Variable.SetValue(databox.GetData<StringType>(table, entry, languageCodeField).Value);
            }

            // set music always on
            isMusicOn.Variable.SetValue(true);

            UpdateAudioState();
            UpdateLanguageState();

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
            var musicIndexField = DataBaseController.Settings_musicIndexField;
            var isSfxOnField = DataBaseController.Settings_isSfxOnField;
            var isVibrationOnField = DataBaseController.Settings_isVibrationOnField;
            var languageCodeField = DataBaseController.Settings_LanguageCodeField;

            var isMusicOnValue = new BoolType(isMusicOn);
            var musicIndexValue = new IntType(musicIndex);
            var isSfxOnValue = new BoolType(isSfxOn);
            var isVibrationOnValue = new BoolType(isVibrationOn);
            var languageCodeValue = new StringType(languageCode);

            if (!databox.EntryExists(table, entry))
            {
                databox.AddData(table, entry, isMusicOnField, isMusicOnValue);
                databox.AddData(table, entry, musicIndexField, musicIndexValue);
                databox.AddData(table, entry, isSfxOnField, isSfxOnValue);
                databox.AddData(table, entry, isVibrationOnField, isVibrationOnValue);
                databox.AddData(table, entry, languageCodeField, languageCodeValue);
            }
            else
            {
                databox.SetData<BoolType>(table, entry, isMusicOnField, isMusicOnValue);
                databox.SetData<BoolType>(table, entry, isSfxOnField, isSfxOnValue);
                databox.SetData<BoolType>(table, entry, isVibrationOnField, isVibrationOnValue);
                databox.SetData<StringType>(table, entry, languageCodeField, languageCodeValue);

                if (!databox.ValueExists(table, entry, musicIndexField))
                {
                    databox.AddData(table, entry, musicIndexField, musicIndexValue);
                }
                else
                {
                    databox.SetData<IntType>(table, entry, musicIndexField, musicIndexValue);
                }
            }

            databox.SaveDatabase();
        }

        public void StoreSettings()
        {
            if (!settingsLoaded)
            {
                return;
            }

            SaveSettings();
            RaiseSettingsEvent();
        }

        private void UpdateAudioState()
        {
            audioMixer.SetFloat("Music_Volume", isMusicOn ? musicVolume : zeroVolume);
            audioMixer.SetFloat("SFX_Volume", isSfxOn ? sfxVolume : zeroVolume);
        }

        private void UpdateLanguageState()
        {
            ruFlagCheckmark.SetActive(languageCode == ruLanguageCode);
            enFlagCheckmark.SetActive(languageCode == enLanguageCode);
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

        public void SetEnglishLanguage()
        {
            if (!settingsLoaded)
            {
                return;
            }

            languageCode.Variable.SetValue(enLanguageCode);

            UpdateLanguageState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        public void SetRussianLanguage()
        {
            if (!settingsLoaded)
            {
                return;
            }

            languageCode.Variable.SetValue(ruLanguageCode);

            UpdateLanguageState();
            SaveSettings();
            RaiseSettingsEvent();
        }

        #endregion

        #region Language

        private string GetLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    return enLanguageCode;

                case SystemLanguage.Russian:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Belarusian:
                    return ruLanguageCode;

                default:
                    return enLanguageCode;
            }
        }

        #endregion
    }
}
