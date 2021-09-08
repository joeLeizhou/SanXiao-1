namespace Orcas.Iap
{
    public class GooglePurchaseData 
    {
        public readonly string DeveloperPayload;
        public readonly string OrderId;
        public readonly string Sku;
        public string Signature;
        public string OrignalJson;
        public readonly string Token;
        public string State;
        public GooglePurchaseData(string data){
            var s = data.Split ('&');
            if (s.Length < 7) {
                return;
            }
            DeveloperPayload 	= s [0];
            OrderId 			= s [1];
            Sku	 				= s [2];
            Signature 			= s [3];
            OrignalJson 		= s [4];
            Token 				= s [5];
            State				= s [6];
        }
    }
}
