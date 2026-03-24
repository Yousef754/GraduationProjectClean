using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IMapper mapper,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork
        )
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string email)
        {
            // 1- Map shipping address
            var orderAddress = _mapper.Map<OrderAddress>(orderDTO.ShipToAddress);

            // 2- Retrieve basket
            var basket = await _basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (basket is null)
                return Error.NotFound(
                    "Basket.NotFound",
                    $"The basket with Id:{orderDTO.BasketId} is not found"
                );

            if (basket.PaymentIntentID is null)
                return Error.Validation("PaymentIntent.NotFound");

            // 3- Create order items
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.ProductId);
                if (product is null)
                    return Error.NotFound(
                        "Product.NotFound",
                        $"The product with Id:{item.ProductId} is not found"
                    );

                orderItems.Add(CreateOrderItem(item, product));
            }

            // 4- Retrieve delivery method
            var deliveryMethod = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(orderDTO.DeliveryMethodId);
            if (deliveryMethod is null)
                return Error.NotFound(
                    "DeliveryMethod.NotFound",
                    $"The Delivery Method with Id:{orderDTO.DeliveryMethodId} is not found"
                );

            // 5- Calculate subtotal
            var subTotal = orderItems.Sum(x => x.Price * x.Quantity);

            // 6- Check if order exists with this PaymentIntentId
            var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentID);
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var existingOrder = await orderRepo.GetByIdAsync(orderSpec);
            if (existingOrder is not null)
                orderRepo.Delete(existingOrder);

            // 7- Create new order
            var order = new Order()
            {
                UserEmail = email,
                Address = orderAddress,
                DeliveryMethod = deliveryMethod,
                PaymentIntentId = basket.PaymentIntentID!,
                SubTotal = subTotal,
                Items = orderItems,
            };

            await orderRepo.AddAsync(order);

            bool result = await _unitOfWork.SaveChangesAsync() > 0;
            if (!result)
                return Error.Faliure("Order.Faliure", "There was a problem while creating the order");

            // 8- Return mapped DTO
            var orderToReturn = _mapper.Map<OrderToReturnDTO>(order);
            return Result<OrderToReturnDTO>.Ok(orderToReturn);
        }

        public async Task<Result<IEnumerable<DeliveryMethodDTO>>> GetAllDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            if (!deliveryMethods.Any())
                return Error.NotFound("DeliveryMethods.NotFound", "No delivery methods found");

            var data = _mapper.Map<IEnumerable<DeliveryMethodDTO>>(deliveryMethods);
            return Result<IEnumerable<DeliveryMethodDTO>>.Ok(data);
        }

        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email)
        {
            var orderSpec = new OrderSpecification(email);
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(orderSpec);

            if (!orders.Any())
                return Error.NotFound(
                    "Orders.NotFound",
                    $"No orders found for the user with email:{email}"
                );

            var data = _mapper.Map<IEnumerable<OrderToReturnDTO>>(orders);
            return Result<IEnumerable<OrderToReturnDTO>>.Ok(data);
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid Id, string email)
        {
            var orderSpec = new OrderSpecification(Id, email);
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderSpec);

            if (order is null)
                return Error.NotFound(
                    "Order.NotFound",
                    $"No order found with Id:{Id} for the user with email:{email}"
                );

            var data = _mapper.Map<OrderToReturnDTO>(order);
            return Result<OrderToReturnDTO>.Ok(data);
        }

        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            return new OrderItem()
            {
                Product = new ProductItemOrdered()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl,
                },
                Price = product.Price,
                Quantity = item.Quantity,
            };
        }
    }
}
