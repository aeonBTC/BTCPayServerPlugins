# Dynamic Pricing Plugin for BTCPay Server

This plugin allows store owners to offer dynamic shipping costs and percentage discounts based on the total price of orders.

## Features

- **Dynamic Shipping Costs**: Configure shipping costs that decrease as order total increases
- **Automatic Discounts**: Apply percentage discounts automatically based on order total thresholds
- **Easy Configuration**: Simple interface to set up and manage thresholds for both features

## How It Works

### Shipping Discounts

Configure shipping costs that change based on order total thresholds. For example:
- Orders under $50: Standard shipping cost
- Orders $50-$99.99: Reduced shipping cost
- Orders $100+: Free shipping

### Order Discounts

Configure percentage discounts that apply based on order total thresholds. For example:
- Orders $50-$99.99: 5% discount
- Orders $100-$199.99: 10% discount
- Orders $200+: 15% discount

## Setup Instructions

1. Install the plugin via BTCPay Server's plugin management page
2. Navigate to your store settings
3. Select "Dynamic Pricing" from the menu
4. Configure your discount and shipping thresholds
5. Enable or disable features as needed

## Integration with BTCPay Server

When creating invoices, the plugin automatically:
1. Examines the order total
2. Applies the appropriate shipping discount if enabled
3. Applies the appropriate percentage discount if enabled
4. Updates the invoice amount accordingly

Customers will see the applied discounts during checkout.

## Requirements

- BTCPay Server v2.0.0 or later

## License

This plugin is available as open source under the terms of the MIT License. 