using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var rebateDataStore = new RebateDataStore();
        var productDataStore = new ProductDataStore();

        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest { Volume = 1 };
        Console.WriteLine(service.Calculate(request).Success);
        //result will be false because DataStore doesn't work and rebase is empty
        Console.Read();
    }
}
