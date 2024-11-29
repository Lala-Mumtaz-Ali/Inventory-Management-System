using Inventory.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
namespace Inventory.Repository.Services
{
    public class LoginDTO
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class AuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext _context;
        private readonly string _secretKey;
        private readonly string _issuer;

        public AuthService(IEmployeeRepository employeeRepository, AppDbContext context, string secretKey, string issuer)
        {
            _employeeRepository = employeeRepository;
            _context = context;
            _secretKey = secretKey;
            _issuer = issuer;
        }

        public async Task<Employees> authetication(string username, string password)
        {
            Employees? emp = await _employeeRepository.Authenticate(username, password);
            return emp;
        }

        public string Generate_Token(Employees emp)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, emp.user_name),
                new Claim(ClaimTypes.Role, emp.role)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
             issuer: _issuer,
             audience: _issuer,
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(3000), // Use UTC
             signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token); ;

        }
    }

}