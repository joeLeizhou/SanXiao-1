namespace Orcas.Iap.Interface
{
    public interface IPurchaseEventListener
    {
        void OnInitialized(string info);
        
        void OnInitializedFailed(string info);

        void OnQueryInventoryFinished(string inventory);

        void OnQueryInventoryFailed(string info);

        void OnPurchaseSuccess(PurchaseResult info);

        void OnPurchaseFailed(PurchaseFailedType reason, string code);

        void OnConsumeSuccess(string orderId);

        void OnConsumeFailed(string orderId);
    }
}