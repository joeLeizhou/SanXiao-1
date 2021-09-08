# OrcasIAP

本项目提供Android和iOS平台内购的原生层实现和csharp层接口封装。

## 一、配置和依赖

### 1、权限

Android: 需要在主工程的AndroidManifest,xml添加内购权限

```xml
<uses-permission android:name="com.android.vending.BILLING" />
```

### 2、依赖

Android:

安卓需要依赖的package有billing-v3和google play services的base库（版本视工程而定）。

```gradle
implementation (name: 'billing-3.0.1',ext:'aar')  
implementation 'com.google.android.gms:play-services-base:17.0.0'
```

iOS：

需要确保Xcode添加InAppPurchasing的工程依赖。

## 二、Manual

1、初始化

需要在合适的位置，比如游戏启动时调用`NativePurchasing.Init` 。根据平台传入不同的Bridge类型，并自定义好事件监听类。 注意，如果连接商店失败，客户端自行根据业务逻辑需要制定重新连接商店的策略，调用`NativePurchasing.ReconnectStore()`。

根据需要添加初始化、获取商品信息，请求内购结果等的事件回调。

例子：

```csharp
#if !UNITY_EDITOR && UNITY_IOS
			NativePurchasing.Init<AppStorePurchaseBridge, NativeIAPListener>();
#elif !UNITY_EDITOR && UNITY_ANDROID
			NativePurchasing.Init<GooglePlayPurchaseBridge, NativeIAPListener>();
#endif

public class NativeIAPListener : IPurchaseEventListener
{
    public void OnInitialized(string info)
    {

    }

    public void OnInitializedFailed(string info)
    {

    }

    public void OnQueryInventoryFinished(string inventory)
    {

    }

    public void OnQueryInventoryFailed(string info)
    {

    }

    public void OnPurchaseSuccess(PurchaseResult info)
    {

    }

    public void OnPurchaseFailed(PurchaseFailedType reason, string code)
    {

    }

    public void OnConsumeSuccess(string orderId)
    {

    }
}
```

2、设置商品id数组

在合适的位置，调用`NativePurchasing.SetSkuIdList` 接口，向原生层传递所有商品的SkuId。参数为用'|'分隔的拼接字符串。

例子：

```csharp
string strSkuIdList =
        "diamond_100|diamond_1300|diamond_3000|diamond_8500|diamond_20000|tour_offer_7|tour_offer_6|tour_offer_5";
        
NativePurchasing.SetSkuIdList(strSkuIdList);
```



3、请求商品信息

在设置完成商品id之后，可以调用`NativePurchasing.QueryInventory` 获取商品信息，目前只存了价格信息。

4、请求购买商品

调用`NativePurchasing.Purchase` 方法可以发起一次内购，注意处理结果的各种回调。在合适的位置调用`NativePurchasing.Consume` 处理已经完成的订单。

5、购买失败错误码参考

Android:

[官方文档](https://developer.android.com/reference/com/android/billingclient/api/BillingClient.BillingResponseCode)

```java
public @interface BillingResponseCode {
        int SERVICE_TIMEOUT = -3;
        int FEATURE_NOT_SUPPORTED = -2;
        int SERVICE_DISCONNECTED = -1;
        int OK = 0;
        int USER_CANCELED = 1;
        int SERVICE_UNAVAILABLE = 2;
        int BILLING_UNAVAILABLE = 3;
        int ITEM_UNAVAILABLE = 4;
        int DEVELOPER_ERROR = 5;
        int ERROR = 6;
        int ITEM_ALREADY_OWNED = 7;
        int ITEM_NOT_OWNED = 8;    
}
// 另外，如果QueryInventory失败了，无法获取商品信息的情况下，则返回-100

```



iOS:

```objective-c
typedef NS_ENUM(NSInteger,SKErrorCode) {
    SKErrorUnknown = 0,
    SKErrorClientInvalid,                                                                                     // client is not allowed to issue the request, etc.
    SKErrorPaymentCancelled,                                                                                    // user cancelled the request, etc.
    SKErrorPaymentInvalid,                                                                                      // purchase identifier was invalid, etc.
    SKErrorPaymentNotAllowed,                                                                                   // this device is not allowed to make the payment
    SKErrorStoreProductNotAvailable 
// Product is not available in the current storefront
    SKErrorCloudServicePermissionDenied, 
// user has not allowed access to cloud service information
    SKErrorCloudServiceNetworkConnectionFailed,   
// the device could not connect to the nework
    SKErrorCloudServiceRevoked,                 
// user has revoked permission to use this cloud service
    SKErrorPrivacyAcknowledgementRequired,  
// The user needs to acknowledge Apple's privacy policy
    SKErrorUnauthorizedRequestData,                      
// The app is attempting to use SKPayment's requestData property, but does not have the appropriate entitlement
    SKErrorInvalidOfferIdentifier,                       
// The specified subscription offer identifier is not valid
    SKErrorInvalidSignature,                             
// The cryptographic signature provided is not valid
    SKErrorMissingOfferParams,                           
// One or more parameters from SKPaymentDiscount is missing
    SKErrorInvalidOfferPrice,                            
// The price of the selected offer is not valid (e.g. lower than the current base subscription price)
    SKErrorOverlayCancelled,
    SKErrorOverlayInvalidConfiguration,
    SKErrorOverlayTimeout,
    SKErrorIneligibleForOffer,                              
// User is not eligible for the subscription offer
    SKErrorUnsupportedPlatform,
} API_AVAILABLE(ios(3.0), macos(10.7), watchos(6.2));
```





