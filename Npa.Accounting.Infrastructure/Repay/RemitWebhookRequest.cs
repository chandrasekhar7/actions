using System;

namespace Npa.Accounting.Infrastructure.Repay
{
    public class RemitWebhookRequest
    {
        public EventMetaData Event_Meta_Data { get; init; }
        public EventData Event_Data { get; init; }

        public class EventMetaData
        {
            public string Event_Type { get; init; }
            public string Version { get; init; }
        }
        public class EventData
        {
            public RequestData Request { get; init; }
            public DateTime Timestamp { get; init; }
            public ResultData Result { get; init; }
        }

        public class RequestData
        {
            public string Gateway_Mid { get; init; }
            public string Payment_Channel { get; init; }
            public string Amount { get; init; }
            public string Amount_Cents { get; init; }
            public string Card_Brand { get; init; }
            public string Card_Type { get; init; }
            public string Card_Expiration { get; init; }
            public string Card_Last_Four { get; init; }
            public string Card_Name { get; init; }
            public string Address { get; init; }
            public string? Invoice_Number { get; init; }
            public string Customer_Id { get; init; }
            public CustomField[] Custom_Fields { get; init; }
            public string? Nickname { get; init; }
        }

        public class CustomField
        {
            public string PaymentChannel { get; init; }
            public string MultiPay { get; init; }
            public string ChannelUser { get; init; }
            public string Source { get; init; }
        }

        public class ResultData
        {
            public string? Original_Transaction_Id { get; init; }
            public string Result_Code { get; init; }
            public string Pn_Ref { get; init; }
            public string Auth_Code { get; init; }
            public string Host_Code { get; init; }
            public string Host_Url { get; init; }
            public string Message { get; init; }
            public string Message1 { get; init; }
            public string Message2 { get; init; }
            public string Resp_Msg { get; init; }
            public AvsResult Avs_Result { get; init; }
            public string Commercial_Card { get; init; }
            public string Cv_Result { get; init; }
            public string Card_Token { get; init; }
            public string Last_Batch_Number { get; init; }
        }

        public class AvsResult
        {
            public string Avs_Response { get; init; }
            public string Avs_Response_Msg { get; init; }
            public string Zip_Match { get; init; }
            public string Street_Match { get; init; }
        }
    }
}
