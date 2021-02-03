using System.Collections;
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

        public static string Stats_Table = "Stats";

        public static string Settings_Table = "Settings";

        [SerializeField] private DataboxObject databox;
        [SerializeField] private GameEvent dataBaseReadyEvent;

        private void Start()
        {
            string path = databox.ReturnSavePath(databox.fileName);
            if (!System.IO.File.Exists(path))
            {
                databox.AddDatabaseTable(Cubes_Table);
                databox.AddDatabaseTable(Stats_Table);
                databox.AddDatabaseTable(Settings_Table);

                databox.SaveDatabase();
            }

            databox.OnDatabaseLoaded += OnDatabaseLoaded;
            databox.LoadDatabase();
        }

        private void OnDatabaseLoaded()
        {
            dataBaseReadyEvent.Raise();
        }
    }
}
