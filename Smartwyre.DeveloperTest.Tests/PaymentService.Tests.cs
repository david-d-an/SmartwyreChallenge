using System;
using Xunit;
using Moq;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Data;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private readonly Mock< IRebateService> _mockRebateService;
    private readonly CalculateRebateRequest _mockCalculateRebateRequest;
    private readonly Mock<IProductDataStore> _mockProductDataStore;
    private readonly Mock<IRebateDataStore> _mockRebateDataStore;
    private readonly IRebateService _rebateService;

  public PaymentServiceTests() {
        // _mockRebateService = new Mock<IRebateService>();
        _mockCalculateRebateRequest = new CalculateRebateRequest {
            RebateIdentifier = "Test_Rebate_Identifier",
            ProductIdentifier = "Test_Product_Identifier",
            Volume = 100
        };
        _mockProductDataStore = new Mock<IProductDataStore>();
        _mockRebateDataStore = new Mock<IRebateDataStore>();

        _rebateService = new RebateService(
            _mockRebateDataStore.Object,
            _mockProductDataStore.Object);
    }

    [Fact]
    public void ShouldNotPassIfProductOrRebateIsNull()
    {
        CalculateRebateResult result;

        // Test #1: Only Product is Null
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns<Product>(null);
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedCashAmount
            });

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        // Test #2: Only Rebate is Null
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns<Rebate>(null);

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        // Test #3: Both Product and Rebate are null
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns<Product>(null);
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns<Rebate>(null);

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(SupportedIncentiveType.FixedCashAmount, IncentiveType.FixedRateRebate)]
    [InlineData(SupportedIncentiveType.FixedCashAmount, IncentiveType.AmountPerUom)]

    [InlineData(SupportedIncentiveType.FixedRateRebate, IncentiveType.FixedCashAmount)]
    [InlineData(SupportedIncentiveType.FixedRateRebate, IncentiveType.AmountPerUom)]

    [InlineData(SupportedIncentiveType.AmountPerUom, IncentiveType.FixedCashAmount)]
    [InlineData(SupportedIncentiveType.AmountPerUom, IncentiveType.FixedRateRebate)]
    public void ShouldNotPassIfProductRebateMistmatch(
        SupportedIncentiveType supportedIncentiveType,
        IncentiveType incentiveType)
    {
        CalculateRebateResult result;

        // Test #1: Only Product is Null
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = supportedIncentiveType
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = incentiveType
            });

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(SupportedIncentiveType.FixedCashAmount)]
    [InlineData(SupportedIncentiveType.FixedRateRebate)]
    [InlineData(SupportedIncentiveType.AmountPerUom)]
    public void ShopuldNotPassIfRebateIncentiveIsInvalid(
        SupportedIncentiveType supportedIncentiveType
    )
    {
        CalculateRebateResult result;

        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = supportedIncentiveType,
                Price = 1
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = (IncentiveType)(-1),   //
                Amount = 1,
                Percentage = 1
            });

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom)]
    public void ShopuldNotPassIfProductIncentiveIsInvalid(
        IncentiveType incentiveType
    )
    {
        CalculateRebateResult result;

        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = (SupportedIncentiveType)(65536),
                Price = 1
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = incentiveType,
                Amount = 1,
                Percentage = 1
            });

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);
    }



    [Fact]
    public void PassRulesOnFixedCashAmount()
    {
        CalculateRebateResult result;

        // if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
        //     return INVALID_REBATE_AMOUNT;
        // else if (rebate.Amount == 0)
        //     return INVALID_REBATE_AMOUNT;
        // else
        //     return rebate.Amount;

        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedCashAmount,
                Amount = 0,
                Percentage = 0
            });

        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        var testRebateAmount = new Random().Next(1, 100);
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedCashAmount,
                Amount = testRebateAmount,
                Percentage = 0
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.True(result.Success);
        Assert.Equal(testRebateAmount, result.RebateAmount);
    }

    [Fact]
    public void PassRulesOnFixedRateRebate() {
        CalculateRebateResult result;

        // if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate)) {
        //     return INVALID_REBATE_AMOUNT;
        // }
        // else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0) {
        //     return INVALID_REBATE_AMOUNT;
        // }
        // else {
        //     return product.Price * rebate.Percentage * request.Volume;
        // }

        // Test #1
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 0
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = 1
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        // Test #2
        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 1
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = 0
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        var testProductPrice = new Random().Next(1, 100);
        var testRebatePercentage = new Random().Next(1, 99);
        var volume = _mockCalculateRebateRequest.Volume;
        var expectedRebateAmt = testProductPrice * testRebatePercentage * volume;

        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = testProductPrice
            });
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = testRebatePercentage
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.True(result.Success);
        Assert.Equal(expectedRebateAmt, result.RebateAmount);
    }

    [Fact]
    public void PassRulesOnAmountPerUom() {
        CalculateRebateResult result;

        // if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom)) {
        //     return INVALID_REBATE_AMOUNT;
        // }
        // else if (rebate.Amount == 0 || request.Volume == 0) {
        //     return INVALID_REBATE_AMOUNT;
        // }
        // else {
        //     return rebate.Amount * request.Volume;
        // }

        _mockProductDataStore
            .Setup(x => x.GetProduct(It.IsAny<string>()))
            .Returns(new Product{
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
            });

        // Test #1
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.AmountPerUom,
                Amount = 0
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.False(result.Success);

        // Test #2
        var testRebateAmount = new Random().Next(1, 99);
        var volume = _mockCalculateRebateRequest.Volume;
        var expectedRebateAmt = testRebateAmount * volume;
        _mockRebateDataStore
            .Setup(x => x.GetRebate(It.IsAny<string>()))
            .Returns(new Rebate{
                Incentive = IncentiveType.AmountPerUom,
                Amount = testRebateAmount
            });
        result = _rebateService.Calculate(_mockCalculateRebateRequest);
        Assert.True(result.Success);
        Assert.Equal(expectedRebateAmt, result.RebateAmount);
    }
}
