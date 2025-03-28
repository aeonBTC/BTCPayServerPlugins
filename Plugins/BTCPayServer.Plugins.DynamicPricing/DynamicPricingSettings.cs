using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.DynamicPricing
{
    public class DynamicPricingSettings
    {
        [Display(Name = "Enable dynamic shipping discounts")]
        public bool EnableShippingDiscounts { get; set; } = true;

        [Display(Name = "Enable order total discounts")]
        public bool EnableOrderDiscounts { get; set; } = true;

        public List<ShippingThreshold> ShippingThresholds { get; set; } = new List<ShippingThreshold>
        {
            new ShippingThreshold { OrderTotal = 50, ShippingCost = 5 },
            new ShippingThreshold { OrderTotal = 100, ShippingCost = 2.5M },
            new ShippingThreshold { OrderTotal = 200, ShippingCost = 0 }
        };

        public List<DiscountThreshold> DiscountThresholds { get; set; } = new List<DiscountThreshold>
        {
            new DiscountThreshold { OrderTotal = 50, DiscountPercentage = 5 },
            new DiscountThreshold { OrderTotal = 100, DiscountPercentage = 10 },
            new DiscountThreshold { OrderTotal = 200, DiscountPercentage = 15 }
        };
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