using System;

namespace Npa.Accounting.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SnakeCasedAttribute : Attribute {
    public SnakeCasedAttribute() {
        // intended blank
    }
}