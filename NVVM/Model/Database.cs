﻿using MySql.Data.MySqlClient;


namespace StoneProtocol.NVVM.Model
{
  
     public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            //connectionString = "server=sql7.freemysqlhosting.net;database=sql7712035;user=sql7712035;password=8aGxfmHxFi;port=3306;";
             connectionString = "server=108.181.197.184;database=stoneprotocol;user=admin;password=3nmNkxdK;port=10058;";
        }

        public MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            return connection;
        }


    }
}
