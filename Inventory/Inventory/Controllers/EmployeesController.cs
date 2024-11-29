using Inventory.Models;
using Inventory.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private IEmployeeRepository employeeRepository;

    public EmployeesController(IEmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] Employees Employee)
    {
        if (ModelState.IsValid)
        {
            await employeeRepository.Create(Employee);
            return CreatedAtAction(nameof(Get_By_Username), new { user_name = Employee.user_name }, Employee);
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Get_By_Username/{user_name}")]
    public async Task<IActionResult> Get_By_Username(string user_name)
    {
        if (String.IsNullOrEmpty(user_name))
        {
            return BadRequest("Username is empty");
        }
        else
        {
            Employees? emp = await employeeRepository.Get_By_Username(user_name);
            if (emp == null)
                return NotFound();
            else
                return Ok(emp);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{user}")]
    public async Task<IActionResult> Delete(string user)
    {
        if (String.IsNullOrEmpty(user))
        {
            return BadRequest("Username is empty");
        }

        try
        {
            await employeeRepository.Delete(user);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch]
    [Consumes("application/json")]
    public async Task<IActionResult> Update([FromBody] EmployeeDTO Employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await employeeRepository.Update(Employee);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var Employees = await employeeRepository.GetAllEmployees();
        return Ok(Employees);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("condition")]
    public async Task<IActionResult> GetByCondition([FromQuery] String? role)
    {
        if (!String.IsNullOrEmpty(role))
        {
            IEnumerable<Employees>? Employees = await employeeRepository.GetByCondition(role);
            return Ok(Employees);
        }

        return NotFound(role);
    }
}
