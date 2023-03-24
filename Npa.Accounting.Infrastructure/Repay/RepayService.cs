using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Infrastructure.Abstractions;
using Npa.Accounting.Infrastructure.Newtonsoft.ContractResolvers;
using Npa.Accounting.Infrastructure.Npacc;
using Npa.Accounting.Infrastructure.Repay.Paytokens;
using Npa.Accounting.Infrastructure.Repay.Responses;

namespace Npa.Accounting.Infrastructure.Repay;

internal class RepayService : IRepayService
{
    private readonly HttpClient client;
    private readonly LegacyRepayService legacyRepayService;
    private readonly CheckoutForm checkoutForm;
    private readonly string pmtCheckoutFormId;
    private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
    {
        ContractResolver = new SnakeCaseContractResolver()
    };

    public RepayService(HttpClient client, LegacyRepayService legacyRepayService, IOptions<RepayOptions> options)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.legacyRepayService = legacyRepayService ?? throw new ArgumentNullException(nameof(legacyRepayService));
        this.checkoutForm = options.Value.CheckoutForm;

        Guard.ForNullOrEmpty(options.Value.Uri, nameof(options.Value.Uri));
        Guard.ForNullOrEmpty(options.Value.Auth, nameof(options.Value.Auth));
        client.BaseAddress = new Uri(options.Value.Uri);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apptoken", options.Value.Auth);
    }

    private async Task ThrowIfInvalidResponse(HttpResponseMessage res)
    {
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException rhe)
        {
            RepayErrors? o = null;
            try
            {
                o = JsonConvert.DeserializeObject<RepayErrors>(await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(await res.Content.ReadAsStringAsync(), rhe, rhe.StatusCode);
            }

            throw new HttpRequestException(o?.ToString() ?? "An unknown error occured", rhe, rhe.StatusCode);
        }
    }

    private string GetFormId(BasePaytoken b) => b.CheckoutFormKey switch
    {
        CheckoutFormKey.TokenPayment => checkoutForm.TokenPayment,
        CheckoutFormKey.CardAuth => checkoutForm.CardAuth,
        _ => throw new ArgumentOutOfRangeException()
    };

    private async Task SetPayTokenAsync<T>(T request, CancellationToken t = default) where T : BasePaytoken
    {
        var response = await client.PostAsync($"checkout-forms/{GetFormId(request)}/paytoken",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"), t);
        await ThrowIfInvalidResponse(response);
        var result = JsonConvert.DeserializeObject<PaytokenResponse>(await response.Content.ReadAsStringAsync(), serializerSettings);
        request.SetPayToken(result);
    }

    private async Task<PaymentResponse> RunTransactionAsync<T>(T request, CancellationToken t = default)
        where T : BasePaytoken
    {
        var response = await client.PostAsync($"checkout-forms/{checkoutForm.CardAuth}/token-payment",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"), t);
        
        await ThrowIfInvalidResponse(response);
        return JsonConvert.DeserializeObject<PaymentResponse>(await response.Content.ReadAsStringAsync(), serializerSettings) ??
               throw new InfrastructureLayerException(
                   $"Improper Card Response: {await response.Content.ReadAsStringAsync()}");
    }


    public Task<NpaCardResponse> DebitAsync(NpaTokenPayment ct, CancellationToken t = default) =>
        legacyRepayService.DebitAsync(ct, t);

    public async Task<PaymentResponse> AddCardAsync(int customerId, Card card, CancellationToken t = default)
    {
        var request = new PaytokenCardRequest(customerId, 0.00m, card, true, TransactionType.Auth);
        await SetPayTokenAsync(request, t);

        return await RunTransactionAsync(request);
    }

    public async Task<PaymentResponse> TokenPaymentAsync(CustomerCard card, decimal amount, CancellationToken t = default)
    {
        var request = new PaytokenTokenRequest(card.PowerId, amount, card.CardToken, TransactionType.Sale);
        await SetPayTokenAsync(request, t);

        return await RunTransactionAsync(request);
    }
}