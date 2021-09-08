using Orcas.Iap.Interface;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Orcas.Iap
{
    public class AppStorePurchaseBridge : IPurchaseBridge
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void InitIAPManager(); //初始化
        [DllImport("__Internal")]
        private static extern void RestoreTransactions(); //初始化
        [DllImport("__Internal")]
        private static extern bool IsProductAvailable(); //判断是否可以购买
        [DllImport("__Internal")]
        private static extern void RequstProductInfo(string s); //获取商品信息
        [DllImport("__Internal")]
        private static extern void BuyProduct(string s, string data); //购买商品
        [DllImport("__Internal")]
        private static extern void FinishTransaction(string data); //消耗订单
#endif

        private string _skuIds;

        public void Init()
        {
            var obj = new GameObject("OrcasIAP");
            obj.AddComponent<AppleStoreListener>();
#if UNITY_IOS && !UNITY_EDITOR
                InitIAPManager();
#endif
        }

        public void ReconnectStore()
        {
        }

        public void QueryInventory()
        {
#if UNITY_IOS && !UNITY_EDITOR
            RequstProductInfo(_skuIds);
#endif
        }

        public void RestorePurchase()
        {
#if UNITY_IOS && !UNITY_EDITOR
            RestoreTransactions();
#endif
        }

        public void Purchase(string skuId, string extraData)
        {
#if UNITY_IOS && !UNITY_EDITOR
            BuyProduct(skuId, extraData);
#endif
        }

        public void Consume(string storeOrderId)
        {
#if UNITY_IOS && !UNITY_EDITOR
            FinishTransaction(storeOrderId);
#endif
        }

        public bool CheckServiceAvailable()
        {
            return true;
        }

        public void SetSkuIdList(string skuIdList)
        {
            _skuIds = skuIdList;
        }
    }
}