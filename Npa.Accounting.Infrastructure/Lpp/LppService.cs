using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Infrastructure.Abstractions;
using Npa.Accounting.Infrastructure.Helpers;

namespace Npa.Accounting.Infrastructure.Lpp;

internal class LppService : ILppService
{
    private readonly ICryptoRepository crypto;
    private readonly HttpClient client;
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
    private readonly LppOptions options;

    public LppService(ICryptoRepository crypto, HttpClient client, IOptions<LppOptions> options)
    {
        this.crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.options = options.Value;

        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        serializerSettings.Converters.Add(new StringEnumConverter());
    }

    private string? GetDebitCredentials(Merchant m) =>
        options.Credentials.FirstOrDefault(c => c.Merchant == m)?.Debit;

    private string? GetDisbursementCredentials(Merchant m) =>
        options.Credentials.FirstOrDefault(c => c.Merchant == m)?.Disbursement;

    private async Task<LppCard> PrepareAsync(Transaction ct, CancellationToken t = default)
    {
        var card = await crypto.GetAsync(ct.CardTransaction.Card.Id, ct.Teller, t);
        if (card == null)
        {
            throw new InfrastructureLayerException("Unable to process the card");
        }

        return LppCard.From(card, ct.Amount < 0 ? -ct.Amount : ct.Amount);
    }

    private CardReturnStatus CodeFromResult(Status s) => s switch
    {
        Status.Success => CardReturnStatus.Approve,
        Status.Failure => CardReturnStatus.Deny,
        Status.Error => CardReturnStatus.Error,
        Status.BadRequest => CardReturnStatus.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
    };

    private ReturnMessage ToResult(PaymentResult res) => new ReturnMessage(CodeFromResult(res.Status), CodeFromResult(res.Status).ToString(), res.Message, res.TransactionId);

    private HttpRequestMessage PrepareMessage(Uri uri, string key, string content)
    {
        var req = new HttpRequestMessage();
        req.RequestUri = uri;
        req.Headers.Add("TransactionKey", key);
        req.Method = HttpMethod.Post;
        req.Content = new StringContent(content, Encoding.UTF8,
            "application/json");
        return req;
    }

    public async Task<ReturnMessage> DebitAsync(Transaction ct, CancellationToken t = default)
    {
        var card = await PrepareAsync(ct, t);
        var r = await client.SendAsync(
            PrepareMessage(new Uri($"{client.BaseAddress}v2/paymentcards/run"), GetDebitCredentials(ct.CardTransaction.MerchantId),
                JsonConvert.SerializeObject(card, serializerSettings)
            )
        );

        await ResponseHelper.ThrowIfInvalidResponse(r);
        return ToResult(
            JsonConvert.DeserializeObject<PaymentResult>(await r.Content.ReadAsStringAsync(), serializerSettings)!);
    }

    public async Task<ReturnMessage> DisburseAsync(Transaction ct, CancellationToken t = default)
    {
        var card = await PrepareAsync(ct, t);
        var r = await client.SendAsync(
            PrepareMessage(new Uri($"{client.BaseAddress}v2-1/paymentcards/disburse"),
                GetDisbursementCredentials(ct.CardTransaction.MerchantId),
                JsonConvert.SerializeObject(card, serializerSettings)
            ), t);

        await ResponseHelper.ThrowIfInvalidResponse(r);

        return ToResult(
            JsonConvert.DeserializeObject<PaymentResult>(await r.Content.ReadAsStringAsync(), serializerSettings))!;
    }
}