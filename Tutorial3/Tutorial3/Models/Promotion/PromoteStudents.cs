using System.Data.SqlClient;

namespace Tutorial3.Models.Promotion
{
    public class PromoteStudents
    {
        public string Studies { get; set; }
        public string Semester { get; set; }

        public PromoteStudents()
        {
        }

        public PromoteStudents(string studies, string semester)
        {
            Studies = studies;
            Semester = semester;
        }

        public PromoteStudents(string studies)
        {
            Studies = studies;
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
        
        public static PromoteStudents GetEnrollment(string studiesName)
        {
            using (var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandText = "SELECT Name, MAX(Semester) AS MaxSem " +
                                          "FROM Enrollment, Studies " +
                                          "WHERE Enrollment.IdStudy = Studies.IdStudy " +
                                          "AND Name ='" + studiesName + "' " +
                                          "GROUP BY Name";
                    sqlConnection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        return new PromoteStudents()
                        {
                            Studies = dataReader["Name"].ToString(),
                            Semester = dataReader["MaxSem"].ToString()
                        };
                    }
                    return null;
                }
            }
        }
    }
    
    
}