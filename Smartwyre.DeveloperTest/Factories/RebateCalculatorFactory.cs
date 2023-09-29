using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Smartwyre.DeveloperTest.Factories;

internal sealed class RebateCalculatorFactory
{
    private static Dictionary<IncentiveType, IRebateCalculator> _rebateCalculators = new Dictionary<IncentiveType, IRebateCalculator>();

    static RebateCalculatorFactory()
    {
        var calculatorTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(IRebateCalculator).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach(var calculatorType in calculatorTypes)
        {
            var calculator = (IRebateCalculator)Activator.CreateInstance(calculatorType);
            _rebateCalculators.Add(calculator.IncentiveType, calculator);
        }
    }

    public static IRebateCalculator CreateRebateCalculator(Rebate rebate)
    {
        return _rebateCalculators[rebate.Incentive];
    }
}
