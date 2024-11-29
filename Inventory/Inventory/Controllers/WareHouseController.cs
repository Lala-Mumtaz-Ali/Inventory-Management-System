using Inventory.Repository;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {


        private readonly ITransactionsRepository _transaction_repo;

        public WareHouseController(ITransactionsRepository transaction_repo)
        {
            _transaction_repo = transaction_repo;
        }


        [HttpGet("WareHouseList")]

        public async Task<IActionResult> GetWareHouses()
        {

            var warehouses = await _transaction_repo.GetWareHouseList();
            if (warehouses == null)
                return NotFound();

            return Ok(warehouses);
        }

        [HttpGet("GetWarehouseLots/{Id}")]
        public async Task<IActionResult> WareHouseLots(int Id, [FromQuery] int product_id)
        {
            try
            {
                var lots = await _transaction_repo.GetWareHouseLots(Id, product_id);

                if (lots == null)
                    return NotFound(new { message = "No lots in this warehouse" });

                return Ok(lots);
            }

            catch (KeyNotFoundException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetTransferableWarehouse/{lot_id}/{warehouse_id}")]
        public async Task<IActionResult> GetWareHouses(int lot_id, int warehouse_id)
        {
            try
            {
                await _transaction_repo.CheckWarehouse(lot_id, warehouse_id);


                return Ok(new { message = "WareHouse Available For transfer" });
            }

            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("TransferLot/{lot_id}/{Dest_id}")]
        public async Task<IActionResult> Transfer_Lot(int lot_id, int Dest_id)
        {
            if (lot_id == 0 || Dest_id == 0)
                return BadRequest();



            bool result = await _transaction_repo.Transfer(lot_id, Dest_id);

            if (result)
                return Ok(new { message = "Lot Transfered" });

            else
                return NotFound(new { message = "Transaction Failed" });





        }

        [HttpGet("TrackLot/{lot_id}")]
        public async Task<IActionResult> TrackLots(int lot_id)
        {
            try
            {
                var LotHistory = await _transaction_repo.GetLotHistory(lot_id);

                return Ok(LotHistory);
            }

            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



        // New method to get all lot movements
        [HttpGet("AllLotMovements")]
        public async Task<IActionResult> GetAllLotMovements()
        {
            var lotMovements = await _transaction_repo.GetAllLotMovements();

            if (lotMovements == null || !lotMovements.Any())
                return NotFound(new { message = "No lot movements found." });

            return Ok(lotMovements);
        }


    }
}
