using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

#if EM_UIAP
using UnityEngine.Purchasing;
#endif

namespace ClashTheCube
{
    public class BuyPopupUIController : MonoBehaviour
    {
        [SerializeField] private UILabel priceLabel;

        private void OnEnable()
        {
            string localizedPrice = "0 RUB";

#if EM_UIAP
            ProductMetadata data = InAppPurchasing.GetProductLocalizedData(EM_IAPConstants.Product_Remove_ADs);
            if (data != null)
            {
                localizedPrice = data.localizedPriceString;
            }
#endif

            priceLabel.text = localizedPrice;
        }
    }
}
