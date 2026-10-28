namespace Homework1.Application.UseCases.GenerateReport;

public readonly record struct ClientReportDto(
    long ClientId,
    float OrdersSum,
    string FavoriteItemName
);