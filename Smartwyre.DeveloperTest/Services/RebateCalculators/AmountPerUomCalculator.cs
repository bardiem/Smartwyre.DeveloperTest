using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal sealed class AmountPerUomCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.AmountPerUom;

    public CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();

        var incentiveTypeIsNotSupported = !product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom);
        var rebateAmountIsIncorrect = rebate.Amount <= 0 || request.Volume <= 0;

        if (incentiveTypeIsNotSupported || rebateAmountIsIncorrect)
        {
            result.Success = false;
            return result;
        }
        
        result.RebateAmount = rebate.Amount * request.Volume;
        result.Success = true;

        return result;
    }
}