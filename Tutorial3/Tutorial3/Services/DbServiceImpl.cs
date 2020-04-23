using System.Data.SqlClient;

namespace Tutorial3.Services
{
    public class DbServiceImpl : IDbService
    {
        public bool IsThereStudentWithId(string idStudent)
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
    }
}