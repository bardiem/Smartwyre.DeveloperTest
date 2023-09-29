using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal sealed class FixedRateRebateCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedRateRebate;

    public CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();

        var incentiveTypeIsNotSupported = !product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate);
        var rebateAmountIsIncorrect = rebate.Percentage <= 0 || product.Price <= 0 || request.Volume <= 0;
        
        if (incentiveTypeIsNotSupported || rebateAmountIsIncorrect)
        {
            result.Success = false;
            return result;
        }
        
        result.RebateAmount = product.Price * rebate.Percentage * request.Volume;
        result.Success = true;

        return result;
    }
}