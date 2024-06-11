using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoneProtocol.NVVM.Model;
using StoneProtocol.Theme;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaFactura : UserControl
    {
        private FacturaRepository _facturaRepository;
        private Factura _selectedFactura;
        private static readonly Random Random = new Random();
        private readonly Dictionary<string, ImageSource> _imageCache = new Dictionary<string, ImageSource>();

        public VistaFactura()
        {
            InitializeComponent();
            try
            {
                _facturaRepository = new FacturaRepository();
                LoadFacturasAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing VistaFactura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            LineasFacturaWrapPanel.Children.Clear();

            foreach (var lineaFactura in lineasFactura)
            {
                var viewModel = new ProductoDisplay
                {
                    NombreProducto = lineaFactura.Producto.NombreProducto,
                    CategoriaNombre = lineaFactura.Producto.CategoriaNombre,
                    BackgroundGradient = GetRandomGradient(),
                    ImageSource = GetImageSourceByCategory(lineaFactura.Producto.CategoriaNombre),
                    Precio = lineaFactura.Producto.Precio,
                    Descripcion = lineaFactura.Producto.Descripcion,
                };

                var productDisplay = new ProductDisplay()
                {
                    DataContext = viewModel,
                    Margin = new Thickness(20, 0, 40, 0),
                    Width = 400,
                    Height = 300
                };

                LineasFacturaWrapPanel.Children.Add(productDisplay);
            }
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
            // Aquí debes obtener el nombre del usuario a partir de su ID
            // Supongo que tienes un método GetUsuarioById en tu repositorio de usuarios
            var usuarioRepository = new UsuarioRepository();
            var usuario = usuarioRepository.GetUsuarioById(usuarioId);

            UsuarioTextBlock.Text = $"Usuario: {usuario.Nombre}";
            DireccionTextBlock.Text = $"Dirección: {direccion}";
        }

        private LinearGradientBrush GetRandomGradient()
        {
            Color color1 = Color.FromRgb((byte)Random.Next(256), (byte)Random.Next(256), (byte)Random.Next(256));
            Color color2 = Color.FromRgb((byte)Random.Next(256), (byte)Random.Next(256), (byte)Random.Next(256));
            return new LinearGradientBrush(color1, color2, 45);
        }

        private ImageSource GetImageSourceByCategory(string category)
        {
            if (!_imageCache.TryGetValue(category, out var imageSource))
            {
                string imagePath = category switch
                {
                    "Smartphones" => "pack://application:,,,/Imagenes/movil.png",
                    "Laptops" => "pack://application:,,,/Imagenes/portatil.png",
                    "Tablets" => "pack://application:,,,/Imagenes/tablet.png",
                    "Accessories" => "pack://application:,,,/Imagenes/acc.png",
                    _ => "pack://application:,,,/Imagenes/3.png",
                };
                imageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                _imageCache[category] = imageSource;
            }

            return imageSource;
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
