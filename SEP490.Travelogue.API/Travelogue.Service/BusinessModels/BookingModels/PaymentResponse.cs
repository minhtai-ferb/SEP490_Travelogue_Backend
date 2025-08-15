using Newtonsoft.Json;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class PaymentResponse
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("data")]
    public PaymentDataJson Data { get; set; }

    [JsonProperty("signature")]
    public string Signature { get; set; }
}

public class PaymentDataJson
{
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("reference")]
    public string Reference { get; set; }

    [JsonProperty("transactionDateTime")]
    public string TransactionDateTime { get; set; }

    [JsonProperty("virtualAccountNumber")]
    public string VirtualAccountNumber { get; set; }

    [JsonProperty("counterAccountBankId")]
    public string CounterAccountBankId { get; set; }

    [JsonProperty("counterAccountBankName")]
    public string CounterAccountBankName { get; set; }

    [JsonProperty("counterAccountName")]
    public string CounterAccountName { get; set; }

    [JsonProperty("counterAccountNumber")]
    public string CounterAccountNumber { get; set; }

    [JsonProperty("virtualAccountName")]
    public string VirtualAccountName { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; }

    [JsonProperty("paymentLinkId")]
    public string PaymentLinkId { get; set; }
}