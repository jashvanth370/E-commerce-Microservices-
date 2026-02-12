using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline ) : IOrderService
    {
        //get product
        public async Task<ProductDTO> GetProduct(int productId)
        {
            //Call Product Api using HttpClient
            //Redirect this call to the API GAteway since product api is not response to outsiders
            var getProduct = await httpClient.GetAsync($"http://localhost:5001/api/Product/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        //get user
        public async Task<AppUserDTO> GetUser(int userId)
        {
            //Call Product Api using HttpClient
            //Redirect this call to the API GAteway since product api is not response to outsiders
            var getUser = await httpClient.GetAsync($"http://localhost:5000/api/Authentication/getUser/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }
        public async Task<OrderDetailDTO> GetOrderDetails(int orderId)
        {
            //Prepare order
            var order = await orderInterface.FindByIdAsync(orderId);
            if(order is null || order!.Id <= 0)
            {
                return null!;
            }

            //Get retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            var productDTO = await retryPipeline.ExecuteAsync(
                async token => await GetProduct(order.ProductId));

            if (productDTO is null)
                throw new Exception("Product service failed or product not found.");

            var appUserDTO = await retryPipeline.ExecuteAsync(
                async token => await GetUser(order.ClientId));

            if (appUserDTO is null)
                throw new Exception("User service failed or user not found.");


            //Populate order details
            return new OrderDetailDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurcheseQuantity,
                productDTO.Price,
                productDTO.Price * order.PurcheseQuantity,
                order.OrderDate
                );

        }

        //get orders by clientId
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            //Get all client orders 
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if(!orders.Any()) return null!;

            // Convert from entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
            
        }
    }
}
