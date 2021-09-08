using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Orcas.Iap.Interface;
using UnityEngine;

namespace Orcas.Iap
{

    public enum PurchaseFailedType
    {
        UserCancelled,
        ServiceUnavailable,
        Other
    }

    public static partial class NativePurchasing
    {
        public static bool Initialized { get; private set; }
        public static string LastOrderId { get; private set; } = "";

        public static readonly Dictionary<string, string> PriceMap = new Dictionary<string, string>();

        private static IPurchaseBridge _bridge;
        private static IPurchaseEventListener _eventListener;

        /// <summary>
        /// 初始化内购
        /// </summary>
        public static void Init<T1, T2>() where T1 : IPurchaseBridge, new() where T2 : IPurchaseEventListener, new()
        {
            if (Initialized) return;
            _bridge = new T1();
            _eventListener = new T2();
            _bridge.Init();
            Initialized = true;
        }

        /// <summary>
        /// 重新连接到商店，连接失败时调用
        /// </summary>
        public static void ReconnectStore()
        {
            _bridge.ReconnectStore();
        }

        /// <summary>
        /// 请求商品信息
        /// </summary>
        public static void QueryInventory()
        {
            if (CheckServiceAvailable() == false)
            {
                Debug.Log("[OrcasIap] Iap Service Not Available. Please Check Google Play/App Store Service Installed");
                return;
            }
            _bridge.QueryInventory();
        }

        public static void RestorePurchase()
        {
            if (CheckServiceAvailable() == false)
            {
                Debug.Log("[OrcasIap] Iap Service Not Available. Please Check Google Play/App Store Service Installed");
                return;
            }
            _bridge.RestorePurchase();
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="skuId">商品的SkuId</param>
        /// <param name="extraData">额外的信息，如服务器产生的订单id</param>
        public static void Purchase(string skuId, string extraData)
        {
            if (CheckServiceAvailable() == false)
            {
                Debug.Log("[OrcasIap] Iap Service Not Available. Please Check Google Play/App Store Service Installed");
                return;
            }
            _bridge.Purchase(skuId, extraData);
        }

        /// <summary>
        /// 消耗订单
        /// </summary>
        /// <param name="skuId">商品id</param>
        public static void Consume(string storeOrderId)
        {
            if (CheckServiceAvailable() == false)
            {
                Debug.Log("[OrcasIap] Iap Service Not Available. Please Check Google Play/App Store Service Installed");
                return;
            }
            _bridge.Consume(storeOrderId);
        }


        /// <summary>
        /// 检查是否支持内购服务
        /// </summary>
        /// <returns></returns>
        public static bool CheckServiceAvailable()
        {
            bool available = _bridge.CheckServiceAvailable();
            return available;
        }


        /// <summary>
        /// 设置初始化的skuId列表
        /// </summary>
        /// <param name="skuIdList">所有SkuId拼接成的字符串，用'|'符号隔开</param>
        public static void SetSkuIdList(string skuIdList)
        {
            if (CheckServiceAvailable() == false)
            {
                Debug.Log("[OrcasIap] Iap Service Not Available. Please Check Google Play/App Store Service Installed");
                return;
            }
            _bridge.SetSkuIdList(skuIdList);
        }

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        internal static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        private static string Base64Encode(Encoding encodeType, string source)
        {
            string encode;
            var bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }


        public static string GetPriceString(string productId)
        {
            return PriceMap.ContainsKey(productId) ? PriceMap[productId] : string.Empty;
        }

        public static string GetOriPrice(string str, float dis)
        {
            if (dis < 0.00000001f)
            {
                dis = 0.001f;
            }
            if (string.IsNullOrEmpty(str)) return "";

            var index = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (!Regex.IsMatch(str[i].ToString(), @"^\d+$")) continue;
                index = i;
                break;
            }

            var head = str.Substring(0, index);
            var mid = str.Substring(index);
            index = 0;
            for (var i = 0; i < mid.Length; i++)
            {
                if (Regex.IsMatch(mid[i].ToString(), @"^(\d+|\.)$"))
                {
                    index = i;
                }
                else
                {
                    break;
                }

            }
            var numStr = "";
            var foot = "";
            index++;
            if (index >= mid.Length)
            {
                numStr = mid;
            }
            else
            {
                numStr = mid.Substring(0, index);
                foot = mid.Substring(index);
            }

            if (float.TryParse(numStr, out var price))
            {
                price /= dis;
            }
            return head + price.ToString("F2") + foot;
        }
    }
}

