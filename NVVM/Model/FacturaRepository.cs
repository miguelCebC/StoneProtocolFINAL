using System;
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

        public void CreateFactura(Factura factura)
        {
            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO facturas (fecha, usuario_id, confirmado, enviado, direccion)
                        VALUES (@Fecha, @UsuarioId, @Confirmado, @Enviado, @Direccion)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                        cmd.Parameters.AddWithValue("@UsuarioId", factura.UsuarioId);
                        cmd.Parameters.AddWithValue("@Confirmado", factura.Confirmado);
                        cmd.Parameters.AddWithValue("@Enviado", factura.Enviado);
                        cmd.Parameters.AddWithValue("@Direccion", factura.Direccion);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating factura: {ex.Message}");
            }
        }

        public Factura GetFirstUnconfirmedFacturaByUserId(int userId)
        {
            Factura factura = null;

            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT f.id, f.fecha, f.usuario_id, f.confirmado, f.enviado, f.direccion
                        FROM facturas f
                        WHERE f.usuario_id = @UsuarioId AND f.confirmado = 0
                        ORDER BY f.fecha ASC
                        LIMIT 1";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                factura = new Factura
                                {
                                    Id = reader.GetInt32("id"),
                                    Fecha = reader.GetDateTime("fecha"),
                                    UsuarioId = reader.GetInt32("usuario_id"),
                                    Confirmado = reader.GetBoolean("confirmado"),
                                    Enviado = reader.GetBoolean("enviado"),
                                    Direccion = reader.GetString("direccion"),
                                    LineasFactura = GetLineasFacturaByFacturaId(reader.GetInt32("id"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving first unconfirmed factura: {ex.Message}");
            }

            return factura;
        }

        public void AddLineaFactura(LineaFactura lineaFactura)
        {
            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO lineas_factura (factura_id, producto_id, cantidad, precio_unitario)
                        VALUES (@FacturaId, @ProductoId, @Cantidad, @PrecioUnitario)";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FacturaId", lineaFactura.FacturaId);
                        cmd.Parameters.AddWithValue("@ProductoId", lineaFactura.ProductoId);
                        cmd.Parameters.AddWithValue("@Cantidad", lineaFactura.Cantidad);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", lineaFactura.PrecioUnitario);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding linea factura: {ex.Message}");
            }
        }

        public List<Factura> GetAllFacturas()
        {
            var facturas = new List<Factura>();

            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT f.id, f.fecha, f.usuario_id, f.confirmado, f.enviado, f.direccion
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
                                Confirmado = reader.GetBoolean("confirmado"),
                                Enviado = reader.GetBoolean("enviado"),
                                Direccion = reader.GetString("direccion"),
                                LineasFactura = new List<LineaFactura>()
                            });
                        }
                    }
                }

                foreach (var factura in facturas)
                {
                    factura.LineasFactura = GetLineasFacturaByFacturaId(factura.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving facturas: {ex.Message}");
            }

            return facturas;
        }

        public List<Factura> GetFacturasByUserId(int userId)
        {
            var facturas = new List<Factura>();

            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT f.id, f.fecha, f.usuario_id, f.confirmado, f.enviado, f.direccion
                        FROM facturas f
                        WHERE f.usuario_id = @UsuarioId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var factura = new Factura
                                {
                                    Id = reader.GetInt32("id"),
                                    Fecha = reader.GetDateTime("fecha"),
                                    UsuarioId = reader.GetInt32("usuario_id"),
                                    Confirmado = reader.GetBoolean("confirmado"),
                                    Enviado = reader.GetBoolean("enviado"),
                                    Direccion = reader.GetString("direccion"),
                                    LineasFactura = GetLineasFacturaByFacturaId(reader.GetInt32("id"))
                                };
                                facturas.Add(factura);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving facturas: {ex.Message}");
            }

            return facturas;
        }

        public void UpdateFactura(Factura factura)
        {
            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        UPDATE facturas 
                        SET confirmado = @Confirmado, enviado = @Enviado, direccion = @Direccion
                        WHERE id = @Id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Confirmado", factura.Confirmado);
                        cmd.Parameters.AddWithValue("@Enviado", factura.Enviado);
                        cmd.Parameters.AddWithValue("@Direccion", factura.Direccion);
                        cmd.Parameters.AddWithValue("@Id", factura.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating factura: {ex.Message}");
            }
        }

        public void DeleteLineasFacturaByFacturaId(int facturaId)
        {
            try
            {
                using (var connection = database.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        DELETE FROM lineas_factura
                        WHERE factura_id = @FacturaId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FacturaId", facturaId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting lineas factura: {ex.Message}");
            }
        }

        private List<LineaFactura> GetLineasFacturaByFacturaId(int facturaId)
        {
            var lineasFactura = new List<LineaFactura>();

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving lineas factura: {ex.Message}");
            }

            return lineasFactura;
        }
    }
}
