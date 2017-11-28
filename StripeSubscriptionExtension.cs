using Stripe;
using System;

namespace Flexinets.Stripe
{
    public static class StripeSubscriptionExtension
    {
        /// <summary>
        /// Get currencycode as upper case
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static String GetCurrencyCode(this StripeSubscription subscription)
        {
            return subscription?.StripePlan?.Currency?.ToUpperInvariant();
        }


        /// <summary>
        /// Get price with decimal
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static Decimal? GetPrice(this StripeSubscription subscription)
        {
            return subscription?.StripePlan?.Amount / 100m;
        }


        /// <summary>
        /// Get subtotal without VAT
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static Decimal? GetSubTotal(this StripeSubscription subscription)
        {
            return subscription.GetPrice() * subscription?.Quantity;
        }


        /// <summary>
        /// Get vat sum
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static Decimal? GetVatSum(this StripeSubscription subscription)
        {
            return subscription.GetSubTotal() * ((subscription.TaxPercent ?? 0) / 100m);
        }


        /// <summary>
        /// Get total sum including any VAT
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static Decimal? GetTotalSum(this StripeSubscription subscription)
        {
            return subscription.GetSubTotal() + subscription.GetVatSum();
        }


        /// <summary>
        /// Get PaymentMethod
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static PaymentMethod GetPaymentMethod(this StripeSubscription subscription)
        {
            // todo make sure this actually works...
            return subscription.Billing.Value == StripeBilling.ChargeAutomatically ? PaymentMethod.CreditCard : PaymentMethod.Invoice;
        }
    }
}
