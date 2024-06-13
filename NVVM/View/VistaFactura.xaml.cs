using System.Windows;
using System.Windows.Controls;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaFactura : UserControl
    {
        private FacturaRepository _facturaRepository;
        private Factura _selectedFactura;
        private static readonly Random Random = new Random();

        public VistaFactura()
        {
            InitializeComponent();
            _facturaRepository = new FacturaRepository();
            LoadFacturasAsync();
        }

        private async void LoadFacturasAsync()
        {
            try
            {
                var facturas = await Task.Run(() => _facturaRepository.GetAllFacturas());
                var filteredFacturas = facturas.Where(f => f.Confirmado && !f.Enviado).ToList();
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

        private async void MarcarComoEnviadoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFactura != null)
            {
                try
                {
                    _selectedFactura.Enviado = true;
                    await Task.Run(() => _facturaRepository.UpdateFactura(_selectedFactura));
                    MessageBox.Show("Factura marcada como enviada exitosamente.");

                    // Recargar las facturas
                    LoadFacturasAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error marcando la factura como enviada: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecciona una factura primero.");
            }
        }
    }
}
