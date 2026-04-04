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
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(CreateOrderDto dto, string userid)
        {
            // 1- Get Basket by UserId
            //var basket = await _basketRepository.GetBasketAsync(email);

            var basket = await _basketRepository.GetBasketAsync(userid);

            if (basket is null || !basket.Items.Any())
                return Error.NotFound("Basket.NotFound", "Basket is empty");

            // 2- Create Order Items + Reduce Stock
            List<OrderItem> orderItems = new();

            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork
                    .GetRepository<Product, int>()
                    .GetByIdAsync(item.ProductId);

                if (product is null)
                    return Error.NotFound("Product.NotFound", $"Product {item.ProductId} not found");

                // تحقق من الكمية
                if (product.Quantity < item.Quantity)
                    return Error.Validation("Product.OutOfStock", $"{product.Name} is out of stock");

                // تقليل الكمية
                product.Quantity -= item.Quantity;

                orderItems.Add(CreateOrderItem(item, product));
            }

            // 3- Get Delivery Method
            var deliveryMethod = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(dto.DeliveryMethodId);

            if (deliveryMethod is null)
                return Error.NotFound("DeliveryMethod.NotFound", "Invalid delivery method");

            // 4- Calculate Subtotal
            var subTotal = orderItems.Sum(x => x.Price * x.Quantity);

            // 5- Create Order
            var order = new Order
            {
                UserId = userid,
                Address = dto.Address,
                Phone = dto.Phone,
                PaymentMethod = dto.PaymentMethod,
                DeliveryMethodId =dto.DeliveryMethodId,
                SubTotal = subTotal,
                Items = orderItems,
                //Status = dto.PaymentMethod == PaymentMethod.Cash
                //            ? OrderStatus.Pending
                //            : OrderStatus.Paid
            };

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            await orderRepo.AddAsync(order);

            // 6- Save Changes
            //var result = await _unitOfWork.SaveChangesAsync() > 0;

            //if (!result)
            //    return Error.Faliure("Order.Failure", "Failed to create order");

            try
            {
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (!result)
                    return Error.Faliure("Order.Failure", "Failed to create order");
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message;
                return Error.Faliure("Order.Failure", inner ?? ex.Message);
            }

            // 7- Delete Basket after success
            await _basketRepository.DeleteBasketAsync(userid);

            // 8- Return DTO
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

            var orders = await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetAllAsync(orderSpec);

            if (!orders.Any())
                return Error.NotFound("Orders.NotFound", $"No orders found for user {email}");

            var data = _mapper.Map<IEnumerable<OrderToReturnDTO>>(orders);
            return Result<IEnumerable<OrderToReturnDTO>>.Ok(data);
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string email)
        {
            var orderSpec = new OrderSpecification(id, email);

            var order = await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetByIdAsync(orderSpec);

            if (order is null)
                return Error.NotFound("Order.NotFound", $"Order {id} not found");

            var data = _mapper.Map<OrderToReturnDTO>(order);
            return Result<OrderToReturnDTO>.Ok(data);
        }

        // 🔥 Helper Method
        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            return new OrderItem
            {
                Product = new ProductItemOrdered
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl
                },
                Price = product.Price,
                Quantity = item.Quantity
            };
        }
    }
}
