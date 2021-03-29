using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Events;
using Databox;

namespace ClashTheCube
{
    public class AbilityButtonController : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;
        [SerializeField] LocalisationController localisationController;
        [SerializeField] private AbilityTypeInfo info;
        [SerializeField] private AbilityTypeInfo activeInfo;
        [SerializeField] private UILabel countLabel;
        [SerializeField] private UILabel nameLabel;
        [SerializeField] private RectTransform addIcon;
        [SerializeField] private Image icon;
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
            activeInfo.CloneFrom(info);

            if (count > 0)
            {
                if (!info.CanActivate())
                {
                    return;
                }
                
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

        public void AddPurchasedAbility()
        {
            AddAbility(info.purchaseCount);
        }

        public void AddAdAbility()
        {
            AddAbility(info.adCount);
        }

        public void AddRewardAbility()
        {
            AddAbility(info.rewardCount);
        }

        private void AddAbility(int amount)
        {
            if (info != activeInfo)
            {
                return;
            }

            count += amount;

            Save();
            UpdateUI();
        }

        public void UpdateUI()
        {
            icon.sprite = info.icon;
            localisationController.LocalizeLabel(nameLabel, info.localNameKey);
            countLabel.text = string.Format("x{0}", count);
            addIcon.gameObject.SetActive(count == 0);
        }

        private void Load()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Purchases_Entry;
            var field = info.field;

            if (string.IsNullOrEmpty(field))
            {
                Debug.LogError("Unable to get DB entry field for ability: " + info.abilityType);
                return;
            }

            if (!databox.EntryExists(table, entry) || !databox.ValueExists(table, entry, field))
            {
                count = info.defaultCount;
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
            var field = info.field;

            if (string.IsNullOrEmpty(field))
            {
                Debug.LogError("Unable to get DB entry field for ability: " + info.abilityType);
                return;
            }

            databox.SetData<IntType>(table, entry, field, new IntType(count));
            databox.SaveDatabase();
        }
    }
}
