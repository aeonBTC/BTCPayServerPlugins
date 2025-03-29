using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Extensions;
using BTCPayServer.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.DynamicPricing
{
    public class DynamicPricingPlugin : BaseBTCPayServerPlugin
    {
        public override IBTCPayServerPlugin.PluginDependency[] Dependencies { get; } =
        {
            new() { Identifier = nameof(BTCPayServer), Condition = ">=2.0.0" }
        };

        public override void Execute(IServiceCollection applicationBuilder)
        {
            applicationBuilder.AddSingleton<DynamicPricingService>();
            applicationBuilder.AddHostedService(s => s.GetRequiredService<DynamicPricingService>());
            
            // Add UI Extensions
            applicationBuilder.AddUIExtension("invoice-checkout-payment-method", "DynamicPricing/CheckoutExtension");
            applicationBuilder.AddUIExtension("header-nav", "DynamicPricing/NavExtension");
            
            base.Execute(applicationBuilder);
        }
    }
} 