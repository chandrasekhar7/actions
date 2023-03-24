using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Infrastructure.Helpers;
using Npa.Accounting.Infrastructure.Models;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Infrastructure.Customers;

public class CustomerInfoRepository : ICustomerInfoRepository
{
    private HttpClient client;

    public CustomerInfoRepository(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }
    public async Task<CustomerInfo> GetInfoById(int powerId, CancellationToken t = default)
    {
        try
        {
            var response = await client.GetAsync("" + powerId, t);
            await ResponseHelper.ThrowIfInvalidResponse(response);

            var customersApiResponse = new JsonSerializer()
                .Deserialize<CustomerInfoResponse>(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync(t))));

            var obj = new CustomerInfo
            {
                IsLoanLocked = customersApiResponse.ManagementBlock,
                IsPraLimit = customersApiResponse.PraLimit,
                IsMilitary = customersApiResponse.MilitaryStatus == null || !customersApiResponse.MilitaryStatus.IsMilitary ? false : true,
                Location = customersApiResponse.Location
            };
            return obj;
        }
        catch (Exception e)
        {
            throw new InfrastructureLayerException(e.Message);
        }
    }
}