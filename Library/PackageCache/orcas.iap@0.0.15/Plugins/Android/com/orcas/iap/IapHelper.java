package com.orcas.iap;

import android.app.Activity;
import android.content.Context;
import android.util.Log;

import com.android.billingclient.api.BillingClientStateListener;
import com.android.billingclient.api.BillingFlowParams;
import com.android.billingclient.api.BillingResult;
import com.android.billingclient.api.BillingClient;
import com.android.billingclient.api.BillingClient.BillingResponseCode;
import com.android.billingclient.api.BillingClient.SkuType;
import com.android.billingclient.api.ConsumeParams;
import com.android.billingclient.api.ConsumeResponseListener;
import com.android.billingclient.api.Purchase;
import com.android.billingclient.api.PurchasesUpdatedListener;
import com.android.billingclient.api.SkuDetails;
import com.android.billingclient.api.SkuDetailsParams;
import com.android.billingclient.api.SkuDetailsResponseListener;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.unity3d.player.UnityPlayer;

public class IapHelper {

    private PurchasesUpdatedListener purchasesUpdatedListener = new PurchasesUpdatedListener() {
        @Override
        public void onPurchasesUpdated(BillingResult billingResult, List<Purchase> purchases) {
            int code = billingResult.getResponseCode();
            if (code == BillingResponseCode.OK
                    && purchases != null) {
                for (Purchase purchase : purchases) {
                    int state = purchase.getPurchaseState();
                    if(state == Purchase.PurchaseState.PURCHASED){
                        mPurchases.put(purchase.getOrderId(), purchase);
                        UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseSuccess", purchase2String(0, purchase));
//                         Log("Purchase Result Success " + purchase2String(0, purchase));
                    }
                    else{
                    // 可能是PENDING或者UNSPECIFIED_STATE状态，此时该订单还未能付款成功，不能进行处理。暂时不认为是失败。
//                         UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseUnknownFailed", "Error PurchaseState - " + state);
//                         Log("Purchase Result Fail " + "Error PurchaseState - " + state);
                    }
                }
            }
            else if (code == BillingResponseCode.USER_CANCELED) {
                UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseCancelled", ""+ code);
//                 Log("Purchase Result Fail " + "User Cancelled - "+ code);
            } else if (code == BillingResponseCode.SERVICE_UNAVAILABLE || code == BillingResponseCode.SERVICE_TIMEOUT || code == BillingResponseCode.SERVICE_DISCONNECTED){
                UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseConnectFailed", ""+ code);
//                 Log("Purchase Result Fail " + "Service Down - "+ code);
            }
            else {
                UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseUnknownFailed", ""+ code);
//                 Log("Purchase Result Fail " + "Error BillingResponseCode - "+ code);
            }
        }
    };


    private BillingClient billingClient;

    private HashMap<String, SkuDetails> mSkuDetails = new HashMap<>();
    private HashMap<String, Purchase> mPurchases = new HashMap<>();
    private ArrayList<String> mSkuIds = new ArrayList<>();
    private boolean NeedLog = true;
    private Activity currentActivity;
    private Context currentContext;

    private void Log(String s){
        if(NeedLog){
            Log.i("[OrcasIap] ", s);
        }
    }

    public IapHelper(Context ctx, Activity activity){
        currentActivity = activity;
        currentContext = ctx;
    }

    public static IapHelper GetIapHelperImpl(Context ctx, Activity activity){
        return new IapHelper(ctx, activity);
    }

    public void Initialize() {
        Log("Start Initialized");
        billingClient = BillingClient.newBuilder(currentActivity)
                .setListener(purchasesUpdatedListener)
                .enablePendingPurchases()
                .build();

        billingClient.startConnection(new BillingClientStateListener() {
            @Override
            public void onBillingSetupFinished(BillingResult billingResult) {
                int code = billingResult.getResponseCode();
                if (code == BillingResponseCode.OK) {
                    UnityPlayer.UnitySendMessage("OrcasIAP", "OnInitialized", "");
//                     Log("Setup Success");
                } else {
                    UnityPlayer.UnitySendMessage("OrcasIAP", "OnInitializedFailed", "" + code);
//                     Log("Setup Failed " + code);
                }
            }

            @Override
            public void onBillingServiceDisconnected() {
                UnityPlayer.UnitySendMessage("OrcasIAP", "OnInitializedFailed", "-100");
//                 Log("Service Disconnect ");
            }
        });
    }

    public void SetSkuIdList(final String strOrderIdList){
        mSkuIds.clear();
        String[] s = strOrderIdList.split("\\|");
        for(int i = 0; i < s.length; i++){
            mSkuIds.add(s[i]);
        }
//         Log("SetSkuIdList " + strOrderIdList);
    }

