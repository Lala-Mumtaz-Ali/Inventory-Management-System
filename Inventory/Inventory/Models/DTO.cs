using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Security.Certificates;

namespace Inventory.Models
{
    public class EmployeeDTO
    {
        public string user_name { get; set; } = "None";
        public string? password { get; set; }
        public string? email { get; set; }
        public string? contact_no { get; set; }
        public string? role { get; set; }
        public string? name { get; set; }

    }

    public class ProductDashDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? Category { get; set; }
        //public string? Generic { get; set; }

        public required int current_price { get; set; }

        public bool? is_active { get; set; }

        public int? quantity { get; set; }

    }

    public class Product_Attributes_DTO
    {

        public int id { get; set; }
        public string? name { get; set; }
        public int price { get; set; }

        public int quantity { get; set; }

    }


    public class ProductUpdateDTO
    {


        public string? Name { get; set; }

        public int? Current_Price { get; set; }

        public string? Description { get; set; }

        public int? Low_Stock_Threshold { get; set; }
    }


    public class ProductAnalysisDTO
    {

        public int product_id { get; set; }

        public required string name { get; set; }

        public int price { get; set; }

        public int att_id1 { get; set; }

        public int att_id2 { get; set; }

        public int? att_id3 { get; set; }

        public required string value1 { get; set; }

        public required string value2 { get; set; }

        public string? value3 { get; set; }

    }

    public class ProductCreationDTO
    {

        public required string Name { get; set; }

        public int Category_id { get; set; }
        public string? Description { get; set; }
        public int Current_Price { get; set; }
        public int Low_Stock_Threshold { get; set; }
    }

    public class ProductWarehouse
    {

        public int Warehouse_id { get; set; }

        public required string Warehouse_name { get; set; }

        public int NumberOfLots { get; set; }
        public int TotalQuantity { get; set; }

    }


    public class AnalysisDTO
    {

        public required string Name { get; set; }
        public int current_price { get; set; }

        public string? description { get; set; }

        public List<ProductWarehouse>? Warehouse_List { get; set; }


    }

    public class OverStockProducts
    {

        public int Product_id { get; set; }

        public int TotalQuantity { get; set; }

        public int ExceedingQuantity { get; set; }

        public IEnumerable<PotentialWareHouseDTO>? PotentialWarehouses { get; set; }
    }

    public class PotentialWareHouseDTO
    {
        public int Warehouse_id { get; set; }
        public required string Warehouse_name { get; set; }


    }


    public class AttributeValue
    {


        public required string attribute_name { get; set; }

        public required List<string> values { get; set; }
    }




    public class LotOrderDTO
    {
        public int Product_id { get; set; }
        public int WareHouse_Id { get; set; }


        public int Attribute_id1 { get; set; }  // The ID for the first attribute (e.g., size)
        public int Attribute_id2 { get; set; }  // The ID for the second attribute (e.g., color)
        public int Attribute_id3 { get; set; }  // The ID for the third attribute (optional, for other attributes)

        // These represent the values for each attribute in the lot
        public required string Value1 { get; set; }  // The value for the first attribute (e.g., size=large)
        public required string Value2 { get; set; }  // The value for the second attribute (e.g., color=red)
        public required string Value3 { get; set; }  // The value for the third attribute (e.g., weight=light)

        public int Quantity { get; set; }
    }

    public class UrgentOrdersDTO
    {

        public int Product_Id { get; set; }

        public string? Product_Name { get; set; }

        public int WareHouse_Id { get; set; }
        public string? WareHouse_Name { get; set; }
        public int CurrentQuantity { get; set; }
        public int MinOrderQuantity { get; set; }

    }

    public class ProductOrderDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int Price { get; set; }

        public int LowStockThreshold { get; set; }

        public int WarehouseId { get; set; }
        public int TotalQuantity { get; set; }
        public int SupplierId { get; set; }
    }
    public class MissingProductOrderDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int Price { get; set; }
        public int Capacity { get; set; }
        public int WarehouseId { get; set; }
        public int TotalQuantity { get; set; }
        public int SupplierId { get; set; }
    }
    public class SuccessfulReStock
    {
        public int Product_Id { get; set; }
        public int WareHouse_Id { get; set; }

    }

    public class OrderDispatchDTO
    {
        public int Order_Id { get; set; }

        public int WareHouse_Id { get; set; }
    }
}
