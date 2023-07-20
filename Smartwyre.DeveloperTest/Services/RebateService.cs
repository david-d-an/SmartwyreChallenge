// using System;
using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private const decimal INVALID_REBATE_AMOUNT = -1000000;
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
    }
    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        try {
            // Inject values
            // var rebateDataStore = new RebateDataStore();
            // var productDataStore = new ProductDataStore();

            Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
            Product product = _productDataStore.GetProduct(request.ProductIdentifier);

            if (rebate == null || product == null) {
                return new CalculateRebateResult {
                    Success = false
                };
            }

            var rebateAmount = GetRebateAmount(product, rebate, request);
            if (rebateAmount == INVALID_REBATE_AMOUNT) {
                return new CalculateRebateResult {
                    Success = false
                };
            }

            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
            return new CalculateRebateResult {
                Success = true,
                RebateAmount = rebateAmount
            };
        } catch(Exception ex) {
            Console.WriteLine($"Exception during RebateService.Calculate(): {ex.Message}");
            return new CalculateRebateResult {
                Success = false
            };
        }
    }

  // TODO: Check possibilites to extend Product to embed logic to calculate Rebate
  //       That might be mixing data and logic mixed together and potentially violation of layer.
  //       Need discussion.
  private decimal GetRebateAmount(Product product, Rebate rebate, CalculateRebateRequest request) {
        /**
        * Original Logic
        */
        // switch (rebate.Incentive)
        // {
        //     case IncentiveType.FixedCashAmount:
        //         if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
        //         {
        //             result.Success = false;
        //         }
        //         else if (rebate.Amount == 0)
        //         {
        //             result.Success = false;
        //         }
        //         else
        //         {
        //             rebateAmount = rebate.Amount;
        //             result.Success = true;
        //         }
        //         break;

        //     case IncentiveType.FixedRateRebate:
        //         if (product == null)
        //         {
        //             result.Success = false;
        //         }
        //         else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
        //         {
        //             result.Success = false;
        //         }
        //         else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        //         {
        //             result.Success = false;
        //         }
        //         else
        //         {
        //             rebateAmount += product.Price * rebate.Percentage * request.Volume;
        //             result.Success = true;
        //         }
        //         break;

        //     case IncentiveType.AmountPerUom:
        //         if (product == null)
        //         {
        //             result.Success = false;
        //         }
        //         else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
        //         {
        //             result.Success = false;
        //         }
        //         else if (rebate.Amount == 0 || request.Volume == 0)
        //         {
        //             result.Success = false;
        //         }
        //         else
        //         {
        //             rebateAmount += rebate.Amount * request.Volume;
        //             result.Success = true;
        //         }
        //         break;
        // }

        if (rebate.Incentive == IncentiveType.FixedCashAmount) {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount)) {
                return INVALID_REBATE_AMOUNT;
            }
            else if (rebate.Amount == 0) {
                return INVALID_REBATE_AMOUNT;
            }
            else {
                return rebate.Amount;
            }
        }
        else if (rebate.Incentive == IncentiveType.FixedRateRebate) {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate)) {
                return INVALID_REBATE_AMOUNT;
            }
            else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0) {
                return INVALID_REBATE_AMOUNT;
            }
            else {
                return product.Price * rebate.Percentage * request.Volume;
            }
        }
        else if (rebate.Incentive == IncentiveType.AmountPerUom) {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom)) {
                return INVALID_REBATE_AMOUNT;
            }
            else if (rebate.Amount == 0 || request.Volume == 0) {
                return INVALID_REBATE_AMOUNT;
            }
            else {
                return rebate.Amount * request.Volume;
            }
        }

        return INVALID_REBATE_AMOUNT;
    }
}