    public void QueryInventory() {
        if (billingClient != null && billingClient.isReady()) {
//             Log("Start QueryInventory ");
            SkuDetailsParams.Builder params = SkuDetailsParams.newBuilder();
            params.setSkusList(mSkuIds).setType(SkuType.INAPP);
            billingClient.querySkuDetailsAsync(params.build(),
                    new SkuDetailsResponseListener() {
                        @Override
                        public void onSkuDetailsResponse(BillingResult billingResult,
                                                         List<SkuDetails> skuDetailsList) {
                            if(billingResult.getResponseCode() == BillingResponseCode.OK){
                                mSkuDetails.clear();
                                StringBuilder sb = new StringBuilder();
                                for(int i = 0; i < skuDetailsList.size(); ++i){
                                    if(i != 0) {
                                        sb.append('|');
                                    }
                                    SkuDetails item = skuDetailsList.get(i);
                                    mSkuDetails.put(item.getSku(), item);
                                    sb.append(item.getSku());
                                    sb.append('!');
                                    sb.append(item.getPrice());
                                }
                                UnityPlayer.UnitySendMessage("OrcasIAP", "OnQueryInventoryFinished", sb.toString());
//                                 Log("QueryInventory Success " + sb.toString());
                            }
                            else{
                                UnityPlayer.UnitySendMessage("OrcasIAP", "OnQueryInventoryFailed", "" + ((int) billingResult.getResponseCode()));
//                                 Log("QueryInventory Failed " + billingResult.getResponseCode());
                            }
                        }
                    });
        }
    }

    public void QueryPurchase(){
        if (billingClient != null && billingClient.isReady()){
//             Log("Start QueryPurchase ");
            Purchase.PurchasesResult result = billingClient.queryPurchases(SkuType.INAPP);
            purchasesUpdatedListener.onPurchasesUpdated(result.getBillingResult(), result.getPurchasesList());
        }
    }


    public void StartPurchase(final String skuId, final String payload){
        if (billingClient != null && billingClient.isReady()){
//             Log("StartPurchase skuId - " + skuId + " payload - " + payload);
            if(mSkuDetails.containsKey(skuId)){
                SkuDetails item = mSkuDetails.get(skuId);
                BillingFlowParams billingFlowParams = BillingFlowParams.newBuilder()
                        .setSkuDetails(item)
                        .setObfuscatedAccountId(payload)
                        .build();
                int responseCode = billingClient.launchBillingFlow(currentActivity, billingFlowParams).getResponseCode();
                if(responseCode == BillingClient.BillingResponseCode.OK){
//                     Log("Purchase launchBillingFlow Success");
                }
                else{
                    UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseUnknownFailed", "" + ((int)responseCode));
//                     Log("Purchase launchBillingFlow Failed " + responseCode);
                }
            }
            else{
                UnityPlayer.UnitySendMessage("OrcasIAP", "OnPurchaseUnknownFailed", "-100");
//                 Log("Purchase Failed Sku Not Exists ");
            }
        }
    }



    public void Consume(final String orderId){
        if (billingClient != null && billingClient.isReady()){
            if(mPurchases.containsKey(orderId)){
//                 Log("Start Consume - " + orderId);
                final Purchase purchase = mPurchases.get(orderId);
                ConsumeParams consumeParams =
                        ConsumeParams.newBuilder()
                                .setPurchaseToken(purchase.getPurchaseToken())
                                .build();

                ConsumeResponseListener listener = new ConsumeResponseListener() {
                    @Override
                    public void onConsumeResponse(BillingResult billingResult, String purchaseToken) {
                        int code = billingResult.getResponseCode();
                        if (code == BillingResponseCode.OK) {
                            UnityPlayer.UnitySendMessage("OrcasIAP", "ConsumeSuccess", purchase.getSku());
//                             Log("Consume Success - " + orderId);
                        }
                        else{
                            UnityPlayer.UnitySendMessage("OrcasIAP", "ConsumeFailed", purchase.getSku()+":code-"+code);
//                             Log("Consume Failed - " + orderId+":code-"+code);
                        }
                    }
                };

                billingClient.consumeAsync(consumeParams, listener);
            }
        }
    }


    public String purchase2String(int state, Purchase purchase) {
        StringBuilder sb = new StringBuilder();
        if(state == 0) {
            sb.append(purchase.getAccountIdentifiers().getObfuscatedAccountId());
            sb.append('&');
            sb.append(purchase.getOrderId());
            sb.append('&');
            sb.append(purchase.getSku());
            sb.append('&');
            sb.append(purchase.getSignature());
            sb.append('&');
            sb.append(purchase.getOriginalJson());
            sb.append('&');
            sb.append(purchase.getPurchaseToken());
            sb.append("&0");
        } else {
            for(int i = 0; i < 6; ++i) {
                sb.append('&');
            }
            sb.append(String.valueOf(state));
        }

        return sb.toString();
    }

    public boolean CheckGooglePlayServiceAvailable(){
        GoogleApiAvailability googleApiAvailability = GoogleApiAvailability.getInstance();
        int status = googleApiAvailability.isGooglePlayServicesAvailable(currentActivity);
        if(status != ConnectionResult.SUCCESS) {
            if(googleApiAvailability.isUserResolvableError(status)) {
                googleApiAvailability.getErrorDialog(currentActivity, status, 2404).show();
            }
            return false;
        } else {
            return true;
        }
    }
}
