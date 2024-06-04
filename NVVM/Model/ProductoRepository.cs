using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace StoneProtocol.NVVM.Model
{
    public class ProductoRepository
    {
        private readonly Database database;

        public ProductoRepository()
        {
            database = new Database();
        }

        public List<Producto> GetAllProductos()
        {
            var productos = new List<Producto>();

            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT p.id, p.nombre_producto, p.categoria_id, c.nombre as categoria_nombre
                    FROM productos p
                    JOIN categorias c ON p.categoria_id = c.id";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            Id = reader.GetInt32("id"),
                            NombreProducto = reader.GetString("nombre_producto"),
                            CategoriaId = reader.GetInt32("categoria_id"),
                            CategoriaNombre = reader.GetString("categoria_nombre")
                        });
                    }
                }
            }

            return productos;
        }

        public void UpdateProducto(Producto producto)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "UPDATE productos SET nombre_producto = @nombreProducto, categoria_id = (SELECT id FROM categorias WHERE nombre = @categoriaNombre) WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", producto.Id);
                    cmd.Parameters.AddWithValue("@nombreProducto", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@categoriaNombre", producto.CategoriaNombre);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProducto(int id)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM productos WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddProducto(Producto producto)
        {
            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO productos (nombre_producto, categoria_id) VALUES (@nombreProducto, (SELECT id FROM categorias WHERE nombre = @categoriaNombre))";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombreProducto", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@categoriaNombre", producto.CategoriaNombre);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
