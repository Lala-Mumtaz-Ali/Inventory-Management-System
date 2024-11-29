using System;
using System.Collections.Generic;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Mysqlx.Crud;


namespace Inventory.Repository
{
    public interface ITransactionsRepository
    {
        Task<IEnumerable<WareHouse>> GetWareHouseList();

        //Task<IEnumerable<OverStockProducts>> CheckWareHouseCapacity(int w_id);

        Task<IEnumerable<Product_Lot>> GetWareHouseLots(int Id, int product_id);
        Task CheckWarehouse(int Lot_id, int W_id);

        Task<bool> Transfer(int lot_id, int Dest_id);

        Task<List<LotMovements>> GetLotHistory(int lot_id);
        Task<IEnumerable<LotMovements>> GetAllLotMovements();

        //Task<IEnumerable<AttributeValue>> GetAttributeValueList(string category);
        //IEnumerable<WareHouse> GetPotential();

        //IEnumerable<WareHouse> Get_Potential_Movements();
        //Task<bool> PlaceLotOrder(List<LotOrderDTO> lotorders);
    }
}
