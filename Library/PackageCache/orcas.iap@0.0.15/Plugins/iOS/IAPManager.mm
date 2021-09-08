#import "IAPManager.h"

@implementation IAPManager
IAPManager *iapManager;
const char * asObjectName="OrcasIAP";
NSArray * trans;
bool isRestoring = false;

+(IAPManager *)SingleTon{
    if(iapManager==nil)
    {
        iapManager = [[IAPManager alloc] init];
    }
    return iapManager;
}

-(void) restorePurchased {
    isRestoring = true;
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

-(void) attachObserver{
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    UnitySendMessage(asObjectName, "OnInitializedFailed","");
}

-(BOOL) CanMakePayment{
    return [SKPaymentQueue canMakePayments];
}

-(void) requestProductData:(NSString *)productIdentifiers{
    NSArray *idArray = [productIdentifiers componentsSeparatedByString:@"|"];
    NSSet *idSet = [NSSet setWithArray:idArray];
    [self sendRequest:idSet];
}

-(void)sendRequest:(NSSet *)idSet{
    SKProductsRequest *productsrequest = [[SKProductsRequest alloc] initWithProductIdentifiers:idSet];
    productsrequest.delegate = self;
    [productsrequest start];
}

-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
    NSArray *products = response.products;
    
    if([products count]==0)
    {
        UnitySendMessage(asObjectName, "OnQueryInventoryFailed","");
        return;
    }
    
    for (SKProduct *p in products) {
        UnitySendMessage(asObjectName, "OnQueryInventoryFinished", [[self productInfo:p] UTF8String]);
    }
}

