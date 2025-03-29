using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.DynamicPricing
{
    public class DynamicPricingSettings
    {
        public DynamicPricingSettings()
        {
            // Initialize arrays by default to prevent null reference exceptions
            ShippingThresholds = new ShippingThreshold[0];
            DiscountThresholds = new DiscountThreshold[0];
        }

        [Display(Name = "Enable dynamic shipping discounts")]
        public bool EnableShippingDiscounts { get; set; }

        [Display(Name = "Enable order total discounts")]
        public bool EnableOrderDiscounts { get; set; }

        public ShippingThreshold[] ShippingThresholds { get; set; }

        public DiscountThreshold[] DiscountThresholds { get; set; }
    }

    public class ShippingThreshold
    {
        [Required]
        [Display(Name = "Order Total (≥)")]
        public decimal OrderTotal { get; set; }

        [Required]
        [Display(Name = "Shipping Cost")]
        public decimal ShippingCost { get; set; }
    }

    public class DiscountThreshold
    {
        [Required]
        [Display(Name = "Order Total (≥)")]
        public decimal OrderTotal { get; set; }

        [Required]
        [Display(Name = "Discount Percentage")]
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
    }
} 