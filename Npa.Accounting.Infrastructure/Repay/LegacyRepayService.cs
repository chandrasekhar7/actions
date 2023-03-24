using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npa.Accounting.Infrastructure.Npacc;

namespace Npa.Accounting.Infrastructure.Repay;

internal class LegacyRepayService
{
    private readonly HttpClient client;

    public LegacyRepayService(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }
        
    private async Task ThrowIfInvalidResponse(HttpResponseMessage res)
    {
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException rhe)
        {
            // ParseMessage out
            var o = JsonConvert.DeserializeObject<NpaccError>(await res.Content.ReadAsStringAsync());
            throw new HttpRequestException(o.Message, rhe, rhe.StatusCode);
        }
    }

    public async Task<NpaCardResponse> DebitAsync(NpaTokenPayment ct, CancellationToken t = default)
    {
        var response = await client.PostAsync("Token",
            new StringContent(JsonConvert.SerializeObject(ct), Encoding.UTF8, "application/json"), t);
        await ThrowIfInvalidResponse(response);
            
        var raw = await response.Content.ReadAsStringAsync(t);
        var result = JsonConvert.DeserializeObject<NpaCardResponse>(raw);
        if (result == null)
        {
            throw new InfrastructureLayerException($"Valid response not received from endpoint {raw}");
        }

        return result;
    }
}