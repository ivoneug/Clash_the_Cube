﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Events;
using Databox;

namespace ClashTheCube
{
    public class DataBaseController : MonoBehaviour
    {
        public static string Cubes_Table = "Cubes";
        public static string Cubes_StateField = "StateField";
        public static string Cubes_NumberField = "Number";
        public static string Cubes_PositionField = "PositionField";
        public static string Cubes_RotationField = "RotationField";

        public static string Data_Table = "Data";
        public static string Stats_Entry = "Stats";
        public static string Stats_ScoreField = "Score";

        public static string Settings_Entry = "Settings";
        public static string Settings_isMusicOnField = "isMusicOn";
        public static string Settings_isSfxOnField = "isSfxOnField";
        public static string Settings_isVibrationOnField = "isVibrationOn";

        [SerializeField] private DataboxObject databox;
        [SerializeField] private GameEvent dataBaseReadyEvent;
        [SerializeField][Range(0, 30)] private int saveDatabaseDelay = 5;

        private bool isDataBaseReady = false;

        private void Start()
        {
            string path = databox.ReturnSavePath(databox.fileName);
            if (!System.IO.File.Exists(path))
            {
                databox.AddDatabaseTable(Cubes_Table);
                databox.AddDatabaseTable(Data_Table);

                databox.SaveDatabase();
            }

            databox.OnDatabaseLoaded += OnDatabaseLoaded;
            databox.LoadDatabase();
        }

        private void OnDatabaseLoaded()
        {
            isDataBaseReady = true;
            dataBaseReadyEvent.Raise();
        }

        private void OnApplicationQuit()
        {
            SaveDatabase();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                return;
            }

            SaveDatabase();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                return;
            }

            SaveDatabase();
        }

        public void SaveDatabaseDelayed()
        {
            CancelInvoke("SaveDatabase");
            Invoke("SaveDatabase", saveDatabaseDelay);
        }

        private void SaveDatabase()
        {
            if (!isDataBaseReady)
            {
                return;
            }
            CancelInvoke("SaveDatabase");

            databox.SaveDatabase();
            Debug.Log("DB Saved");
        }
    }
}
