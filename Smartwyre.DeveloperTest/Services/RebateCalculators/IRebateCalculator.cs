using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

internal interface IRebateCalculator
{
    IncentiveType IncentiveType { get; }
    CalculateRebateResult Calculate(CalculateRebateRequest request, Rebate rebate, Product product);
}

