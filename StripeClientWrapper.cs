using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Flexinets.Stripe
{
    public class StripeClientWrapper
    {
        /// <summary>
        /// Create a subscription for customer
        /// </summary>
        /// <param name="stripeCustomerId"></param>
        /// <param name="planId"></param>
        /// <param name="quantity"></param>
        /// <param name="vatExempt"></param>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>
        public static async Task<StripeSubscription> CreateStripeSubscriptionAsync(String stripeCustomerId, String planId, Int32 quantity, Boolean vatExempt, PaymentMethod paymentMethod)
        {
            return await new StripeSubscriptionService().CreateAsync(stripeCustomerId, new StripeSubscriptionCreateOptions
            {
                Items = new List<StripeSubscriptionItemOption> { new StripeSubscriptionItemOption { Quantity = quantity, PlanId = planId } },
                TaxPercent = vatExempt ? 0 : 25m,
                Billing = paymentMethod == PaymentMethod.CreditCard ? StripeBilling.ChargeAutomatically : StripeBilling.SendInvoice,
                DaysUntilDue = paymentMethod == PaymentMethod.CreditCard ? null : (Int32?)30,
                TrialFromPlan = true
            });
        }


        /// <summary>
        /// Updates the quantity of the primary (first and hopefully only) subscription on a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static async Task<StripeSubscription> UpdateSubscriptionQuantityAsync(String customerId, Int32 quantity)
        {
            // todo this assumes our customers only have one subscription and item and will fail miserably if there are more
            var customer = await new StripeCustomerService().GetAsync(customerId);
            var subscription = customer.Subscriptions.Data.SingleOrDefault(o => !o.CanceledAt.HasValue);
            var item = subscription.Items.Data.SingleOrDefault();

            var items = new List<StripeSubscriptionItemUpdateOption> {
                new StripeSubscriptionItemUpdateOption {
                    Id = item.Id,
                    PlanId = item.Plan.Id,
                    Quantity = quantity
                }
            };

            return await new StripeSubscriptionService().UpdateAsync(subscription.Id, new StripeSubscriptionUpdateOptions { Items = items });
        }


        /// <summary>
        /// Create a stripe customer
        /// </summary>
        /// <param name="email"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>
        /// <param name="name"></param>
        /// <param name="postcode"></param>
        /// <param name="streetaddress"></param>
        /// <param name="stripeToken"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static async Task<StripeCustomer> CreateStripeCustomerAsync(String email, String city, String country, String name, String postcode, String streetaddress, String stripeToken = null)
        {
            return await new StripeCustomerService().CreateAsync(new StripeCustomerCreateOptions
            {
                Email = email,
                SourceToken = stripeToken,
                Shipping = new StripeShippingOptions
                {
                    CityOrTown = city,
                    Country = country,
                    Name = name,
                    PostalCode = postcode,
                    Line1 = streetaddress
                }
            });
        }
    }
}