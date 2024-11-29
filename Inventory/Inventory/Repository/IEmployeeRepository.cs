using System;
using System.Collections.Generic;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Mysqlx.Crud;

namespace Inventory.Repository
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employees> Get_Employees();
        Task Create(Employees employees);
        Task Update(EmployeeDTO Employee);
        Task Delete(string user_name);
        Task<Employees?> Get_By_Username(string username);
        Task<IEnumerable<Employees>> GetAllEmployees();
        Task<IEnumerable<Employees>> GetByCondition(string? role);
        Task<Employees> Authenticate(string username, string password);
    }
}