-(void)buyRequest:(NSString *)productIdentifier OrderID:(NSString *)orderid{
    
    //   [SKMutablePayment paymentWithProduct:];
    SKMutablePayment *payment=[SKMutablePayment paymentWithProductIdentifier:productIdentifier];
    payment.applicationUsername=orderid;
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

-(NSString *)productInfo:(SKProduct *)product{
    // NSLog(@"product.price:%@",product.price);
    //  NSString *price;
    NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
    [formatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [formatter setLocale:product.priceLocale];
    NSString * currencyString = [formatter stringFromNumber:product.price];
    // NSLog(@"hellocurrency:%@",currencyString);
    //  if([[[UIDevice currentDevice] systemVersion] floatValue] >= 10){
    //     price = [NSString stringWithFormat:@"%@%@",product.price,product.priceLocale.currencySymbol];
    //}else{
    //   price = [NSString stringWithFormat:@"%@",product.price];
    //}
    NSString *data = [NSString stringWithFormat:@"%@,%@",product.productIdentifier,currencyString];
    NSArray *info = [NSArray arrayWithObjects:data,nil];
    return [info componentsJoinedByString:@"\t"];
}

-(NSString *)encode:(const uint8_t *)input length:(NSInteger) length{
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    
    NSMutableData *data = [NSMutableData dataWithLength:((length+2)/3)*4];
    uint8_t *output = (uint8_t *)data.mutableBytes;
    
    for(NSInteger i=0; i<length; i+=3){
        NSInteger value = 0;
        for (NSInteger j= i; j<(i+3); j++) {
            value<<=8;
            
            if(j<length){
                value |=(0xff & input[j]);
            }
        }
        
        NSInteger index = (i/3)*4;
        output[index + 0] = table[(value>>18) & 0x3f];
        output[index + 1] = table[(value>>12) & 0x3f];
        output[index + 2] = (i+1)<length ? table[(value>>6) & 0x3f] : '=';
        output[index + 3] = (i+2)<length ? table[(value>>0) & 0x3f] : '=';
    }
    
    return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
}

-(void) provideContent:(const char *)funcName transaction:(SKPaymentTransaction *)transaction{
    
    //    if(transaction == nil){
    //        NSLog(@"transaction是空的");
    //        return;
    //    }
    //    if(transaction.transactionReceipt == nil){
    //        NSLog(@"transaction是空的");
    //    }
    //    if(transaction.payment == nil){
    //        NSLog(@"payment是空的");
    //        return;
    //    }
    //    if(transaction.payment.applicationUsername == nil){
    //        NSLog(@"applicationUsername是空的");
    //        [[SKPaymentQueue defaultQueue] finishTransaction:transaction]
    //        return;
    //    }
    //    if(transaction.payment.productIdentifier == nil){
    //        NSLog(@"productIdentifier是空的");
    //        return;
    //    }
    
    NSString * transactions=[self encode:(uint8_t *) transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length ];
    NSString * username=transaction.payment.applicationUsername;
    if (username == nil){
        username = @"1";
    }
    NSString *newString = [NSString stringWithFormat:@"%@,%@:%@",transaction.payment.productIdentifier,transactions,username];
    NSLog(newString);
    UnitySendMessage(asObjectName, funcName, [newString cStringUsingEncoding:NSASCIIStringEncoding]);
}

-(void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions{
    trans = transactions;
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self purchasedTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            default:
                break;
        }
    }
}

-(bool)paymentQueue:(SKPaymentQueue *)queue shouldAddStorePayment:(SKPayment *)payment forProduct:(SKProduct *)product{
    return true;
}

-(void)FinishTransaction:(NSString *)data{
    for (SKPaymentTransaction *transactions in trans) {
        NSString *username = transactions.payment.applicationUsername;
        if(username == nil){
            if([data isEqualToString:@"1"]){
                [[SKPaymentQueue defaultQueue] finishTransaction:transactions];
            }
        }else{
            if([username isEqualToString: data]){
                [[SKPaymentQueue defaultQueue] finishTransaction:transactions];
            }
        }
    }
}

-(void)purchasedTransaction:(SKPaymentTransaction *)transaction {
    if (isRestoring == false)
    {
        NSLog(@"OnPurchaseSuccess %@", transaction.payment.productIdentifier);
        [self provideContent:"OnPurchaseSuccess" transaction:transaction];
    }
}

-(void) failedTransaction:(SKPaymentTransaction *)transaction{
    if(transaction.error.code == SKErrorPaymentCancelled){
        UnitySendMessage(asObjectName, "OnPurchaseCancelled",[[NSString stringWithFormat:@"%ld", (long)transaction.error.code] cStringUsingEncoding:NSUTF8StringEncoding]);
        NSLog(@"iOS Purchase Failed: Cancelled");
    }
    else if(transaction.error.code == SKErrorCloudServiceNetworkConnectionFailed){
        UnitySendMessage(asObjectName, "OnPurchaseConnectFailed",[[NSString stringWithFormat:@"%ld", (long)transaction.error.code] cStringUsingEncoding:NSUTF8StringEncoding]);
        NSLog(@"iOS Purchase Failed: Cloud Service Network Connection Failed");
    }
    else{
        UnitySendMessage(asObjectName, "OnPurchaseUnknownFailed",[[NSString stringWithFormat:@"%ld", (long)transaction.error.code] cStringUsingEncoding:NSUTF8StringEncoding]);
        NSLog(@"iOS Purchase Failed: Other Failed");
    }
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

-(void) restoreTransaction:(SKPaymentTransaction *)transaction{
    NSLog(@"restoreTransaction %@", transaction.payment.productIdentifier);
    [self provideContent:"OnRestoreSuccess" transaction:transaction];
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    NSString *idString = @"";
//    NSLog(@"received restored transactions: %i", queue.transactions.count);
    for (SKPaymentTransaction *transaction in queue.transactions)
    {
        NSString *productID = transaction.payment.productIdentifier;
        idString = [idString stringByAppendingString:productID];
        idString = [idString stringByAppendingString:@","];
        NSLog(@"RestoreCompletedTransactionsFinished %@", productID);
        [self provideContent:"OnRestoreSuccess" transaction:transaction];
    }
    NSLog(@"rectore completed idString = %@",idString);
    isRestoring = false;
}

- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
    NSString *idString = [error localizedFailureReason];
    UnitySendMessage(asObjectName, "OnRestoreFailed", [idString cStringUsingEncoding:NSASCIIStringEncoding]);
    isRestoring = false;
}
@end
