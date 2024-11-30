using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Analytics;
using UnityEngine.Purchasing.Security;
using System;
using UnityEngine.Purchasing.Extension;

public class IAPManager : IDetailedStoreListener
{
    public const string productIDConsumable = "consumable";
    public const string productIDNonConsumable = "nonconsumable";
    public const string productIDSubscription = "subscription";

    // Android 상품 ID
    private const string _Android_ConsumableId = "com.studio.app.consumable.android";
    private const string _Android_NonconsumableId = "com.studio.app.nonconsumable.android";
    private const string _Android_SubscriptionId = "com.studio.app.subscription.android";

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(productIDConsumable, ProductType.Consumable, new IDs() {
            {_Android_ConsumableId, GooglePlay.Name}
        });

        builder.AddProduct(productIDNonConsumable, ProductType.NonConsumable, new IDs(){
            {_Android_NonconsumableId, GooglePlay.Name }
       });

        builder.AddProduct(productIDSubscription, ProductType.Subscription, new IDs(){
        {_Android_SubscriptionId, GooglePlay.Name}
       });

        UnityPurchasing.Initialize(this, builder);
    }



    // OnInitialized : Unity IAP가 모든 제품 메타 데이터를 검색하여 구매할 준비가되면 호출된다.
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("유니티 IAP 초기화 성공");
        storeController = controller;
        storeExtensionProvider = extensions;

#if UNITY_ANDROID || UNITY_IOS
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
        AppleTangle.Data(), Application.identifier);

        var products = storeController.products.all;
        foreach (var product in products)
        {
            if (product.hasReceipt)
            {
                var result = validator.Validate(product.receipt);

                foreach (IPurchaseReceipt productReceipt in result)
                {
                    //앱스토어의 경우 GooglePlayReceipt를 AppleInAppPurchaseReceipt로 바꾸면 됩니다.
                    GooglePlayReceipt googlePlayReceipt = productReceipt as GooglePlayReceipt;
                    if (null != googlePlayReceipt)
                    {
                        Debug.Log($"Product ID : {googlePlayReceipt.productID}");
                        Debug.Log($"Purchase date : {googlePlayReceipt.purchaseDate.ToLocalTime()}");
                        Debug.Log($"Transaction ID : {googlePlayReceipt.transactionID}");
                        Debug.Log($"Purchase token : {googlePlayReceipt.purchaseToken}");
                    }
                }
            }
        }
#endif
    }


    [Obsolete]
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Obsolete 메서드에서 새로운 메서드로 위임
        OnInitializeFailed(error, "Default message from Obsolete method.");
    }

    // OnInitializeFailed : Unity IAP가 초기화에 실패할 경우 호출된다.
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        // 새로운 메서드의 구현
        Console.WriteLine($"유니티 초기화 실패 : {error}, Message: {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"구매 실패 - {product.definition.id}, {failureDescription}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"구매 실패 - {product.definition.id}, {failureReason}");
        throw new NotImplementedException();
    }

    // ProcessPurchase : 구매가 성공하면 호출된다.


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"구매 성공 - ID : {args.purchasedProduct.definition.id}");

        if (args.purchasedProduct.definition.id == "ProductDia")
            Debug.Log("다이아 구매 완료");


        return PurchaseProcessingResult.Complete;
    }

    public void Purchase(string purchaseId)
    {
        var product = storeController.products.WithID(purchaseId);
        if (product != null && product.availableToPurchase)
        {
            Debug.LogWarning($"구매 시도 - {product.availableToPurchase}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"구매 시도 불가 - {purchaseId}");
        }
    }
}
