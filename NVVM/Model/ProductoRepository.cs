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
    }
}
