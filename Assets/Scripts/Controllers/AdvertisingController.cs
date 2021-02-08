using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public class AdvertisingController : MonoBehaviour
    {
        [SerializeField] private GameObject removeAdsButton;

        private void Start()
        {
            RefreshADs();
        }

        public void RefreshADs()
        {
            if (InAppPurchaseController.Instance.IsRemoveADsProductOwned())
            {
                removeAdsButton.SetActive(false);
            }
            else
            {
                removeAdsButton.SetActive(true);
            }
        }
    }
}
