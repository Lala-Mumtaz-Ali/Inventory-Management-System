using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using AutoMapper;

namespace Inventory.Repository.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private AppDbContext _context;
        private readonly IMapper _mapper;
        public EmployeeRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<Employees> Get_Employees()
        {
            IEnumerable<Employees> Employees = _context.Employees.ToList();
            return Employees;

        }

        public async Task Create(Employees employees)
        {
            //employees.password = BCrypt.Net.BCrypt.HashPassword(employees.password);
            await _context.Employees.AddAsync(employees);
            _context.SaveChanges();

            return;

        }

        public async Task<Employees?> Get_By_Username(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username cannot be null or empty");

            return await _context.Employees.FirstOrDefaultAsync(e => e.user_name == userName);
        }


        public async Task Delete(string user_name)
        {
            Employees? emp = await _context.Employees.FirstOrDefaultAsync(e => e.user_name == user_name);

            if (emp != null)
            {
                _context.Employees.Remove(emp);
                _context.SaveChanges();

            }

            else
            {
                throw new Exception("Employee Not found with this username");
            }
        }

        public async Task Update(EmployeeDTO Employee)
        {
            Employees? existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.user_name == Employee.user_name);

            if (existingEmployee != null)
            {
                _mapper.Map(Employee, existingEmployee);
                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Employee not found with this username");
            }
        }



        public async Task<IEnumerable<Employees>> GetAllEmployees()
        {
            return await (from emp in _context.Employees
                          select emp).ToListAsync();

        }

        public async Task<IEnumerable<Employees>> GetByCondition(string? role)
        {
            return await (from emp in _context.Employees
                          where emp.role == role
                          select emp).ToListAsync();
        }



        public async Task<Employees> Authenticate(string user_name, string password)
        {
            Employees? emp = await _context.Employees.FirstOrDefaultAsync(e => e.user_name == user_name && e.password == password);
            if (emp != null)    
            {
                return emp;
            }

            throw new KeyNotFoundException("No employee Found");
        }


    }

}
