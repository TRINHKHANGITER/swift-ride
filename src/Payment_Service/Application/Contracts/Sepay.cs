namespace Payment_Service.Application.Contracts;

public sealed class SepayIpnContract
{
    public int Id { get; set; }
    public string Gateway { get; set; } = string.Empty;
    public string TransactionDate { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? SubAccount { get; set; }
    public string? Code { get; set; }
    public string Content { get; set; } = string.Empty;
    public string TransferType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TransferAmount { get; set; }
    public int Accumulated { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;

}

public sealed record SepayIpnResponseContract(bool Success, string Message);
