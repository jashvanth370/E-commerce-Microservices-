using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderDate = order.OrderDate,
            PurcheseQuantity = order.PurcheseQuantity,
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            //single 
            if(order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ClientId,
                    order.ProductId,
                    order.PurcheseQuantity,
                    order.OrderDate
                    );

                return (singleOrder, null);
            }

            //List Of order
            if(order is null || orders is not null)
            {
                var _orders = orders!.Select(o => new OrderDTO(
                    o.Id,
                    o.ClientId,
                    o.ProductId,
                    o.PurcheseQuantity,
                    o.OrderDate));

                return (null, _orders);
            }

            return (null, null);
        }
    }
}
