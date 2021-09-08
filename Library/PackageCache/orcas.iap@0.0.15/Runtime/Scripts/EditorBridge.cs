using System.Collections;
using System.Collections.Generic;
using Orcas.Iap.Interface;
using UnityEngine;

namespace Orcas.Iap
{
    public class EditorBridge : IPurchaseBridge
    {
        public void Init()
        {

        }

        public void ReconnectStore()
        {

        }

        public void QueryInventory()
        {

        }

        public void RestorePurchase()
        {

        }

        public void Purchase(string skuId, string extraData)
        {
            Debug.Log("Editor Iap Purchase skuId: " + skuId + " extraData: " + extraData);
        }

        public void Consume(string orderId)
        {
            Debug.Log("Editor Iap Consume " + orderId);
        }

        public bool CheckServiceAvailable()
        {
            return false;
        }

        public void SetSkuIdList(string skuIdList)
        {
            Debug.Log("Editor Iap SetSkuIdList: " + skuIdList);
        }
    }
}

