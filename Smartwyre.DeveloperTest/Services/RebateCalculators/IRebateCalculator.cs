using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public interface IRebateCalculator
{
    IncentiveType IncentiveType { get; }
    CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product);
}

