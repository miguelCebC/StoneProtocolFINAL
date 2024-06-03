using MySql.Data.MySqlClient;


namespace StoneProtocol.NVVM.Model
{
  
     public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            connectionString = "server=localhost;database=stoneprotocol;user=root;password=;port=3306;";
        }

        public MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }


    }
}
