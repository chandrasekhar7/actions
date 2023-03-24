using System;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Npa.Accounting.Infrastructure.Attributes;
using Npa.Accounting.Infrastructure.Newtonsoft.Converters;

namespace Npa.Accounting.Infrastructure.Newtonsoft.ContractResolvers;

public class SnakeCaseContractResolver : DefaultContractResolver {
    public new static readonly SnakeCaseContractResolver Instance = new SnakeCaseContractResolver();

    protected override JsonContract CreateContract(Type objectType) {
        JsonContract contract = base.CreateContract(objectType);

        if (objectType?.GetCustomAttributes(true).OfType<SnakeCasedAttribute>().Any() == true) {
            contract.Converter = new SnakeCaseConverter();
        }

        return contract;
    }
}