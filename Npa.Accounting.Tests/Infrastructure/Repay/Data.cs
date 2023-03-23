using System;
using System.Linq;
using Npa.Accounting.Infrastructure.Repay.Responses;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class Data
{
    public static PaymentResponse RepayResponse200 => new PaymentResponse()
    {
        EntryMethod = "Keyed",
        RequestedAuthAmount = 0m,
        AuthAmount = 0m,
        TotalAmount = 0m,
        TipAmount = 0m,
        ConvenienceAmount = null,
        CustomFields = new CustomField[]
        {
            new CustomField()
            {
                CustomFieldId = 536878227,
                CustomFieldName = "PaymentChannel",
                Description = "PaymentChannel",
                MerchantId = 536875818,
                FieldValue = "web",
                TransactionId = 565135833,
                DisplayOnReceipt = false
            },

        }.ToList(),
        MerchantId = 536875818,
        MerchantName = "Net Pay Advance Channels test 1",
        CustomerId = "123456",
        InvoiceId = null,
        Date = DateTime.Parse("2022-09-27T16:41:49.8318408Z").ToUniversalTime(),
        PaymentTypeId = "MASTERCARD",
        CardBin = "510258",
        Last4 = "9913",
        NameOnCard = "Test Name",
        TransTypeId = "Authorization",
        UserName = "ebpp_api_ntpyadv1",
        LastBatchNumber = "384",
        BatchNumber = null,
        Result = "0",
        ResultText = "Approved",
        ResultDetails = new ResultDetails()
        {
            AuthorizationReversed = false,
            DelayedInReporting = false,
            CardInfo = new CardInfo()
            {
                Country = null,
                BankName = "FISERV SOLUTIONS INC.",
                Brand = "MASTERCARD",
                Category = null,
                Type = "DEBIT"
            }
        },
        Voided = false,
        Reversed = false,
        HostRefNum = "09225E7VWKAPE",
        AvsResponse = "",
        AvsResponseCode = "0",
        CvResponse = "",
        CvResponseCode = "",
        SettleFlag = "0",
        RetryCount = 0,
        CurrencyId = "840",
        Street = "",
        Zip = "",
        ProcessorId = "LOOPBACK",
        ExpDate = "1222",
        PnRef = "565135833",
        AuthCode = "Y59J7I",
        SavedPaymentMethod = new SavedPaymentMethod()
        {
            Id = "9d6e15af-ca55-4402-a303-3297d974eccd",
            Token = "560132784",
            IsEligibleForDisbursement = false
        },
        PaymentMethodDetail = new PaymentMethodDetail()
        {
            CardLastFour = "9913"
        },
        ReceiptId =
            "565135833.FhS5Tg.12A72aEm3_FmU7fuBbkx9ojFEhYvBHIzkD-3o9-3d3eQmTJ4SELp9OKmX0mHXPuPYUBJw8ondSpHAByTXWA-Kg",
    };
}