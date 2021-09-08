namespace Orcas.Iap.Interface
{
    public interface IPurchaseBridge
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 重连到商店
        /// </summary>
        void ReconnectStore();
        /// <summary>
        /// 请求商品信息
        /// </summary>
        void QueryInventory();
        /// <summary>
        /// 恢复已购买非消耗商品
        /// </summary>
        void RestorePurchase();
        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="extraData"></param>
        void Purchase(string skuId, string extraData);
        /// <summary>
        /// 消耗订单
        /// </summary>
        /// <param name="storeOrderId"></param>
        void Consume(string storeOrderId);
        /// <summary>
        /// 检查服务是否可用
        /// </summary>
        bool CheckServiceAvailable();
        /// <summary>
        /// 初始化SkuId列表
        /// </summary>
        /// <param name="skuIdList"></param>
        void SetSkuIdList(string skuIdList);
    }
}