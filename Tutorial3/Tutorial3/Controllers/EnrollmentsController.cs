using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tutorial3.DTOs;
using Tutorial3.Models;
using Tutorial3.Models.Enrollment;
using Tutorial3.Models.Promotion;
using Tutorial3.Services;

namespace Tutorial3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IStudentDbService _service;
        private readonly IConfiguration _configuration;
        public EnrollmentController(IStudentDbService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }
        
        [HttpPost("login", Name = "login")]
        public IActionResult Login(LoginRequestDTO loginRequest)
        {
            if (!Student.IsItPossibleToLogIn(loginRequest))
            {
                return StatusCode(403);
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, loginRequest.Login),
                new Claim(ClaimTypes.Role, Student.GetRole(loginRequest.Login))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var myRefreshToken = Guid.NewGuid();
            var token = new JwtSecurityToken
            (
                issuer: "Valera",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            Student.UpdateRefreshToken(loginRequest.Login, myRefreshToken);
            return Ok(new {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = myRefreshToken
            });
        }
        

        [HttpPost(Name = nameof(EnrollStudent))]
        [Route("enroll")]
        [Authorize(Roles = "employee")]
        [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
        public IActionResult EnrollStudent(AddEnrollment student)
        {
            var result = _service.EnrollStudent(student);
            if (result.Studies != null) return CreatedAtAction(nameof(EnrollStudent), result);
            return BadRequest(result.IdStudent);
        }
        
        [HttpPost(Name = nameof(Promote))]
        [Route("promote")]
        [Authorize(Roles = "employee")]
        public IActionResult Promote(PromoteStudents study)
        {
            var result = _service.Promote(study);
            if (result.Semester != null) return CreatedAtAction(nameof(Promote), result);
            return BadRequest(result.Studies);
        }
        
        [HttpGet("{idStudent}", Name = "StudentGetter")]
        public IActionResult GetStudent(string idStudent)
        {
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT  Semester " +
                              "FROM Student, Enrollment " +
                              "WHERE IdStudent=@idStudent " +
                              "AND Student.IdEnrollment = Enrollment.IdEnrollment"
            };
            SqlParameter parameter = new SqlParameter();
            command.Parameters.AddWithValue("idStudent", idStudent);
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while(dataReader.Read()) 
                return Ok("Student(" + idStudent + ") started his/her studies in " +
                          Int32.Parse(dataReader["Semester"].ToString()) + ".");
            return NotFound("Invalid Input Provided");
        }

        [HttpPost("refreshToken", Name = "refreshToken")]
        public IActionResult RefreshToken(string refreshToken)
        {
            var result = Student.FindStudentByRefreshToken(refreshToken);
            if (result == null)
            {
                return BadRequest("No Student With Such Refresh Token");
            }
           
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result),
                new Claim(ClaimTypes.Role, Student.GetRole(result))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var myRefreshToken = Guid.NewGuid();
            var token = new JwtSecurityToken
            (
                issuer: "Valera",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            Student.UpdateRefreshToken(result, myRefreshToken);
            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = myRefreshToken
            });
        }
    }
}
