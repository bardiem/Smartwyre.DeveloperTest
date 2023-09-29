using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal sealed class FixedCashAmountCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedCashAmount;

    public CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();

        var incentiveTypeIsNotSupported = !product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount);
        var amountIsIncorrect = rebate.Amount <= 0;

        if (incentiveTypeIsNotSupported || amountIsIncorrect)
        {
            result.Success = false;
            return result;
        }
        
        result.RebateAmount = rebate.Amount;
        result.Success = true;

        return result;
    }
}