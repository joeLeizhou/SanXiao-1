using UnityEngine;

namespace Orcas.Iap
{
    public static partial class NativePurchasing
    {
        /// <summary>
        /// 初始化成功事件
        /// </summary>
        /// <param name="info"></param>
        internal static void OnInitialized(string info)
        {
            _eventListener.OnInitialized(info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnInitialized " + info);
#endif
        }

        /// <summary>
        /// 初始化失败事件，业务层自行制定重连策略
        /// </summary>
        /// <param name="info"></param>
        internal static void OnInitializedFailed(string info)
        {
            _eventListener.OnInitializedFailed(info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnInitializedFailed " + info);
#endif
        }

        /// <summary>
        /// 获取商品信息成功
        /// </summary>
        /// <param name="inventory"></param>
        internal static void OnQueryInventoryFinished(string inventory)
        {
            _eventListener.OnQueryInventoryFinished(inventory);
#if ADDLOG
            Debug.Log("[OrcasIap] OnQueryInventoryFinished " + inventory);
#endif
        }

        /// <summary>
        /// 获取商品信息失败
        /// </summary>
        /// <param name="info"></param>
        internal static void OnQueryInventoryFailed(string info)
        {
            _eventListener.OnQueryInventoryFailed(info);
#if ADDLOG
            Debug.Log("[OrcasIap] QueryInventoryFailedEvent " + info);
#endif
        }

        
        /// <summary>
        /// 购买成功
        /// </summary>
        /// <param name="info"></param>
        internal static void OnPurchaseSuccess(PurchaseResult info)
        {
            LastOrderId = info.StoreOrderId;
            _eventListener.OnPurchaseSuccess(info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnPurchaseSuccess sku = " + info.SkuId + ", StoreOrderId = " + info.StoreOrderId + ", ServerOrderId = " + info.ServerOrderId + ", Token = " + info.Token + ", Rececipt=" + info.Receipt);
#endif
        }

        /// <summary>
        /// 用户取消购买
        /// </summary>
        /// <param name="info"></param>
        internal static void OnPurchaseCancelled(string info)
        {
            _eventListener.OnPurchaseFailed(PurchaseFailedType.UserCancelled, info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnPurchaseCancelled " + info);
#endif
        }

        /// <summary>
        /// 由于连接不上商店服务器导致的购买失败
        /// </summary>
        /// <param name="info"></param>
        internal static void OnPurchaseConnectFailed(string info)
        {
            _eventListener.OnPurchaseFailed(PurchaseFailedType.ServiceUnavailable, info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnPurchaseConnectFailed " + info);
#endif
        }

        /// <summary>
        /// 其他原因导致的购买失败
        /// </summary>
        /// <param name="info"></param>
        internal static void OnPurchaseOtherFailed(string info)
        {
            _eventListener.OnPurchaseFailed(PurchaseFailedType.Other, info);
#if ADDLOG
            Debug.Log("[OrcasIap] OnPurchaseOtherFailed " + info);
#endif
        }

        /// <summary>
        /// 消耗商品失败
        /// </summary>
        /// <param name="orderId"></param>
        internal static void OnConsumeFailed(string orderId)
        {
            _eventListener.OnConsumeFailed(orderId);
#if ADDLOG
            Debug.Log("[OrcasIap] OnConsumeFailed " + orderId);
#endif
        }

        /// <summary>
        /// 消耗商品成功
        /// </summary>
        /// <param name="orderId"></param>
        internal static void OnConsumeSuccess(string orderId)
        {
            _eventListener.OnConsumeSuccess(orderId);
#if ADDLOG
            Debug.Log("[OrcasIap] OnConsumeSuccess " + orderId);
#endif
        }
    }
}