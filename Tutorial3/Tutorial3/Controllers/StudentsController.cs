using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tutorial3.Models;
using Tutorial3.Models.Enrollment;

namespace Tutorial3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var students = new List<Student>();
            using(var sqlConnection = new SqlConnection(@"Server=db-mssql16.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandText = "SELECT FirstName,LastName,BirthDate,Name,Semester " +
                                          "FROM Enrollment, Studies, Student " +
                                          "WHERE Studies.IdStudy = Enrollment.IdStudy " +
                                          "AND Enrollment.IdEnrollment = Student.IdEnrollment";
                    sqlConnection.Open();
                    var response = command.ExecuteReader();
                    while (response.Read())
                    {
                        var st = new Student
                        {
                            FirstName = response["FirstName"].ToString(),
                            LastName = response["LastName"].ToString(),
                            Studies = response["Name"].ToString(),
                            BirthDate = DateTime.Parse(response["BirthDate"].ToString()),
                            Semester = int.Parse(response["Semester"].ToString())
                        };
                        students.Add(st);
                    }
                }
            }
            return Ok(students);
        }
    }
}
