namespace Orcas.Iap
{
    public class PurchaseResult
    {
        /// <summary>
        /// 服务器产生的订单Id
        /// </summary>
        public string ServerOrderId;

        /// <summary>
        /// 购买订单的Token
        /// </summary>
        public string Receipt;

        /// /// <summary>
        /// 购买订单凭证，由购买信息字段拼接并进行base64加密
        /// </summary>
        public string Token;

        /// <summary>
        /// 商品的SkuId
        /// </summary>
        public string SkuId;

        /// <summary>
        /// 商店生成的订单Id
        /// </summary>
        public string StoreOrderId;
        /// <summary>
        /// 1:正常购买
        /// 2:恢复购买
        /// </summary>
        public int PurchaseType;
    }
}
