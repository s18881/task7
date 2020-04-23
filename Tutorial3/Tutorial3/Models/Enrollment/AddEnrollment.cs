using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Tutorial3.Models.Enrollment
{
    public class AddEnrollment
    {
        public string IdStudent { get; set; }
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string BirthDate { get; set;}
        public string Studies { get; set;}
        public string Password { get; set;}
        public string Role { get; set;}

        public override string ToString()
        {
            return IdStudent + " " + Studies +  "  " + Role;
        }

        public AddEnrollment()
        {
        }

        public AddEnrollment(string idStudent, string firstName, string lastName, string birthDate, string studies, string password, string role)
        {
            IdStudent = idStudent;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Studies = studies;
            Password = password;
            Role = role;
        }

        public AddEnrollment(string idStudent)
        {
            IdStudent = idStudent;
        }

        public static bool IsThereStudentWithId(string idStudent)
        {
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT IdStudent " +
                              "FROM Student " +
                              "WHERE IdStudent=" +
                              idStudent
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (int.TryParse(dataReader["IdStudent"].ToString(), out _)) return true;
            }
            return false;
        }
        
        public static bool IsThereEnrollmentWithId(string idEnrollment)
        {
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT IdEnrollment " +
                              "FROM Enrollment " +
                              "WHERE IdEnrollment=" +
                              idEnrollment
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (int.TryParse(dataReader["IdEnrollment"].ToString(), out _)) return true;
            }
            return false;
        }

        public static bool IsThereStudy(string studyName)
        {
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT IdStudy " +
                              "FROM Studies " +
                              "WHERE Name=\'" +
                              studyName + "\'"
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (int.TryParse(dataReader["IdStudy"].ToString(), out _)) return true;
            }
            return false;
        }
        
        public static bool DoesStudyHaveSemesterOne(string studyName)
        {
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT Semester " +
                              "FROM Studies, Enrollment " +
                              "WHERE Studies.IdStudy=Enrollment.IdStudy " +
                              "AND Name='" +
                              studyName + "'"
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var sem = dataReader["Semester"];
                int n;
                if (!int.TryParse(sem.ToString(), out n)) continue;
                if(n == 1) return true;
            }
            return false;
        }

        public static int GetStudyId(string studyName)
        {
            int idStudy;
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT IdStudy " +
                              "FROM Studies " +
                              "WHERE Name='" +
                              studyName + "'"
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (int.TryParse(dataReader["IdStudy"].ToString(), out idStudy)) return idStudy;
            }
            idStudy = 0;
            return idStudy;
        }
        
        
        
        public static int GetEnrollmentIdWithSemOne(int idStudy)
        {
            int idEnrollment;
            using var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;");
            using var command = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "SELECT IdEnrollment " +
                              "FROM Studies, Enrollment " +
                              "WHERE Studies.IdStudy=Enrollment.IdStudy " +
                              "AND Semester = 1 " +
                              "AND Studies.IdStudy=" +
                              idStudy
            };
            sqlConnection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (int.TryParse(dataReader["IdEnrollment"].ToString(), out idEnrollment)) return idEnrollment;
            }
            idEnrollment = 0;
            return idEnrollment;
        }

        public static AddEnrollment GetStudent(string idStudent)
        {
            using (var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandText = "SELECT IdStudent, FirstName, LastName, BirthDate, Name, Password, Role " +
                                          "FROM Student, Enrollment, Studies " +
                                          "WHERE Student.IdEnrollment = Enrollment.IdEnrollment " +
                                          "AND Enrollment.IdStudy = Studies.IdStudy " +
                                          "AND IdStudent ='" + idStudent + "'";
                    sqlConnection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        return new AddEnrollment
                        {
                            IdStudent = dataReader["IdStudent"].ToString(),
                            FirstName = dataReader["FirstName"].ToString(),
                            LastName = dataReader["LastName"].ToString(),
                            BirthDate = dataReader["BirthDate"].ToString(),
                            Studies = dataReader["Name"].ToString(),
                            Password = dataReader["Password"].ToString(),
                            Role = dataReader["Role"].ToString()
                        };
                    }
                    return null;
                }
            }
        }
    }
}