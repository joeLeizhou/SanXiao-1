using System;
using Orcas.Iap.Interface;
using UnityEngine;

namespace Orcas.Iap
{
    public class AppleStoreListener : MonoBehaviour, IPurchaseNativeEventListener
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void OnInitialized(string info)
        {
            NativePurchasing.OnInitialized("");
        }

        public void OnInitializedFailed(string info)
        {
            NativePurchasing.OnInitializedFailed(info);
        }

        public void OnQueryInventoryFinished(string inventory)
        {
            var productId = inventory.Substring(0, inventory.IndexOf(",", StringComparison.Ordinal));
            var price = inventory.Substring(inventory.IndexOf(",", StringComparison.Ordinal) + 1);
            if (NativePurchasing.PriceMap.ContainsKey(productId) == false)
            {
                NativePurchasing.PriceMap.Add(productId, price);
            }

            NativePurchasing.OnQueryInventoryFinished(inventory);
        }

        public void OnQueryInventoryFailed(string info)
        {
            NativePurchasing.OnQueryInventoryFailed(info);
        }

        public void OnPurchaseSuccess(string str)
        {
#if ADDLOG
            Debug.Log("[OrcasIap] OnPurchaseSuccess_iOS " + str);
#endif
            var productid = str.Substring(0, str.IndexOf(",", StringComparison.Ordinal));
            var orderid = str.Substring(str.IndexOf(":", StringComparison.Ordinal) + 1);
            var receipt = str.Substring(str.IndexOf(",", StringComparison.Ordinal) + 1);
            receipt = receipt.Substring(0, receipt.LastIndexOf(":", StringComparison.Ordinal));
            var strTemp = "{\"receipt-data\":\"" + receipt + "\",\"password\":\"\"}";
            var strBase64 = NativePurchasing.Base64Encode(strTemp);

            var result = new PurchaseResult
            {
                ServerOrderId = orderid,
                Receipt = strBase64,
                Token = str,
                SkuId = productid,
                StoreOrderId = orderid,
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
        public void OnRestoreSuccess(string str)
        {
#if ADDLOG
            Debug.Log("[OrcasIap] OnRestoreSuccess_iOS " + str);
#endif
            var productid = str.Substring(0, str.IndexOf(",", StringComparison.Ordinal));
            var orderid = str.Substring(str.IndexOf(":", StringComparison.Ordinal) + 1);
            var receipt = str.Substring(str.IndexOf(",", StringComparison.Ordinal) + 1);
            receipt = receipt.Substring(0, receipt.LastIndexOf(":", StringComparison.Ordinal));
            var strTemp = "{\"receipt-data\":\"" + receipt + "\",\"password\":\"\"}";
            var strBase64 = NativePurchasing.Base64Encode(strTemp);

            var result = new PurchaseResult
            {
                ServerOrderId = orderid,
                Receipt = strBase64,
                Token = str,
                SkuId = productid,
                StoreOrderId = orderid,
                PurchaseType = 2,
            };
            NativePurchasing.OnPurchaseSuccess(result);
        }

        public void OnRestoreFailed(string info)
        {
            NativePurchasing.OnPurchaseOtherFailed(info);
        }
    }
}