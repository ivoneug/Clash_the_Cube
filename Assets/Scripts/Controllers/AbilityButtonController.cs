using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Events;
using Databox;

namespace ClashTheCube
{
    public enum AbilityType
    {
        None,
        SwitchCube,
        DropBomb,
        SuperMagnete
    }

    public class AbilityButtonController : MonoBehaviour
    {
        private static Dictionary<AbilityType, string> AbilityTypeFieldMap = new Dictionary<AbilityType, string>{
            { AbilityType.SwitchCube, DataBaseController.Purchases_SwitchCube },
            { AbilityType.DropBomb, DataBaseController.Purchases_DropBomb },
            { AbilityType.SuperMagnete, DataBaseController.Purchases_SuperMagnete }
        };

        [SerializeField] private DataboxObject databox;
        [SerializeField] LocalisationController localisationController;
        [SerializeField] private AbilityType abilityType = AbilityType.None;
        [SerializeField] private int defaultCount = 0;
        [SerializeField] private UILabel countLabel;
        [SerializeField] private UILabel nameLabel;
        [SerializeField] private RectTransform addIcon;
        [SerializeField] private GameEvent activateAbilityEvent;
        [SerializeField] private GameEvent notEnoughAbilityEvent;

        private int count = 0;

        public void LoadSavedState()
        {
            Load();
            UpdateUI();
        }

        public void TryActivate()
        {
            GameEvent gameEvent = null;

            if (count > 0)
            {
                count--;
                gameEvent = activateAbilityEvent;
            } else
            {
                gameEvent = notEnoughAbilityEvent;
            }

            Save();
            UpdateUI();

            if (gameEvent)
            {
                gameEvent.Raise();
            }
        }

        public void UpdateUI()
        {
            localisationController.LocalizeLabel(nameLabel, AbilityTypeFieldMap[abilityType]);
            countLabel.text = string.Format("x{0}", count);
            addIcon.gameObject.SetActive(count == 0);
        }

        private void Load()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Purchases_Entry;
            var field = AbilityTypeFieldMap[abilityType];

            if (string.IsNullOrEmpty(field))
            {
                Debug.LogError("Unable to get DB entry field for ability: " + abilityType);
                return;
            }

            if (!databox.EntryExists(table, entry) || !databox.ValueExists(table, entry, field))
            {
                count = defaultCount;
                databox.AddData(table, entry, field, new IntType(count));
            }
            else
            {
                count = databox.GetData<IntType>(table, entry, field).Value;
            }
        }

        private void Save()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Purchases_Entry;
            var field = AbilityTypeFieldMap[abilityType];

            if (string.IsNullOrEmpty(field))
            {
                Debug.LogError("Unable to get DB entry field for ability: " + abilityType);
                return;
            }

            databox.SetData<IntType>(table, entry, field, new IntType(count));
            databox.SaveDatabase();
        }
    }
}
