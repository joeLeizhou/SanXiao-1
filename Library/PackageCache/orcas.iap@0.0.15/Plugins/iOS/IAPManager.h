#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
//#import <HideAd/HideAdInterface.h>

@interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>{
    SKProduct *proUpgradeProduct;
    SKProductsRequest *productsRequest;
}
+(IAPManager *)SingleTon;
-(void)restorePurchased;
-(void)attachObserver;
-(BOOL)CanMakePayment;
-(void)requestProductData:(NSString *)productIdentifiers;
-(void)buyRequest:(NSString *)productIdentifier OrderID:(NSString *)orderid;
-(void)FinishTransaction:(NSString *)data;

@end


