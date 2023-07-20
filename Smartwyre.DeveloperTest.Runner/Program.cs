using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        IRebateDataStore rebateDataStore = new RebateDataStore();
        IProductDataStore productDataStore = new ProductDataStore();
        IRebateService Services = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest {
            RebateIdentifier = "Test_Rebate_Identifier",
            ProductIdentifier = "Test_Product_Identifier",
            Volume = 100
        };
        var result = Services.Calculate(request);

        Console.WriteLine($"Rebate Result: {result.Success}");
        if (result.Success) {
            Console.WriteLine($"Rebate Amount: {result.RebateAmount}");
        }
    }
}
