using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Extensions;
using BTCPayServer.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.DynamicPricing
{
    public class DynamicPricingPlugin : BaseBTCPayServerPlugin
    {
        public override string Identifier => "BTCPayServer.Plugins.DynamicPricing";
        public override string Name => "Dynamic Pricing";
        public override string Description => "Apply shipping and percentage-based discounts based on order total thresholds.";
        public override string Author => "aeonBTC";
        public override string Version => "1.0.0";

        public override IBTCPayServerPlugin.PluginDependency[] Dependencies { get; } =
        {
            new() { Identifier = nameof(BTCPayServer), Condition = ">=2.0.0" }
        };

        public override PluginResource[] Resources { get; } = new []
        {
            new PluginResource("Source Code", "https://github.com/aeonBTC/BTCPayServerPlugins/Plugins/BTCPayServer.Plugins.DynamicPricing")
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
