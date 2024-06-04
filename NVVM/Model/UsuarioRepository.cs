using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace StoneProtocol.NVVM.Model
{
    public class UsuarioRepository
    {
        private readonly Database database;

        public UsuarioRepository()
        {
            database = new Database();
        }

        public void CreateUsuario(Usuario usuario)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO usuarios (nombre, email, admin) VALUES (@Nombre, @Email, @Admin)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Admin", usuario.Admin);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Usuario> ReadUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "SELECT id, nombre, email, admin FROM usuarios";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Nombre = reader.GetString("nombre"),
                            Email = reader.GetString("email"),
                            Admin = reader.GetBoolean("admin")
                        });
                    }
                }
            }
            return usuarios;
        }

        public void UpdateUsuario(Usuario usuario)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "UPDATE usuarios SET nombre = @Nombre, email = @Email, admin = @Admin WHERE id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Admin", usuario.Admin);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUsuario(Usuario usuario)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM usuarios WHERE id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
