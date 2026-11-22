namespace Homework1.Application.UseCases.GenerateReport;

public readonly record struct ClientReportDto(
    long ClientId,
    decimal OrdersSum,
    string FavoriteItemName
);