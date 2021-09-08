using Orcas.Iap.Interface;
using UnityEngine;

namespace Orcas.Iap
{
    public class GooglePlayListener : MonoBehaviour, IPurchaseNativeEventListener
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void OnInitialized(string info)
        {
            NativePurchasing.OnInitialized(info);
        }

        public void OnInitializedFailed(string info)
        {
            NativePurchasing.OnInitializedFailed(info);
        }

        public void OnQueryInventoryFinished(string inventory)
        {
            if (string.IsNullOrEmpty(inventory) == false)
            {
                var skuMessages = inventory.Split('|');
                for (var i = 0; i < skuMessages.Length; i++)
                {
                    var msg = skuMessages[i];
                    var index = msg.IndexOf("!");
                    if (index < 0) continue;
                    var sku = msg.Substring(0, index);
                    var price = msg.Substring(index + 1);
                    if (NativePurchasing.PriceMap.ContainsKey(sku) == false)
                    {
                        NativePurchasing.PriceMap.Add(sku, price);
                    }
                }
            }
            NativePurchasing.OnQueryInventoryFinished(inventory);
        }

        public void OnQueryInventoryFailed(string info)
        {
            NativePurchasing.OnQueryInventoryFailed(info);
        }

        public void OnPurchaseSuccess(string info)
        {
            var purchase = new GooglePurchaseData(info);
            var result = new PurchaseResult
            {
                SkuId = purchase.Sku,
                ServerOrderId = purchase.DeveloperPayload,
                Receipt = purchase.Token,
                StoreOrderId = purchase.OrderId,
                Token = NativePurchasing.Base64Encode(info),
                PurchaseType = 1,
            };
            NativePurchasing.OnPurchaseSuccess(result);
        }

        public void OnPurchaseCancelled(string info)
        {
            NativePurchasing.OnPurchaseCancelled(info);
        }

        public void OnPurchaseConnectFailed(string info)
        {
            NativePurchasing.OnPurchaseConnectFailed(info);
        }

        public void OnPurchaseUnknownFailed(string info)
        {
            NativePurchasing.OnPurchaseOtherFailed(info);
        }

        public void ConsumeSuccess(string orderId)
        {
            NativePurchasing.OnConsumeSuccess(orderId);
        }

        public void ConsumeFailed(string orderId)
        {
            NativePurchasing.OnConsumeFailed(orderId);
        }
    }

}
