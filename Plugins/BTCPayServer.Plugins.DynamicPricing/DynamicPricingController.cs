using System;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Abstractions.Extensions;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Client;
using BTCPayServer.Data;
using BTCPayServer.Models;
using BTCPayServer.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BTCPayServer.Plugins.DynamicPricing
{
    [Route("plugins/[controller]")]
    [Authorize(Policy = Policies.CanModifyStoreSettings, AuthenticationSchemes = AuthenticationSchemes.Cookie)]
    public class DynamicPricingController : Controller
    {
        private readonly StoreRepository _storeRepository;
        private readonly ILogger<DynamicPricingController> _logger;

        public DynamicPricingController(
            StoreRepository storeRepository,
            ILogger<DynamicPricingController> logger)
        {
            _storeRepository = storeRepository;
            _logger = logger;
        }

        [HttpGet("{storeId}")]
        public async Task<IActionResult> UpdateDynamicPricingSettings(string storeId)
        {
            var store = HttpContext.GetStoreData();
            if (store == null)
                return NotFound();

            var settings = await _storeRepository.GetSettingAsync<DynamicPricingSettings>(storeId, nameof(DynamicPricingSettings)) 
                ?? new DynamicPricingSettings();
            
            // Initialize empty arrays if they're null to prevent NullReferenceException in the view
            settings.ShippingThresholds ??= new ShippingThreshold[0];
            settings.DiscountThresholds ??= new DiscountThreshold[0];
            
            return View(settings);
        }

        [HttpPost("{storeId}")]
        public async Task<IActionResult> UpdateDynamicPricingSettings(string storeId, DynamicPricingSettings settings)
        {
            if (!ModelState.IsValid)
            {
                // Initialize empty arrays if they're null to prevent NullReferenceException in the view
                settings.ShippingThresholds ??= new ShippingThreshold[0];
                settings.DiscountThresholds ??= new DiscountThreshold[0];
                return View(settings);
            }

            var store = HttpContext.GetStoreData();
            if (store == null)
                return NotFound();

            // Ensure arrays are not null before saving
            settings.ShippingThresholds ??= new ShippingThreshold[0];
            settings.DiscountThresholds ??= new DiscountThreshold[0];

            await _storeRepository.UpdateSetting(storeId, nameof(DynamicPricingSettings), settings);

            TempData.SetStatusMessageModel(new StatusMessageModel
            {
                Message = "Dynamic Pricing settings updated successfully.",
                Severity = StatusMessageModel.StatusSeverity.Success
            });

            return RedirectToAction(nameof(UpdateDynamicPricingSettings), new { storeId });
        }
    }
} 