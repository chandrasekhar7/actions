using System;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions;

public record ReturnMessage
{
    public CardReturnStatus Status { get; }
    public string Code { get; }
    public string Message { get; }
    public string? RefNum { get; }
    private ReturnMessage()
    {
    }

    public ReturnMessage(CardReturnStatus status, string code, string message, string refNum)
    {
        Status = status;
        Code = code;
        Message = message;
        RefNum = refNum;
    }

    public static ReturnMessage Default => new ReturnMessage(CardReturnStatus.NotStarted, String.Empty, String.Empty, String.Empty);
    public override string ToString() => $"{Status.ToString()}: {Message} ({Code}), RefNum: {RefNum}";
}