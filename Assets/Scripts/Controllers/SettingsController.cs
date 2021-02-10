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
        [SerializeField] private DataboxObject databox;
        [SerializeField] private GameEvent settingsChangedEvent;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] [Range(-80f, 40f)] private float musicVolume = -20f;
        [SerializeField] [Range(-80f, 40f)] private float sfxVolume = 0f;

        private float zeroVolume = -80f;

        [SerializeField] private BoolReference isMusicOn;
        [SerializeField] private BoolReference isSfxOn;
        [SerializeField] private BoolReference isVibrationOn;
        [SerializeField] private StringReference languageCode;
        [SerializeField] private UIToggle musicToggle;
        [SerializeField] private UIToggle sfxToggle;
        [SerializeField] private UIToggle vibrationToggle;

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
            var isSfxOnField = DataBaseController.Settings_isSfxOnField;
            var isVibrationOnField = DataBaseController.Settings_isVibrationOnField;
            var languageCodeField = DataBaseController.Settings_LanguageCodeField;

            if (!databox.EntryExists(table, entry))
            {
                isMusicOn.Variable.SetValue(true);
                isSfxOn.Variable.SetValue(true);
                isVibrationOn.Variable.SetValue(true);
                languageCode.Variable.SetValue(GetLanguage());

                SaveSettings();
            }
            else
            {
                isMusicOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isMusicOnField).Value);
                isSfxOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isSfxOnField).Value);
                isVibrationOn.Variable.SetValue(databox.GetData<BoolType>(table, entry, isVibrationOnField).Value);
                languageCode.Variable.SetValue(databox.GetData<StringType>(table, entry, languageCodeField).Value);
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
            var languageCodeField = DataBaseController.Settings_LanguageCodeField;

            var isMusicOnValue = new BoolType(isMusicOn);
            var isSfxOnValue = new BoolType(isSfxOn);
            var isVibrationOnValue = new BoolType(isVibrationOn);
            var languageCodeValue = new StringType(languageCode);

            if (!databox.EntryExists(table, entry))
            {
                databox.AddData(table, entry, isMusicOnField, isMusicOnValue);
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

        #region Language

        private string GetLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    return "en";

                case SystemLanguage.Russian:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Belarusian:
                    return "ru";

                default:
                    return "en";
            }
        }

        #endregion
    }
}
