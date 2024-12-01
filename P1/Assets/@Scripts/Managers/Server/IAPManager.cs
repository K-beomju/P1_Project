using UnityEngine;
using UnityEngine.Purchasing;
using System;
using UnityEngine.Purchasing.Extension;
using System.Collections.Generic;
using static Define;

public class IAPManager : IDetailedStoreListener
{
    /// Consumable	    사용 후 소모됨, 반복 구매 가능	포션, 에너지, 게임 화폐
    /// NonConsumable	영구적으로 사용 가능, 한 번만 구매	광고 제거, 스킨, 확장팩
    /// Subscription	일정 기간 동안 혜택 제공, 기간 종료 시 갱신 필요	VIP 멤버십, 배틀 패스, 매월 지급되는 리소스
    public const string productIDConsumable = "consumable";
    public const string productIDNonConsumable = "nonconsumable";
    public const string productIDSubscription = "subscription";

    private IStoreController storeController;

    private readonly Dictionary<string, Action> purchaseHandlers = new Dictionary<string, Action>
    {
        { ProductIDs.Dia40, () => Debug.Log("다이아 40개 구매 성공") },
        { ProductIDs.Dia220, () => Debug.Log("다이아 220개 구매 성공") },
        { ProductIDs.Dia480, () => Debug.Log("다이아 480개 구매 성공") },
        { ProductIDs.Dia1040, () => Debug.Log("다이아 1040개 구매 성공") },
        { ProductIDs.Dia2800, () => Debug.Log("다이아 2800개 구매 성공") },
        { ProductIDs.Dia6400, () => Debug.Log("다이아 6400개 구매 성공") },
        { ProductIDs.Gold10000, () => Debug.Log("골드 10000개 구매 성공") },
        { ProductIDs.Gold100000, () => Debug.Log("골드 100000개 구매 성공") }
    };



    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach(var purchase in purchaseHandlers)
        {
            builder.AddProduct(purchase.Key, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);

    }



    // OnInitialized : Unity IAP가 모든 제품 메타 데이터를 검색하여 구매할 준비가되면 호출된다.
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.LogWarning("유니티 IAP 초기화 성공");
        storeController = controller;

        var dia_40 = controller.products.WithID("dia40");
        if (dia_40 != null && dia_40.availableToPurchase)
        {
            Debug.Log("다이아 40개 구매 가능");
        }
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
        Debug.LogWarning($"유니티 초기화 실패 : {error}, Message: {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"구매 실패 - {product.definition.id}, 이유: {failureDescription.reason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"구매 실패 - {product.definition.id}, {failureReason}");
    }

    // ProcessPurchase : 구매가 성공하면 호출된다.
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var productID = purchaseEvent.purchasedProduct.definition.id;
        if (purchaseHandlers.TryGetValue(productID, out var handler))
        {
            handler.Invoke();
        }
        else
        {
            Debug.LogWarning($"알 수 없는 제품 ID - {productID}");
        }

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

    // 구독형
    private void CheckNonConsumable(string id)
    {
        var product = storeController.products.WithID(id);
        if (product == null)
        {
            Debug.LogWarning($"상품 {id}을 찾을 수 없습니다.");
            return;
        }

        bool hasReceipt = product.hasReceipt;
        Debug.Log($"상품 {id}에 대한 영수증 존재 여부: {hasReceipt}");

        if (hasReceipt)
        {
            Debug.Log("구매 기록이 확인되었습니다.");
            // 추가 처리가 필요하면 여기에 구현
        }
    }
}
