using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tutorial3.DTOs;
using Tutorial3.Encryption;
using Tutorial3.Models.Promotion;

namespace Tutorial3.Models
{
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Studies { get; set; }
        public int Semester { get; set; }
        
        public string Role { get; set; }
        
        
        [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
        public static Boolean IsItPossibleToLogIn(LoginRequestDTO loginRequest)
        {
            var login = loginRequest.Login;
            var password = loginRequest.Password;
            var isNumeric = int.TryParse(login, out _);


            if ( !isNumeric || string.IsNullOrEmpty(password))
                return false;

            using (var sqlConnection = new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                sqlConnection.Open();
                using (var mainCommand = new SqlCommand())
                {
                    mainCommand.Connection = sqlConnection;
                    mainCommand.CommandText = "SELECT IdStudent, Password, Salt " + 
                                              "From Student " +
                                              "Where IdStudent=@Index";;
                    mainCommand.Parameters.AddWithValue("Index", login);
                    var reader = mainCommand.ExecuteReader();
                    if (!reader.Read()) 
                    {
                        reader.Close();
                        return false;
                    }
                    if (!PasswordEncryptor.Validate(password, reader["Salt"].ToString(), reader["Password"].ToString()))
                    {
                        reader.Close();
                        return false;
                    }
                    
                    reader.Close();
                    return true;
                }
            }
        }
        
        [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
        public static string GetRole(string index)
        {
            using (var sqlConnection = new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                sqlConnection.Open();
                using (var mainCommand = new SqlCommand())
                {
                    mainCommand.Connection = sqlConnection;
                    mainCommand.CommandText = "SELECT Role " +
                                              "From Student " + 
                                              "Where IdStudent=@Index";
                    mainCommand.Parameters.AddWithValue("Index", index);
                    var reader = mainCommand.ExecuteReader();
                    if (reader.Read()) return reader["Role"].ToString();
                    reader.Close();
                    return null;
                }
            }
        }
        
        [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
        public static void UpdateRefreshToken(string login, Guid refreshToken)
        {
            using (var sqlConnection = new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                sqlConnection.Open();
                var transaction = sqlConnection.BeginTransaction();
                using (var mainCommand = new SqlCommand())
                {
                    try
                    {
                        mainCommand.Transaction = transaction;
                        mainCommand.Connection = sqlConnection;
                        mainCommand.CommandText = "UPDATE Student " +
                                                  "SET RefreshToken = @Token " +
                                                  "WHERE IdStudent=@Index";
                        mainCommand.Parameters.AddWithValue("Token", refreshToken);
                        mainCommand.Parameters.AddWithValue("Index", login);
                        mainCommand.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
        
        [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
        public static string FindStudentByRefreshToken(string refreshToken)
        {
            using (var sqlConnection = new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                sqlConnection.Open();
                using (var mainCommand = new SqlCommand())
                {
                    mainCommand.Connection = sqlConnection;
                    mainCommand.CommandText = "SELECT IdStudent " +
                                              "FROM Student " +
                                              "WHERE RefreshToken = @RefreshToken";
                    mainCommand.Parameters.AddWithValue("RefreshToken", refreshToken);
                    var reader = mainCommand.ExecuteReader();
                    if (reader.Read()) return reader["IdStudent"].ToString();
                    reader.Close();
                    return null;
                }
            }
        }
    }
}
