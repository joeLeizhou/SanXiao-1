using Orcas.Iap.Interface;
using UnityEngine;

namespace Orcas.Iap
{
    public class GooglePlayPurchaseBridge : IPurchaseBridge
    {
        private AndroidJavaObject _androidObj = null;
        private bool hasCheckAvailable = false;
        private bool isServiceAvailable = false;
        public void Init()
        {
            var obj = new GameObject("OrcasIAP");
            obj.AddComponent<GooglePlayListener>();
#if UNITY_ANDROID && !UNITY_EDITOR
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var currentContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaClass androidClass = new AndroidJavaClass("com.orcas.iap.IapHelper");
            _androidObj =
 androidClass.CallStatic<AndroidJavaObject>("GetIapHelperImpl", currentContext, currentActivity);
            if(CheckServiceAvailable()){
                _androidObj.Call("Initialize");
            }
#endif
        }

        private void QueryPurchase()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _androidObj?.Call("QueryPurchase");
#endif
        }

        public void ReconnectStore()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
             _androidObj?.Call("Initialize");
#endif
        }

        public void QueryInventory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
             _androidObj?.Call("QueryInventory");
#endif
        }

        public void RestorePurchase()
        {

        }

        public void Purchase(string skuId, string extraData)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                _androidObj?.Call("StartPurchase", skuId, extraData);
#endif
        }

        public void Consume(string storeOrderId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _androidObj?.Call("Consume", storeOrderId);
#endif
        }

        public bool CheckServiceAvailable()
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if(hasCheckAvailable == false){
                hasCheckAvailable = true;
                if (_androidObj != null)
                {
                    isServiceAvailable = _androidObj.Call<bool>("CheckGooglePlayServiceAvailable");
                }
            }
#endif
            return isServiceAvailable;
        }

        public void SetSkuIdList(string skuIdList)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                _androidObj?.Call ("SetSkuIdList", skuIdList);
#endif
            QueryPurchase();
        }
    }
}