using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products found in database.");
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No products found.");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound($"No product with id {id} found in database.");
            var (single, _) = ProductConversion.FromEntity(product, null!);
            return single is not null ? Ok(single) : NotFound($"No product with id {id} found.");
        }
        [HttpPost]
        public async Task<ActionResult> CreateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productEntity = ProductConversion.ToEntity(productDTO);
            var responce = await productInterface.CreateAsync(productEntity);
            return responce.flag ? Ok(responce) : BadRequest(responce.message);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productEntity = ProductConversion.ToEntity(productDTO);
            var responce = await productInterface.UpdateAsync(productEntity);
            return responce.flag ? Ok(responce) : BadRequest(responce.message);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(ProductDTO productDTO)
        {
            var product = ProductConversion.ToEntity(productDTO);
            if (product is null)
                return NotFound($"No product found in database.");
            var responce = await productInterface.DeleteAsync(product);
            return responce.flag ? Ok(responce) : BadRequest(responce.message);
        }
    }
}
