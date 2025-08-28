namespace HamamPos.Shared.Dtos;

// ==== Requests ====
public record OpenTicketRequest(int ServiceUnitId);
public record AddItemRequest(int TicketId, int ProductId, decimal Quantity);
public enum PayMethod { Cash = 1, Card = 2 }
public record PayTicketRequest(int TicketId, PayMethod Method);

// ==== Responses ====
public record TicketItemDto(int Id, int ProductId, string ProductName, decimal Quantity, decimal UnitPrice, decimal LineTotal);
public record PaymentDto(string Type, decimal Amount, DateTime CollectedAtUtc);

public record TicketDto(
    int Id,
    int ServiceUnitId,
    string OpenedBy,
    DateTime OpenedAtUtc,
    DateTime? ClosedAtUtc,
    decimal Total,
    decimal Paid,
    decimal Balance,
    bool IsClosed,
    IReadOnlyList<TicketItemDto> Items,
    IReadOnlyList<PaymentDto> Payments
);
