using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Repository;
using Inventory.Repository.Services;
using Inventory.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ProductRepository _product_repo;
        private readonly ITransactionsRepository _transactions_repo;
        private readonly OrderRepository _order_repo;
        public OrderController(ProductRepository product_repo, ITransactionsRepository transactions_repo, OrderRepository order_repo)
        {
            _product_repo = product_repo;
            _transactions_repo = transactions_repo;
            _order_repo = order_repo;
        }

        [HttpGet("RetrieveProducts/{category}")]
        public async Task<IActionResult> Place_Order(string category)
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

        [HttpGet("PendingCustomerOrders")]
        public async Task<IActionResult> GetCustomerOrders()
        {
            var orders = await _order_repo.GetOrders();

            if (orders == null)
                return NotFound(new { message = "No Pending Orders" });

            return Ok(orders);
        }

        [HttpPost("Dispatch")]
        public async Task<IActionResult> DispatchOrder([FromBody] IEnumerable<OrderDispatchDTO> orders)
        {
            if (orders == null)
                return BadRequest();

            var failedOrders = await _order_repo.BatchOrderProcessing(orders);

            if (failedOrders.Count == 0)
                return Ok(new { message = "All Orders Dispatched" });

            return Ok(failedOrders);
        }

        [HttpGet("StockAllCurrentProducts")]
        public async Task<IActionResult> ReStockCurrentProducts()
        {
            try
            {
                var ReStockOrders = await _order_repo.LowStockReOrder();
                return Ok(ReStockOrders);
            }
            catch (Exception ex)
            {

                return NotFound(ex.Message);
            }
        }

        [HttpGet("StockAllMissingProducts")]
        public async Task<IActionResult> ReStockUnavailableProducts()
        {
            try
            {
                var NewStockOrders = await _order_repo.AddMissingProductsToWareHouse();
                return Ok(NewStockOrders);
            }
            catch (Exception ex)
            {

                return NotFound(ex.Message);
            }

        }

        [HttpPost("RecievePurchaseOrders")]
        public async Task<IActionResult> AddLots()
        {
            try
            {
                List<int> failedLots = await _order_repo.RecieveLots();

                if (failedLots.Count != 0)
                    return Ok(failedLots);

                return Ok(new { message = "All pending orders recieved" });
            }

            catch (Exception ex)
            {
                return Ok(new { message = ex.Message });
            }
        }


        // New method to get all orders
        [HttpGet("AllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _order_repo.GetOrders();

            if (orders == null || !orders.Any())
                return NotFound(new { message = "No orders found." });

            return Ok(orders);
        }

        // New method to get all order items
        [HttpGet("AllOrderItems")]
        public async Task<IActionResult> GetAllOrderItems()
        {
            var orderItems = await _order_repo.GetAllOrderItems();

            if (orderItems == null || !orderItems.Any())
                return NotFound(new { message = "No order items found." });

            return Ok(orderItems);
        }

        // New method to get all suppliers
        [HttpGet("AllSuppliers")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _order_repo.GetAllSuppliers();

            if (suppliers == null || !suppliers.Any())
                return NotFound(new { message = "No suppliers found." });

            return Ok(suppliers);
        }

        // New method to get all purchase orders
        [HttpGet("AllPurchaseOrders")]
        public async Task<IActionResult> GetAllPurchaseOrders()
        {
            var purchaseOrders = await _order_repo.GetAllPurchaseOrders();

            if (purchaseOrders == null || !purchaseOrders.Any())
                return NotFound(new { message = "No purchase orders found." });

            return Ok(purchaseOrders);
        }

        
    }
}
