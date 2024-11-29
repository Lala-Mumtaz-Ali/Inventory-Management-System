
using Inventory.Data;
using Inventory.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

using System.Data;


namespace Inventory.Repository.Services
{
    public class OrderRepository
    {

        private AppDbContext _context;
        private readonly MySql.Data.MySqlClient.MySqlConnection _connection;
        public OrderRepository(AppDbContext context, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<IEnumerable<Orders>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<IEnumerable<OrderItems>> GetAllOrderItems()
        {
            return await _context.OrderItems.ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            return await _context.Supplier.ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrders>> GetAllPurchaseOrders()
        {
            return await _context.PurchaseOrders.ToListAsync();
        }



        public async Task<IEnumerable<Orders>> GetOrders()
        {
            var orders = await (from order in _context.Orders
                                where order.Is_Dispatched == "Pending"
                                select order).ToListAsync();

            return orders;
        }

        
        public async Task<List<OrderDispatchDTO>> BatchOrderProcessing(IEnumerable<OrderDispatchDTO> orders)
        {
            List<OrderDispatchDTO> failedOrders= new List<OrderDispatchDTO> ();
            foreach(var order in orders)
            {
                try
                {
                    await DispatchOrder(order);
                    
                }

                catch (Exception)
                {
                    
                        failedOrders.Add(new OrderDispatchDTO
                        {
                            Order_Id = order.Order_Id,
                            WareHouse_Id = order.WareHouse_Id
                        });

                }            }

            return failedOrders;
        }
        private async Task DispatchOrder(OrderDispatchDTO order)
        {
           
           
            var warehouseId = order.WareHouse_Id;


            List<OrderItems> orderItems = await _context.OrderItems
                .Where(oi => oi.Order_Id == order.Order_Id)
                .ToListAsync();


            using var connection = _connection;
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            
            try
            {
                foreach (var orderItem in orderItems)
                {
                    try
                    {

                        await ProcessOrderItemAsync(orderItem, warehouseId, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing OrderItem {orderItem.Id}: {ex.Message}");
                        await CallReorderFailedOrderItemsProcedure(order.Order_Id, warehouseId, connection, transaction);
                        throw;
                    }
                }
                
                await _context.Orders
                .Where(e => e.Id == order.Order_Id)
               .ExecuteUpdateAsync(update => update.SetProperty(e => e.Is_Dispatched, "Dispatched"));
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                Console.WriteLine("All order items processed successfully.");

                // Query ot change the status of the  order 
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                throw new Exception( ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
              private async Task ProcessOrderItemAsync(OrderItems item, int warehouseId, MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            try
            {
                
                var parameters = new DynamicParameters();
                parameters.Add("p_order_item_id", item.Id, DbType.Int32);
                parameters.Add("p_warehouse_id", warehouseId, DbType.Int32);

                Console.WriteLine($"Processing Order Item {item.Id} for Product {item.Product_Id} in Warehouse {warehouseId}");

                
                Console.WriteLine($"Calling stored procedure 'MyProcessAndDispatchOrderItem' with parameters: OrderItemId = {item.Id}, WarehouseId = {warehouseId}");

                var result = await connection.ExecuteAsync(
                    "FinalProcessAndDispatchOrderItem",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction
                );

               
                Console.WriteLine($"Stored procedure executed for OrderItem {item.Id}. Affected rows: {result}");

                Console.WriteLine($"Order item {item.Id} processed successfully.");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Message.Contains("Not enough stock"))
                {
                    
                    Console.WriteLine($"Failed To further Process Order. Not enough stock for OrderItem {item.Id} (Product {item.Product_Id}): {ex.Message}");

                    
                    throw new Exception($"Failed To Process Order further due to Insufficient stock ", ex);
                }

                Console.WriteLine($"MySqlError for OrderItem {item.Id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error processing OrderItem {item.Id}: {ex.Message}");
                throw;
            }
        }

        private async Task CallReorderFailedOrderItemsProcedure(int orderId, int warehouseId, MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_order_id", orderId, DbType.Int32);
            parameters.Add("p_warehouse_id", warehouseId, DbType.Int32);

            // Call the stored procedure to reorder the failed items
            Console.WriteLine($"Calling stored procedure 'ReorderFailedOrderItems' for Order {orderId} in Warehouse {warehouseId}");
            await connection.ExecuteAsync(
                "ReorderFailedOrderItems",  // Stored procedure name
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: transaction
            );
            Console.WriteLine($"Reorder procedure called for Order {orderId}.");
        }


        public async Task<List<SuccessfulReStock>> LowStockReOrder()
        {
            List<SuccessfulReStock> result = new List<SuccessfulReStock>();

            string sqlQuery = @"
    SELECT 
        p.Id AS ProductId,
        p.Category_id AS CategoryId,
        p.Current_Price AS Price,
        p.Low_Stock_Threshold AS LowStockThreshold,
        plot.WareHouse_Id AS WarehouseId,
        plot.SumQuantity AS TotalQuantity,
        s.Id AS SupplierId
    FROM 
        product p
    JOIN 
        (SELECT 
            pl.Product_Id,
            pl.WareHouse_Id,  
            SUM(pl.Quantity) AS SumQuantity    
        FROM 
            product_lot pl
        GROUP BY 
            pl.WareHouse_Id, pl.Product_Id) plot
    ON p.Id = plot.Product_Id
    JOIN Supplier s ON p.Category_id = s.Category_id
      LEFT JOIN PurchaseOrders po 
            ON p.Id = po.Product_Id 
            AND plot.WareHouse_Id = po.WareHouse_Id 
            AND po.Is_Received = FALSE
        WHERE 
            plot.SumQuantity < p.Low_Stock_Threshold
            AND po.Id IS NULL
";

            var productDataList = await _context
                .Set<ProductOrderDto>()  
                .FromSqlRaw(sqlQuery)   
                .ToListAsync();   
            if (productDataList.Count == 0)
                throw new Exception("No Product in any warehouse below Low Stock Threshold");

            
            foreach (var product in productDataList)
            {
                
                   
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        { 
                        var purchaseOrder = new PurchaseOrders
                        {

                            Product_Id = product.ProductId,
                            WareHouse_Id = product.WarehouseId,
                            Order_Date = DateTime.Now.Date,
                            Delivery_Date = DateTime.Now.Date.AddDays(5),
                            Quantity = product.LowStockThreshold * 2,
                            TotalPrice = product.LowStockThreshold * 2 * product.Price,
                            Is_Received =false,
                            Supplier_Id = product.SupplierId
                        };
                            _context.PurchaseOrders.Add(purchaseOrder);
                            await _context.SaveChangesAsync();

                            
                            await transaction.CommitAsync();
                            result.Add(new SuccessfulReStock
                            {
                                Product_Id = product.ProductId,
                                WareHouse_Id = product.WarehouseId
                            });

                        }
                        catch (Exception)
                        {   
                            await transaction.RollbackAsync();
                        }
                    }
            }
            return result;
        }
         
        public async Task<List<SuccessfulReStock>> AddMissingProductsToWareHouse()
        {
            string sqlQuery = @"
    WITH ProductWarehouse AS (
        -- Create all possible combinations of products and warehouses
        SELECT 
            p.Id AS ProductId,
            p.Current_Price,
           
            p.Category_id as CategoryId,
            w.Id AS WarehouseId,
            w.PerProductCapacity
            
        FROM 
            product p
        CROSS JOIN 
            warehouse w  -- Assuming there's a warehouse table to get all warehouse combinations
    )
    SELECT 
        pw.ProductId,
        pw.WarehouseId,
        pw.Current_Price AS Price,
        pw.PerProductCapacity AS Capacity,
        pw.CategoryId,
        0 AS TotalQuantity,  -- Set quantity to 0, as this product is missing from the warehouse
        s.Id AS SupplierId
    FROM 
        ProductWarehouse pw
    LEFT JOIN 
        product_lot pl ON pw.ProductId = pl.Product_Id AND pw.WarehouseId = pl.WareHouse_Id
    JOIN 
        supplier s ON pw.CategoryId = s.Category_id
    LEFT JOIN PurchaseOrders po 
            ON pw.ProductId = po.Product_Id 
            AND pw.WareHouseId = po.WareHouse_Id 
            AND po.Is_Received = FALSE
      
    WHERE 
        pl.Product_Id IS NULL AND po.Id is Null";

            
            var productDataList = await _context
                .Set<MissingProductOrderDto>()
                .FromSqlRaw(sqlQuery)
                .ToListAsync();

            

            if (productDataList.Count==0)
            {
                throw new Exception("All Products Available in all Warehouses");
            }

            
            List<SuccessfulReStock> result = new List<SuccessfulReStock>();

            
            foreach (var product in productDataList)
            {
                             using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var purchaseOrder = new PurchaseOrders
                        {

                            Product_Id = product.ProductId,
                            WareHouse_Id = product.WarehouseId,
                            Order_Date = DateTime.Now.Date,
                            Delivery_Date = DateTime.Now.Date.AddDays(5),
                            Quantity = product.Capacity,
                            TotalPrice = (product.Capacity) * product.Price,
                            Is_Received =false,
                            Supplier_Id = product.SupplierId
                        };
                       
                        
                        _context.PurchaseOrders.Add(purchaseOrder);
                        await _context.SaveChangesAsync();
                       
                        await transaction.CommitAsync();
                        result.Add(new SuccessfulReStock
                        {
                            Product_Id = product.ProductId,
                            WareHouse_Id = product.WarehouseId,

                        });
                    }
                    catch (Exception )
                    {
                        await transaction.RollbackAsync();   
                    }
                }
            }
            return result;
        }
          

        public async Task<List<int>> RecieveLots()
        {

            List<int> failedLots = new List<int>();
            var purchaseOrders = await (from po in _context.PurchaseOrders
                                where DateTime.Now > po.Delivery_Date && po.Is_Received==false
                                select po).ToListAsync();

            if (purchaseOrders.Count==0)
            {
                throw new Exception("All Purchase Orders to this date have been Recieved");
            }

            foreach (var order in purchaseOrders)
            {
                var lot = new Product_Lot
                {

                    Product_id = order.Product_Id,
                    WareHouse_Id = order.WareHouse_Id,
                    Manufacturing_date = DateTime.Now.Date,

                    Quantity = order.Quantity,
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.Product_Lot.Add(lot);
                        order.Is_Received = true;
                        _context.PurchaseOrders.Update(order);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }

                    catch (DbUpdateException)
                    {
                        await transaction.RollbackAsync();
                        failedLots.Add(order.Id);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        failedLots.Add(order.Id);
                    }
                }

            }

            return failedLots;
        }
    }
}