using System.Text;

using FluentAssertions;

using Homework1.Infrastructure.Persistence;

using Xunit;

namespace Homework1.Tests.Infrastructure;

public class CsvOrderRepositoryTests : IDisposable
{
    private readonly string _tempFilePath;

    public CsvOrderRepositoryTests()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"orders_test_{Guid.NewGuid():N}.csv");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [Fact]
    public void GetByClientId_WhenFileDoesNotExist_ReturnsEmptyCollection()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.csv");
        var repository = new CsvOrderRepository(nonExistentPath);

        // Act
        var result = repository.GetByClientId(1010);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetByClientId_WhenValidCsvProvided_ParsesAndGroupsOrdersCorrectly()
    {
        // Arrange
        var csv = """
            client_id,order_id,item_name,item_quantity,item_price
            1010,1,Огурчики маринованные,1,100
            1010,1,Чипсеки,4,99.99
            1010,2,Вода газированная,2,45.50
            1020,3,Сырок со сгущенкой,20,33
            """;
        File.WriteAllText(_tempFilePath, csv, Encoding.UTF8);

        var repository = new CsvOrderRepository(_tempFilePath);

        // Act
        var orders = repository.GetByClientId(1010);

        // Assert
        orders.Should().HaveCount(2);

        var order1 = orders.Single(o => o.Id == 1);
        order1.ClientId.Should().Be(1010);
        order1.Items.Should().HaveCount(2);
        order1.Items.Should().Contain(i => i.Name == "Огурчики маринованные" && i.Quantity == 1 && i.UnitPrice == 100m);
        order1.Items.Should().Contain(i => i.Name == "Чипсеки" && i.Quantity == 4 && i.UnitPrice == 99.99m);

        var order2 = orders.Single(o => o.Id == 2);
        order2.ClientId.Should().Be(1010);
        order2.Items.Should().ContainSingle(i => i.Name == "Вода газированная" && i.Quantity == 2 && i.UnitPrice == 45.50m);
    }

    [Fact]
    public void GetByClientId_WhenClientHasNoOrdersInCsv_ReturnsEmptyCollection()
    {
        // Arrange
        var csv = """
            client_id,order_id,item_name,item_quantity,item_price
            1020,1,Сырок,10,30
            """;
        File.WriteAllText(_tempFilePath, csv, Encoding.UTF8);

        var repository = new CsvOrderRepository(_tempFilePath);

        // Act
        var orders = repository.GetByClientId(1010);

        // Assert
        orders.Should().BeEmpty();
    }

    [Fact]
    public void GetByClientId_WhenCsvContainsInvalidOrCorruptedLines_SkipsThemAndParsesValidRows()
    {
        // Arrange
        var csv = """
            client_id,order_id,item_name,item_quantity,item_price
            not_a_number,1,Товар,1,10
            -5,1,Товар,1,10
            1010,-1,Товар,1,10
            1010,1,,1,10
            1010,1,Товар,not_an_int,10
            1010,1,Товар,-1,10
            1010,1,Товар,1,invalid_price
            1010,1,Товар,1,-5.50
            1010,1,Только,три,колонки
            
            1010,1,Валидный товар,3,25.50
            """;
        File.WriteAllText(_tempFilePath, csv, Encoding.UTF8);

        var repository = new CsvOrderRepository(_tempFilePath);

        // Act
        var orders = repository.GetByClientId(1010);

        // Assert
        orders.Should().HaveCount(1);
        var order = orders.Single();
        order.Id.Should().Be(1);
        order.Items.Should().ContainSingle(i => i.Name == "Валидный товар" && i.Quantity == 3 && i.UnitPrice == 25.50m);
    }
}