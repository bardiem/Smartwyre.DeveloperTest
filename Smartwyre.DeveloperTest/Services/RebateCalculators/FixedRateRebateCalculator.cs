using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal class FixedRateRebateCalculator : IRebateCalculator
{
    private const IncentiveType Type = IncentiveType.FixedRateRebate;

    public IncentiveType IncentiveType => Type;

    public CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
        {
            result.Success = false;
        }
        else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        {
            result.Success = false;
        }
        else
        {
            result.RebateAmount = product.Price * rebate.Percentage * request.Volume;
            result.Success = true;
        }

        return result;
    }
}