using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Factories;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public sealed class RebateService : IRebateService
{
    private readonly RebateDataStore _rebateData;
    private readonly ProductDataStore _productData;

    public RebateService(RebateDataStore rebateData, ProductDataStore productData)
    {
        _rebateData = rebateData;
        _productData = productData;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateData.GetRebate(request?.RebateIdentifier);
        var product = _productData.GetProduct(request?.ProductIdentifier);

        if (rebate == null || product == null)
        {
            return new CalculateRebateResult { Success = false, RebateAmount = 0 };
        }

        var calculator = RebateCalculatorFactory.CreateRebateCalculator(rebate);

        var result = calculator.Calculate(request, rebate, product);

        if (result.Success)
        {
            _rebateData.StoreCalculationResult(rebate, result.RebateAmount);
        }

        return result;
    }
}
