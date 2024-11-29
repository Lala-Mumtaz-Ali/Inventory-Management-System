

using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Drawing;
using System.Numerics;

namespace Inventory.Models

{
    public class Product
    {

        public int Id { get; set; }

        public required string Name { get; set; }

        public int Category_id { get; set; }

        public int Current_Price { get; set; } = 0;

        public string? Description { get; set; }

        public int Low_Stock_Threshold { get; set; }
    }


    public class Category
    {
        public int Id { get; set; }


        public string? Code { get; set; }



        public required string Name { get; set; }
    }

    public class Product_Lot
    {

        public int Id { get; set; }

        public DateTime Manufacturing_date { get; set; }



        public int WareHouse_Id { get; set; }



        public int Product_id { get; set; }

        public int Quantity { get; set; }


    }

}

