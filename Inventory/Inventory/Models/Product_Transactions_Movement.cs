using System.Numerics;
namespace Inventory.Models
{
    public class WareHouse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int PerProductCapacity { get; set; }
        public required string Location { get; set; }
    }

    public class LotMovements
    {

        public int Id { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public int Product_Id { get; set; }

        public int Lot_Id { get; set; }
        public int? Related_Movement { get; set; }
        public DateTime Transaction_date { get; set; }

    }

    public class PurchaseOrders
    {
        public int Id { get; set; }
        public int Product_Id { get; set; }
        public int WareHouse_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Delivery_Date { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }

        public bool Is_Received { get; set; }
        public int Supplier_Id { get; set; }

    }


    public class Supplier
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Contact { get; set; }

        public int Category_id { get; set; }


    }

    public class Orders
    {

        public int Id { get; set; }

        public int WareHouse_Id { get; set; }
        public int TotalQuantity { get; set; }

        public int TotalAmount { get; set; }

        public required string Is_Dispatched { get; set; } = "Pending";

        public required string Shipping_Address { get; set; }

    }

    public class OrderItems

    {
        public int Id { get; set; }
        public int Order_Id { get; set; }
        public int Product_Id { get; set; }

        public int Quantity { get; set; }
        public int UnitPrice { get; set; }

        public int Sub_Total { get; set; }
    }
}
