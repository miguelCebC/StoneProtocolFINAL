using System.IO;

using System.Windows;
using System.Windows.Controls;

using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaFacturasEnviadas : UserControl
    {
        private FacturaRepository _facturaRepository;
        private Factura _selectedFactura;
        private Usuario _currentUser;
        private UsuarioRepository _usuarioRepository;

        public VistaFacturasEnviadas()
        {
            InitializeComponent();
            _usuarioRepository = new UsuarioRepository();
            _currentUser = _usuarioRepository.GetUsuarioById(AppState.UserId);
            _facturaRepository = new FacturaRepository();
            LoadFacturas();
        }

        private async void LoadFacturas()
        {
            try
            {
                var facturas = _facturaRepository.GetAllFacturas();
                var filteredFacturas = _currentUser.Admin ?
                    facturas.Where(f => f.Enviado).ToList() :
                    facturas.Where(f => f.Enviado && f.UsuarioId == _currentUser.Id).ToList();
                FacturasDataGrid.ItemsSource = filteredFacturas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FacturasDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FacturasDataGrid.SelectedItem is Factura selectedFactura)
            {
                _selectedFactura = selectedFactura;
                PopulateLineasFacturaDisplays(_selectedFactura.LineasFactura);
                CalculateTotal();
                SetUsuarioYDireccion(_selectedFactura.UsuarioId, _selectedFactura.Direccion);
            }
        }

        private void PopulateLineasFacturaDisplays(IEnumerable<LineaFactura> lineasFactura)
        {
            LineasFacturaDataGrid.ItemsSource = lineasFactura;
        }

        private void CalculateTotal()
        {
            if (_selectedFactura != null)
            {
                decimal total = _selectedFactura.LineasFactura.Sum(lf => (decimal)(lf.Producto.Precio * lf.Cantidad));
                TotalTextBlock.Text = $"Total: {total:F2}€";
            }
        }

        private void SetUsuarioYDireccion(int usuarioId, string direccion)
        {
            var usuarioRepository = new UsuarioRepository();
            var usuario = usuarioRepository.GetUsuarioById(usuarioId);

            UsuarioTextBlock.Text = $"Usuario: {usuario.Nombre}";
            DireccionTextBlock.Text = $"Dirección: {direccion}";
        }

      

     

        private void ImprimirFacturaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFactura == null)
            {
                MessageBox.Show("Seleccione una factura para imprimir.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string html = GenerarFacturaHtml(_selectedFactura);
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Factura_{_selectedFactura.Id}.html");

                File.WriteAllText(filePath, html);

                MessageBox.Show($"Factura guardada en: {filePath}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abrir el archivo HTML en el navegador predeterminado
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir la factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerarFacturaHtml(Factura factura)
        {
            var usuarioRepository = new UsuarioRepository();
            var usuario = usuarioRepository.GetUsuarioById(factura.UsuarioId);

            var html = $@"
                <html>
                <head>
                    <title>Factura #{factura.Id}</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        table {{ width: 100%; border-collapse: collapse; }}
                        th, td {{ padding: 8px; border: 1px solid #ddd; text-align: left; }}
                        th {{ background-color: #f2f2f2; }}
                        .total {{ font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <h1>Factura #{factura.Id}</h1>
                    <p><strong>Fecha:</strong> {factura.Fecha}</p>
                    <p><strong>Usuario:</strong> {usuario.Nombre}</p>
                    <p><strong>Dirección:</strong> {factura.Direccion}</p>
                    <table>
                        <thead>
                            <tr>
                                <th>Producto</th>
                                <th>Categoría</th>
                                <th>Precio</th>
                                <th>Cantidad</th>
                                <th>Total</th>
                            </tr>
                        </thead>
                        <tbody>";

            foreach (var linea in factura.LineasFactura)
            {
                html += $@"
                            <tr>
                                <td>{linea.Producto.NombreProducto}</td>
                                <td>{linea.Producto.CategoriaNombre}</td>
                                <td>{linea.Producto.Precio:F2}€</td>
                                <td>{linea.Cantidad}</td>
                                <td>{linea.Producto.Precio * linea.Cantidad:F2}€</td>
                            </tr>";
            }

            decimal totalFactura = factura.LineasFactura.Sum(lf => (decimal)(lf.Producto.Precio * lf.Cantidad));

            html += $@"
                        </tbody>
                    </table>
                    <p class='total'><strong>Total:</strong> {totalFactura:F2}€</p>
                </body>
                </html>";

            return html;
        }

        private void BorrarFacturaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFactura == null)
            {
                MessageBox.Show("Seleccione una factura para borrar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("¿Está seguro de que desea borrar esta factura y todas sus líneas?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _facturaRepository.DeleteFactura(_selectedFactura.Id);
                    _selectedFactura = null;
                    LoadFacturas();
                    MessageBox.Show("Factura y sus líneas borradas exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al borrar la factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
