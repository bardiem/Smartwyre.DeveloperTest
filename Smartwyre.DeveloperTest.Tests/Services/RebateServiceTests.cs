
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests.Services;

public class RebateServiceTests
{
    private const string ProductIdentifier = "prod";
    private const string RebateIdentifier = "rebate";

    private readonly Mock<RebateDataStore> _rebateDataMock;
    private readonly Mock<ProductDataStore> _productDataMock;

    public RebateServiceTests()
    {
        _rebateDataMock = new Mock<RebateDataStore>();
        _productDataMock = new Mock<ProductDataStore>();
    }

    [Fact]
    public void Calculate_ShouldReturnFalse_WhenRequestIsNull()
    {
        // Arrange
        CalculateRebateRequest request = null;

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom)]
    public void Calculate_ShoultReturnFalse_WhenProductNotFound(IncentiveType incentiveType)
    {
        // Arrange
        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = incentiveType,
                Amount = 1,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product());

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = "random",
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom)]
    public void Calculate_ShoultReturnFalse_WhenRebateNotFound(IncentiveType incentiveType)
    {
        // Arrange
        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate());

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = "random",
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnTrue_WhenIncentiveIsFixedCashAmountAndIsCorrect()
    {
        // Arrange
        decimal amount = 11;

        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.FixedCashAmount,
                Amount = amount,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(true);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.Is<Rebate>(x => x.Amount == amount && x.Incentive == IncentiveType.FixedCashAmount),
                It.Is<decimal>(x => x == amount)), Times.Once);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsFixedCashAmountAndAmountIs0()
    {
        // Arrange
        decimal amount = 0;

        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.FixedCashAmount,
                Amount = amount,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsFixedCashAmountAndIncentiveIsNotSupported()
    {
        // Arrange
        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Amount = 1,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnTrue_WhenIncentiveIsFixedRateRebateAndIsCorrect()
    {
        // Arrange
        decimal percentage = 0.5m;
        decimal productPrice = 2m;
        decimal volume = 2m;


        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = percentage
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = productPrice
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = volume
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(true);

        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.Is<Rebate>(x => x.Percentage == percentage && x.Incentive == IncentiveType.FixedRateRebate),
                It.Is<decimal>(x => x == percentage * productPrice * volume)), Times.Once);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsFixedRateRebateAndPercentageIs0()
    {
        // Arrange
        decimal percentage = 0;

        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = percentage,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsFixedRateRebateAndIncentiveIsNotSupported()
    {
        // Arrange
        decimal percentage = 0.5m;
        decimal productPrice = 2m;
        decimal volume = 2m;


        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Percentage = percentage
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = productPrice
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = volume
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnTrue_WhenIncentiveIsAmountPerUomAndIsCorrect()
    {
        // Arrange
        decimal amount = 10m;
        decimal volume = 2m;


        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Amount = amount
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = volume
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(true);

        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.Is<Rebate>(x => x.Amount == amount && x.Incentive == IncentiveType.AmountPerUom),
                It.Is<decimal>(x => x == amount * volume)), Times.Once);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsAmountPerUomAndAmountIs0()
    {
        // Arrange
        decimal amount = 0;

        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Amount = amount,
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = 1
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);
        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_ShoultReturnFalse_WhenIncentiveIsAmountPerUomAndIncentiveIsNotSupported()
    {
        // Arrange
        decimal amount = 10m;
        decimal volume = 2m;


        _rebateDataMock
            .Setup(r => r.GetRebate(It.Is<string>(x => x == RebateIdentifier)))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Amount = amount
            });

        _productDataMock
            .Setup(p => p.GetProduct(It.Is<string>(x => x == ProductIdentifier)))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
            });

        var request = new CalculateRebateRequest
        {
            ProductIdentifier = ProductIdentifier,
            RebateIdentifier = RebateIdentifier,
            Volume = volume
        };

        var sut = new RebateService(_rebateDataMock.Object, _productDataMock.Object);

        // Act
        var result = sut.Calculate(request);

        // Assert
        result.Success.Should().Be(false);

        _rebateDataMock
            .Verify(r => r.StoreCalculationResult(
                It.IsAny<Rebate>(),
                It.IsAny<decimal>()), Times.Never);
    }
}