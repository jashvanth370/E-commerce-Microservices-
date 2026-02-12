using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder orderInterface,IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No orders fetched in database");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
                return NotFound();

            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailDTO>> GetOrderDetails(int orderId)
        {
            if (orderId < 0) return BadRequest("Invalid data provide");
            var orderDetails = await orderService.GetOrderDetails(orderId);
            return orderDetails is null ? NotFound("No details found for the order") : Ok(orderDetails);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<ActionResult<OrderDTO>> GetProductOrders(int productId)
        {
            if (productId < 0) return BadRequest("Invalid data provide");
            var orders = await orderInterface.GetOrdersAsync(o => o.ProductId == productId);
            return !orders.Any() ? NotFound("No orders found for the product") : Ok(orders);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId < 0) return BadRequest("Invalid data provide");
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            return !orders.Any() ? NotFound("No orders found for the client") : Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            //Check modal state if all details are passed
            if (!ModelState.IsValid)
                return BadRequest("Incomplete order submitted");

            //convert to entity
            var getEntity = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsync(getEntity);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.UpdateAsync(order);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.DeleteAsync(order);
            return response.flag ? Ok(response) : BadRequest(response);
        }
    }
}
