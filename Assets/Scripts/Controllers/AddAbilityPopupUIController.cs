using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyMobile;

#if EM_UIAP
using UnityEngine.Purchasing;
#endif

namespace ClashTheCube
{
    public class AddAbilityPopupUIController : MonoBehaviour
    {
        private static Dictionary<AbilityType, string> AbilityTypeFieldMap = new Dictionary<AbilityType, string>{
            { AbilityType.SwitchCube, DataBaseController.Purchases_SwitchCube },
            { AbilityType.DropBomb, DataBaseController.Purchases_DropBomb },
            { AbilityType.SuperMagnete, DataBaseController.Purchases_SuperMagnete }
        };

        [SerializeField] LocalisationController localisationController;
        [SerializeField] private AbilityTypeInfo info;
        [SerializeField] private UILabel title;
        [SerializeField] private TextMeshProUGUI subtitle;
        [SerializeField] private UILabel viewAdLabel;
        [SerializeField] private UILabel priceLabel;
        [SerializeField] private Image icon;

        private void OnEnable()
        {
            var titleText = localisationController.GetLocalizedText("AddAbilityTitle");
            var subtitleText = localisationController.GetLocalizedText("AddAbilitySubtitle");
            var abilityName = localisationController.GetLocalizedText(info.localNameKey);

            title.text = string.Format("\"{0}\"", abilityName);
            subtitle.text = string.Format("{0} {1}x \"{2}\"", subtitleText, info.purchaseCount, abilityName);

            icon.sprite = info.icon;

            localisationController.LocalizeLabel(viewAdLabel, "AddAbilityViewAd");

            string localizedPrice = "0 RUB";

#if EM_UIAP
            ProductMetadata data = InAppPurchasing.GetProductLocalizedData(EM_IAPConstants.Product_Add_Ability);
            if (data != null)
            {
                localizedPrice = data.localizedPriceString;
            }
#endif

            priceLabel.text = localizedPrice;
        }
    }
}
