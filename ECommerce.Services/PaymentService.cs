using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications.OrderSpecifications;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketDTOs;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Forwarding;
using Product = ECommerce.Domain.Entities.ProductModule.Product;

namespace ECommerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PaymentService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper
        )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Result<BasketDTO>> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            var skey = _configuration["Stripe:Skey"];
            if (string.IsNullOrEmpty(skey))
                return Error.Faliure("Failed to obtain Secret Key Value");

            StripeConfiguration.ApiKey = skey;

            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
                return Error.NotFound("Basket not found");

            if (basket.DeliveryMethodId == null)
                return Error.Validation("Delivery method is not selected in the basket");

            var method = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(basket.DeliveryMethodId.Value);

            if (method == null)
                return Error.NotFound("Delivery method not found");

            basket.ShippingPrice = method.Price;

            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.ProductId);
                if (product == null)
                    return Error.NotFound("ProductItem.NotFound");

                item.Price = product.Price;
                item.ProductName = product.Name;
                item.PictureUrl = product.PictureUrl;
            }

            long amount = (long)(basket.Items.Sum(i => i.Quantity * i.Price) * 100);

            var stripeService = new PaymentIntentService();

            if (string.IsNullOrEmpty(basket.PaymentIntentID))
            {
                // Create new PaymentIntent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                var paymentIntent = await stripeService.CreateAsync(options);

                basket.PaymentIntentID = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update existing PaymentIntent
                var options = new PaymentIntentUpdateOptions { Amount = amount };
                await stripeService.UpdateAsync(basket.PaymentIntentID, options);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);

            return _mapper.Map<BasketDTO>(basket);
        }

        public async Task UpdateOrderPaymentStatus(string request, string stripeSignature)
        {
            var endPointSecret = _configuration["Stripe:EndpointSecret"];
            var stripeEvent = EventUtility.ConstructEvent(request, stripeSignature, endPointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var order = await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetByIdAsync(new OrderWithPaymentIntentSpecifications(paymentIntent.Id));

            if (order == null) return;

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                order.Status = OrderStatus.Paid;
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                order.Status = OrderStatus.Failed;
            }
            else
            {
                // Optionally log unhandled event
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                return;
            }

            _unitOfWork.GetRepository<Order, Guid>().Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
