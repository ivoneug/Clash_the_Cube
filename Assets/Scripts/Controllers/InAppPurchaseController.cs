using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using Framework.Events;

public class InAppPurchaseController : MonoBehaviour
{
    // [SerializeField]
    // private TopicInfo buyTopicInfo;

    [SerializeField]
    private GameEvent purchaseCompletedEvent;

    [SerializeField]
    private GameEvent purchaseFailedEvent;

    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
    }

    private void OnEnable()
    {
        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;

        InAppPurchasing.RestoreCompleted += RestoreCompletedHandler;
        InAppPurchasing.RestoreFailed += RestoreFailedHandler;
    }

    private void OnDisable()
    {
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;

        InAppPurchasing.RestoreCompleted -= RestoreCompletedHandler;
        InAppPurchasing.RestoreFailed -= RestoreFailedHandler;
    }

    public void PurchaseTopic()
    {
        // InAppPurchasing.PurchaseWithId(buyTopicInfo.inAppId);
    }

    public bool IsProductOwned()
    {
        return true;
        // return InAppPurchasing.IsProductWithIdOwned(buyTopicInfo.inAppId);
    }

    public void RestorePurchases()
    {
        InAppPurchasing.RestorePurchases();
    }

    private void PurchaseCompletedHandler(IAPProduct product)
    {
        // buyTopicInfo.CloneFrom(product);
        purchaseCompletedEvent.Raise();

        Debug.Log(product.Name + " was purchased.");
    }

    private void PurchaseFailedHandler(IAPProduct product, string info)
    {
        // buyTopicInfo.CloneFrom(product);
        purchaseFailedEvent.Raise();

        Debug.Log("The purchase of product " + product.Name + " has failed.");
    }

    void RestoreCompletedHandler()
    {
        Debug.Log("All purchases have been restored successfully.");
    }

    void RestoreFailedHandler()
    {
        Debug.Log("The purchase restoration has failed.");
    }
}
