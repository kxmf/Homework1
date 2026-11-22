using FluentAssertions;

using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Domain;

using NSubstitute;

using Xunit;

namespace Homework1.Tests.Application;

public class GenerateReportUseCaseTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IReportWriter _consoleWriter = Substitute.For<IReportWriter>();
    private readonly IReportWriter _fileWriter = Substitute.For<IReportWriter>();
    private readonly GenerateReportUseCase _sut;

    public GenerateReportUseCaseTests()
    {
        _consoleWriter.Mode.Returns(OutputMode.Console);
        _fileWriter.Mode.Returns(OutputMode.File);

        _sut = new GenerateReportUseCase(_orderRepository, [_consoleWriter, _fileWriter]);
    }

    [Fact]
    public void Execute_WhenClientIdsIsNull_ThrowsArgumentNullException()
    {
        var act = () => _sut.Execute(null!, OutputMode.Console);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-42)]
    public void Execute_WhenClientIdIsNonPositive_ThrowsInvalidIdException(long invalidId)
    {
        var act = () => _sut.Execute([invalidId], OutputMode.Console);

        act.Should().Throw<InvalidIdException>()
            .WithMessage($"*{invalidId}*");
    }

    [Fact]
    public void Execute_WhenClientHasNoOrders_ThrowsClientNotFoundException()
    {
        long clientId = 1010;
        _orderRepository.GetByClientId(clientId).Returns([]);

        var act = () => _sut.Execute([clientId], OutputMode.Console);

        act.Should().Throw<ClientNotFoundException>()
            .WithMessage($"*{clientId}*");
    }

    [Fact]
    public void Execute_WhenSingleClientHasOrders_CalculatesSumAndFavoriteItemCorrectly()
    {
        long clientId = 1010;
        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                ClientId = clientId,
                Items =
                {
                    new OrderItem { Name = "Огурчики маринованные", UnitPrice = 100.00m, Quantity = 1 },
                    new OrderItem { Name = "Чипсеки", UnitPrice = 99.99m, Quantity = 4 }
                }
            }
        };

        _orderRepository.GetByClientId(clientId).Returns(orders);

        IReadOnlyCollection<ClientReportDto>? writtenReports = null;
        _consoleWriter.When(w => w.Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>()))
            .Do(call => writtenReports = call.Arg<IReadOnlyCollection<ClientReportDto>>());

        _sut.Execute([clientId], OutputMode.Console);

        _consoleWriter.Received(1).Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>());
        _fileWriter.DidNotReceive().Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>());

        writtenReports.Should().NotBeNull().And.HaveCount(1);
        var report = writtenReports!.First();

        report.ClientId.Should().Be(clientId);
        report.OrdersSum.Should().Be(499.96m);
        report.FavoriteItemName.Should().Be("Чипсеки");
    }

    [Fact]
    public void Execute_WhenMultipleOrdersContainSameItem_SumsQuantitiesToFindFavorite()
    {
        long clientId = 1010;
        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                ClientId = clientId,
                Items =
                {
                    new OrderItem { Name = "Шоколад", UnitPrice = 150m, Quantity = 3 },
                    new OrderItem { Name = "Кофе", UnitPrice = 200m, Quantity = 2 }
                }
            },
            new()
            {
                Id = 2,
                ClientId = clientId,
                Items =
                {
                    new OrderItem { Name = "Кофе", UnitPrice = 200m, Quantity = 4 }
                }
            }
        };

        _orderRepository.GetByClientId(clientId).Returns(orders);

        IReadOnlyCollection<ClientReportDto>? writtenReports = null;
        _consoleWriter.When(w => w.Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>()))
            .Do(call => writtenReports = call.Arg<IReadOnlyCollection<ClientReportDto>>());

        _sut.Execute([clientId], OutputMode.Console);

        writtenReports.Should().NotBeNull().And.HaveCount(1);
        var report = writtenReports!.First();

        report.OrdersSum.Should().Be(3 * 150m + 2 * 200m + 4 * 200m);
        report.FavoriteItemName.Should().Be("Кофе");
    }

    [Fact]
    public void Execute_WhenMultipleClientsProvided_GeneratesReportForEach()
    {
        long client1 = 1010;
        long client2 = 1020;

        _orderRepository.GetByClientId(client1).Returns(
        [
            new()
            {
                Id = 1,
                ClientId = client1,
                Items = { new OrderItem { Name = "Товар 1", UnitPrice = 50m, Quantity = 2 } }
            }
        ]);

        _orderRepository.GetByClientId(client2).Returns(
        [
            new()
            {
                Id = 2,
                ClientId = client2,
                Items = { new OrderItem { Name = "Товар 2", UnitPrice = 100m, Quantity = 5 } }
            }
        ]);

        IReadOnlyCollection<ClientReportDto>? writtenReports = null;
        _consoleWriter.When(w => w.Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>()))
            .Do(call => writtenReports = call.Arg<IReadOnlyCollection<ClientReportDto>>());

        _sut.Execute([client1, client2], OutputMode.Console);

        writtenReports.Should().NotBeNull().And.HaveCount(2);
        writtenReports.Should().Contain(r => r.ClientId == client1 && r.OrdersSum == 100m && r.FavoriteItemName == "Товар 1");
        writtenReports.Should().Contain(r => r.ClientId == client2 && r.OrdersSum == 500m && r.FavoriteItemName == "Товар 2");
    }

    [Fact]
    public void Execute_WhenOutputModeIsFile_CallsFileWriter()
    {
        long clientId = 1010;
        _orderRepository.GetByClientId(clientId).Returns(
        [
            new()
            {
                Id = 1,
                ClientId = clientId,
                Items = { new OrderItem { Name = "Печенье", UnitPrice = 75m, Quantity = 1 } }
            }
        ]);

        _sut.Execute([clientId], OutputMode.File);

        _fileWriter.Received(1).Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>());
        _consoleWriter.DidNotReceive().Write(Arg.Any<IReadOnlyCollection<ClientReportDto>>());
    }

    [Fact]
    public void Execute_WhenWriterNotFoundForOutputMode_ThrowsReportWriterNotFoundException()
    {
        var useCaseWithoutWriters = new GenerateReportUseCase(_orderRepository, []);
        long clientId = 1010;
        _orderRepository.GetByClientId(clientId).Returns(
        [
            new()
            {
                Id = 1,
                ClientId = clientId,
                Items = { new OrderItem { Name = "Чай", UnitPrice = 120m, Quantity = 1 } }
            }
        ]);

        var act = () => useCaseWithoutWriters.Execute([clientId], OutputMode.Console);

        act.Should().Throw<ReportWriterNotFoundException>();
    }
}