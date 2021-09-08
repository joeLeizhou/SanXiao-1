namespace Orcas.Iap.Interface
{
    public interface IPurchaseNativeEventListener
    {
        void OnInitialized(string info);

        void OnInitializedFailed(string info);

        void OnQueryInventoryFinished(string inventory);

        void OnQueryInventoryFailed(string info);

        void OnPurchaseSuccess(string str);

        void OnPurchaseCancelled(string info);

        void OnPurchaseConnectFailed(string info);

        void OnPurchaseUnknownFailed(string info);
    }
}
