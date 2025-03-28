using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Events;
using BTCPayServer.HostedServices;
using BTCPayServer.Logging;
using BTCPayServer.Payments;
using BTCPayServer.Services.Invoices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BTCPayServer.Plugins.DynamicPricing
{
    public class DynamicPricingService : EventHostedServiceBase
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly ILogger<DynamicPricingService> _logger;

        public DynamicPricingService(
            EventAggregator eventAggregator,
            InvoiceRepository invoiceRepository,
            ILogger<DynamicPricingService> logger) : base(eventAggregator, logger)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        protected override void SubscribeToEvents()
        {
            Subscribe<InvoiceEvent>();
            base.SubscribeToEvents();
        }

        protected override async Task ProcessEvent(object evt, CancellationToken cancellationToken)
        {
            if (evt is InvoiceEvent invoiceEvent && 
                invoiceEvent.EventCode == InvoiceEventCode.Created)
            {
                await ProcessNewInvoice(invoiceEvent.Invoice);
            }
        }

        private async Task ProcessNewInvoice(InvoiceEntity invoice)
        {
            try
            {
                // Check if we have dynamic pricing settings in the metadata
                if (!invoice.Metadata.AdditionalData.TryGetValue("dynamicPricingSettings", out var settingsJson))
                {
                    // No settings found, nothing to do
                    return;
                }

                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<DynamicPricingSettings>(settingsJson.ToString());
                if (settings == null)
                {
                    return;
                }
                
                // Calculate order total (excluding shipping)
                decimal orderTotal = invoice.Price;
                decimal originalShipping = 0;

                // Extract shipping cost from invoice if available
                if (invoice.Metadata.AdditionalData.TryGetValue("shippingCost", out var shippingCostJson))
                {
                    originalShipping = decimal.Parse(shippingCostJson.ToString());
                    orderTotal -= originalShipping;
                }

                bool modified = false;
                var invoiceLogs = new InvoiceLogs();

                // Apply shipping discounts if enabled
                if (settings.EnableShippingDiscounts && originalShipping > 0)
                {
                    var applicableThreshold = settings.ShippingThresholds
                        .Where(t => orderTotal >= t.OrderTotal)
                        .OrderByDescending(t => t.OrderTotal)
                        .FirstOrDefault();

                    if (applicableThreshold != null)
                    {
                        decimal newShippingCost = applicableThreshold.ShippingCost;
                        
                        if (newShippingCost != originalShipping)
                        {
                            // Update the invoice price
                            invoice.Price = orderTotal + newShippingCost;
                            modified = true;

                            invoiceLogs.Write($"Applied shipping discount. Original shipping: {originalShipping}, New shipping: {newShippingCost}",
                                LogSeverity.Info);
                        }
                    }
                }

                // Apply order discounts if enabled
                if (settings.EnableOrderDiscounts)
                {
                    var applicableThreshold = settings.DiscountThresholds
                        .Where(t => orderTotal >= t.OrderTotal)
                        .OrderByDescending(t => t.OrderTotal)
                        .FirstOrDefault();

                    if (applicableThreshold != null && applicableThreshold.DiscountPercentage > 0)
                    {
                        decimal discountAmount = orderTotal * (applicableThreshold.DiscountPercentage / 100);
                        decimal discountedTotal = orderTotal - discountAmount;
                        
                        // Update the invoice price (add back the shipping cost)
                        decimal newShippingCost = originalShipping;
                        if (modified)
                        {
                            // If shipping was already modified, get the current price minus the order total
                            newShippingCost = invoice.Price - orderTotal;
                        }

                        invoice.Price = discountedTotal + newShippingCost;
                        modified = true;

                        invoiceLogs.Write($"Applied {applicableThreshold.DiscountPercentage}% discount. Discount amount: {discountAmount}",
                            LogSeverity.Info);
                    }
                }

                // Update the invoice if modified
                if (modified)
                {
                    // Update metadata
                    invoice.Metadata.AdditionalData["dynamicPricingApplied"] = true;
                    await _invoiceRepository.UpdateInvoiceMetadata(invoice.Id, invoice.StoreId, invoice.Metadata.ToJObject());
                    
                    // Update invoice price by updating the entire entity
                    // Since UpdateInvoice doesn't exist, we'll need to fetch and update
                    var invoiceToUpdate = await _invoiceRepository.GetInvoice(invoice.Id);
                    if (invoiceToUpdate != null)
                    {
                        invoiceToUpdate.Price = invoice.Price;
                        await _invoiceRepository.UpdateInvoiceMetadata(invoice.Id, invoice.StoreId, invoiceToUpdate.Metadata.ToJObject());
                    }
                    
                    await _invoiceRepository.AddInvoiceLogs(invoice.Id, invoiceLogs);
                    _logger.LogInformation($"Updated pricing for invoice {invoice.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing dynamic pricing for invoice {invoice.Id}");
            }
        }
    }
}
