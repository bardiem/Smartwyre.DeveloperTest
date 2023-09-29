using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal class AmountPerUomCalculator : IRebateCalculator
{
    private const IncentiveType Type = IncentiveType.AmountPerUom;

    public IncentiveType IncentiveType => Type;

    public CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var result = new CalculateRebateResult();

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
        {
            result.Success = false;
        }
        else if (rebate.Amount == 0 || request.Volume == 0)
        {
            result.Success = false;
        }
        else
        {
            result.RebateAmount = rebate.Amount * request.Volume;
            result.Success = true;
        }

        return result;
    }
}