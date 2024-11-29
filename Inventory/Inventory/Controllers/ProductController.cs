using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Repository;
using Inventory.Repository.Services;
using Inventory.Models;
using MySqlX.XDevAPI.Common;
namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly ProductRepository _product_repo;

        public ProductController(ProductRepository product_repo)
        {
            _product_repo = product_repo;
        }



        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _product_repo.GetAllProducts();
                if (products.Any())
                {
                    return Ok(products);
                }
                else
                {
                    return NotFound(new { message = "No products found in the database." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the products.", details = ex.Message });
            }
        }


        [HttpGet("GetProduct/{Id}")]
        public async Task<IActionResult> GetProduct(int Id)
        {
            try
            {
                var product = await _product_repo.GetProductInfo(Id);
                return Ok(product);
            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }

        }


        [HttpGet("ProductAnalysis/{Id}")]
        public async Task<IActionResult> ProductAnalysis(int Id)
        {
            try
            {
                var product = await _product_repo.GetAnalysis(Id);
                return Ok(product);
            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("Get_Main_Filtered/{category}")]

        public async Task<IActionResult> Get_Main_Filtered_Products(string category)
        {
            if (String.IsNullOrEmpty(category))  // sice cateogry must be present only it is checked. 
            {
                return BadRequest();
            }
            try
            {
                var products = await _product_repo.Get_Main_Filtered_Products(category);
                if (products != null)
                    return Ok(products);

                else
                    return NotFound(new { message = "No product found with this Catergory" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }



        }

        [HttpPost("Create")]
        [Consumes("application/json")]

        public async Task<IActionResult> CreateProduct([FromBody] ProductCreationDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                int Id = await _product_repo.CreateProduct(product);
                return Ok(new { message = $"Product Created Successfuly with Id : {Id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the product: " + ex.Message);
            }
        }


        [HttpPut("Edit/{Id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Modify_Product(int Id, [FromBody] ProductUpdateDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _product_repo.Update(Id, product);
                return Ok(new { message = "Product Updated" });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpDelete("Delete/{product_id}")]

        public async Task<IActionResult> Delete(int product_id)
        {
            try
            {
                await _product_repo.delete(product_id);
                return Ok();
            }

            catch (KeyNotFoundException ex)
            {

                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllProductLots")]
        public async Task<IActionResult> GetAllProductLots()
        {
            try
            {
                var productLots = await _product_repo.GetAllProductLots();
                if (productLots.Any())
                {
                    return Ok(productLots);
                }
                else
                {
                    return NotFound(new { message = "No product lots found in the database." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the product lots.", details = ex.Message });
            }
        }


    }
}