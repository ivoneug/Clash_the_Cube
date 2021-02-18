using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using Framework.Events;
using Framework.SystemInfo;

namespace ClashTheCube
{
    public class AdvertisingController : MonoBehaviour
    {
        [SerializeField] private GameEvent adCompletedEvent;
        [SerializeField] private GameEvent adSkippedEvent;

        [SerializeField] private GameObject removeAdsButton;

        private void Start()
        {
            RefreshADs();
        }

        private void OnEnable()
        {
            Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
            Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
        }

        private void OnDisable()
        {
            Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;
            Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
        }

        private bool IsADsSupported()
        {
            return Platform.IsMobilePlatform();
        }

        public void RefreshADs()
        {
            if (!IsADsSupported())
            {
                removeAdsButton.SetActive(false);
                return;
            }

            bool isADsVisible = true;

            if (Advertising.IsAdRemoved())
            {
                isADsVisible = false;
            }
            else if (InAppPurchaseController.Instance.IsRemoveADsProductOwned())
            {
                Advertising.RemoveAds();
                isADsVisible = false;
            }

            removeAdsButton.SetActive(isADsVisible);
        }

        public void ShowAchievementAD()
        {
            if (!Advertising.IsInterstitialAdReady() || !IsADsSupported())
            {
                if (adCompletedEvent)
                {
                    adCompletedEvent.Raise();
                }
                return;
            }

            Advertising.ShowInterstitialAd();
        }

        public void ShowContinueGameAD()
        {
            if (!Advertising.IsRewardedAdReady() || !IsADsSupported())
            {
                if (adCompletedEvent)
                {
                    adCompletedEvent.Raise();
                }
                return;
            }

            Advertising.ShowRewardedAd();
        }

        private void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Interstitial ad has been closed.");

            if (adCompletedEvent)
            {
                adCompletedEvent.Raise();
            }
        }

        private void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Rewarded ad has completed. The user should be rewarded now.");

            if (adCompletedEvent)
            {
                adCompletedEvent.Raise();
            }
        }

        private void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");

            if (adSkippedEvent)
            {
                adSkippedEvent.Raise();
            }
        }
    }
}
