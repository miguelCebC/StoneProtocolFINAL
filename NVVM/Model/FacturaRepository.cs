using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace StoneProtocol.NVVM.Model
{
    public class FacturaRepository
    {
        private readonly Database database;

        public FacturaRepository()
        {
            database = new Database();
        }

        public List<Factura> GetAllFacturas()
        {
            var facturas = new List<Factura>();

            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT f.id, f.fecha, f.usuario_id
                    FROM facturas f";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facturas.Add(new Factura
                        {
                            Id = reader.GetInt32("id"),
                            Fecha = reader.GetDateTime("fecha"),
                            UsuarioId = reader.GetInt32("usuario_id"),
                            LineasFactura = new List<LineaFactura>()
                        });
                    }
                }
            }

            foreach (var factura in facturas)
            {
                factura.LineasFactura = GetLineasFacturaByFacturaId(factura.Id);
            }

            return facturas;
        }

        private List<LineaFactura> GetLineasFacturaByFacturaId(int facturaId)
        {
            var lineasFactura = new List<LineaFactura>();

            using (var connection = database.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT lf.id, lf.factura_id, lf.producto_id, lf.cantidad, lf.precio_unitario,
                           p.nombre_producto, p.categoria_id, c.nombre as categoria_nombre, p.descripcion, p.precio
                    FROM lineas_factura lf
                    JOIN productos p ON lf.producto_id = p.id
                    JOIN categorias c ON p.categoria_id = c.id
                    WHERE lf.factura_id = @FacturaId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FacturaId", facturaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lineasFactura.Add(new LineaFactura
                            {
                                Id = reader.GetInt32("id"),
                                FacturaId = reader.GetInt32("factura_id"),
                                ProductoId = reader.GetInt32("producto_id"),
                                Cantidad = reader.GetInt32("cantidad"),
                                PrecioUnitario = reader.GetDecimal("precio_unitario"),
                                Producto = new Producto
                                {
                                    Id = reader.GetInt32("producto_id"),
                                    NombreProducto = reader.GetString("nombre_producto"),
                                    CategoriaNombre = reader.GetString("categoria_nombre"),
                                    Descripcion = reader.GetString("descripcion"),
                                    Precio = reader.GetDouble("precio")
                                }
                            });
                        }
                    }
                }
            }

            return lineasFactura;
        }
    }
}
