using Inventory.Data;
using Inventory.Models;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Data;


namespace Inventory.Repository.Services
{
    public class WTransactionServices : ITransactionsRepository
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly MySqlConnection _connection;
        public WTransactionServices(AppDbContext context, IMapper mapper, MySqlConnection connection)
        {
            _context = context;
            _mapper = mapper;
            _connection = connection;
        }

        public async Task<IEnumerable<LotMovements>> GetAllLotMovements()
        {
            return await _context.LotMovements.ToListAsync();
        }

        public async Task<IEnumerable<WareHouse>> GetWareHouseList()
        {
            IEnumerable<WareHouse> warehouse = await (from w in _context.WareHouse
                                                      select w).ToListAsync();

            return warehouse;
        }

        public async Task<IEnumerable<Product_Lot>> GetWareHouseLots(int Id, int product_id)
        {
            int w_id = await (from w in _context.WareHouse
                              where w.Id == Id
                              select w.Id).SingleAsync();

            if (w_id == 0)
                throw new KeyNotFoundException("No Warehouse available with this ID");

            var productLots = from pl in _context.Product_Lot
                              where pl.WareHouse_Id == w_id && pl.Product_id == product_id
                              select pl;

            return await productLots.ToListAsync();


        }

        public async Task CheckWarehouse(int Lot_id, int W_id)
        {

            var lot = await (from pl in _context.Product_Lot
                             where pl.Id == Lot_id
                             select pl).SingleAsync();


            if (lot == null)
            {
                throw new Exception("Wrong Lot id entered. Could not find a record");
            }

            var warehouse = await (from w in _context.WareHouse
                                   where w.Id == W_id
                                   select w).SingleAsync();

            if (warehouse == null)
            {
                throw new Exception("Wrong Lot id entered. Could not find a record");
            }

            var totalQunatity = await (from pl in _context.Product_Lot
                                       where pl.WareHouse_Id == warehouse.Id
                                       select pl.Quantity).SumAsync();

            if (totalQunatity + lot.Quantity > warehouse.PerProductCapacity)
                throw new Exception("Warehouse capacity for this  Product is already full");


            return;


        }

        public async Task<bool> Transfer(int lot_id, int Dest_id)
        {
            var lot = await _context.Product_Lot.FindAsync(lot_id);


            if (lot == null)

                throw new KeyNotFoundException("No lot can be identified with this id");

            lot.WareHouse_Id = Dest_id;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }


        }

        public async Task<List<LotMovements>> GetLotHistory(int lot_id)
        {

            var lotHistory = new List<LotMovements>();


            var currentMovements = await _context.LotMovements
                .Where(movement => movement.Lot_Id == lot_id && movement.Related_Movement == null)
                .ToListAsync();


            while (currentMovements.Any())
            {

                lotHistory.AddRange(currentMovements);

                var currentIds = currentMovements.Select(m => m.Id).ToList();
                currentMovements = await _context.LotMovements
                    .Where(movement => currentIds.Contains(movement.Related_Movement.Value))
                    .ToListAsync();
            }

            if (!lotHistory.Any())
            {
                throw new KeyNotFoundException("No history available for this lot.");
            }

            return lotHistory;

        }
    }
}





