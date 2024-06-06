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
                    SELECT p.id, p.nombre_producto, p.categoria_id, c.nombre as categoria_nombre, p.descripcion, p.precio
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
                            CategoriaNombre = reader.GetString("categoria_nombre"),
                            Descripcion = reader.GetString("descripcion"),
                            Precio = reader.GetDouble("precio")
                        });
                    }
                }
            }

            return productos;
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();

            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = "SELECT nombre FROM categorias";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(reader.GetString("nombre"));
                    }
                }
            }

            return categories;
        }

        public List<Producto> SearchProductos(string nombre, string categoria)
        {
            var productos = new List<Producto>();

            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT p.id, p.nombre_producto, p.categoria_id, c.nombre as categoria_nombre, p.descripcion, p.precio
                    FROM productos p
                    JOIN categorias c ON p.categoria_id = c.id
                    WHERE (@nombre IS NULL OR p.nombre_producto LIKE @nombre) 
                      AND (@categoria IS NULL OR c.nombre = @categoria)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(nombre) ? (object)DBNull.Value : $"%{nombre}%");
                    cmd.Parameters.AddWithValue("@categoria", string.IsNullOrEmpty(categoria) ? (object)DBNull.Value : categoria);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                Id = reader.GetInt32("id"),
                                NombreProducto = reader.GetString("nombre_producto"),
                                CategoriaId = reader.GetInt32("categoria_id"),
                                CategoriaNombre = reader.GetString("categoria_nombre"),
                                Descripcion = reader.GetString("descripcion"),
                                Precio = reader.GetDouble("precio")
                            });
                        }
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
                string query = "UPDATE productos SET nombre_producto = @nombreProducto, categoria_id = (SELECT id FROM categorias WHERE nombre = @categoriaNombre), descripcion = @descripcion, precio = @precio WHERE id = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", producto.Id);
                    cmd.Parameters.AddWithValue("@nombreProducto", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@categoriaNombre", producto.CategoriaNombre);
                    cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
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
                string query = "INSERT INTO productos (nombre_producto, categoria_id, descripcion, precio) VALUES (@nombreProducto, (SELECT id FROM categorias WHERE nombre = @categoriaNombre), @descripcion, @precio)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombreProducto", producto.NombreProducto);
                    cmd.Parameters.AddWithValue("@categoriaNombre", producto.CategoriaNombre);
                    cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
