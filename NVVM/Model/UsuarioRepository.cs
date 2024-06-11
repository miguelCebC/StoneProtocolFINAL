using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace StoneProtocol.NVVM.Model
{
    public class UsuarioRepository
    {
        private readonly Database _database;

        public UsuarioRepository()
        {
            _database = new Database();
        }

        public void CreateUsuario(Usuario usuario)
        {
            using (var connection = _database.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO usuarios (nombre, email, admin, contrasena) VALUES (@Nombre, @Email, @Admin, @Contrasena)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Admin", usuario.Admin);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Usuario> ReadUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (var connection = _database.GetConnection())
            {
                connection.Open();
                string query = "SELECT id, nombre, email, admin, contrasena FROM usuarios";
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
                            Admin = reader.GetBoolean("admin"),
                            Contrasena = reader.GetString("contrasena")
                        });
                    }
                }
            }
            return usuarios;
        }

        public void UpdateUsuario(Usuario usuario)
        {
            using (var connection = _database.GetConnection())
            {
                connection.Open();
                string query = "UPDATE usuarios SET nombre = @Nombre, email = @Email, admin = @Admin, contrasena = @Contrasena WHERE id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Admin", usuario.Admin);
                    cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUsuario(Usuario usuario)
        {
            using (var connection = _database.GetConnection())
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
