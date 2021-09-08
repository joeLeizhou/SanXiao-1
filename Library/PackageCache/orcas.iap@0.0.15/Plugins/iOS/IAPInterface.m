#import "IAPInterface.h"
#import "IAPManager.h"

@implementation IAPInterface


void InitIAPManager(){
    [[IAPManager SingleTon] attachObserver];    
}


void RestoreTransactions() {
    [[IAPManager SingleTon] restorePurchased];
}

bool IsProductAvailable(){
    return [[IAPManager SingleTon]CanMakePayment];
}

void RequstProductInfo(const char *productIds){
  //  NSLog(@"productKey:%@",productIds);
    [[IAPManager SingleTon]requestProductData:[[NSString alloc]initWithCString:(const char *) productIds encoding:NSASCIIStringEncoding]];
}

void BuyProduct(const char *productId,const char *orderid){
    NSString *productid=[[NSString alloc]initWithCString:(const char *) productId encoding:NSASCIIStringEncoding];
    NSString *data=[[NSString alloc]initWithCString:(const char *) orderid encoding:NSASCIIStringEncoding];
    [ [IAPManager SingleTon]buyRequest:productid OrderID:data];
}

void FinishTransaction(const char *data){
    [[IAPManager SingleTon]FinishTransaction:[[NSString alloc]initWithCString:(const char *) data encoding:NSASCIIStringEncoding]];
}
@end
