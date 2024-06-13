using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaFacturasEnviadas : UserControl
    {
        private FacturaRepository _facturaRepository;
        private Factura _selectedFactura;
        private Usuario _currentUser;
        private UsuarioRepository _usuarioRepository;
        private static readonly Random Random = new Random();
        private readonly Dictionary<string, ImageSource> _imageCache = new Dictionary<string, ImageSource>();

        public VistaFacturasEnviadas()
        {
            InitializeComponent();
            _usuarioRepository = new UsuarioRepository();
            _currentUser = _usuarioRepository.GetUsuarioById(AppState.UserId);
            _facturaRepository = new FacturaRepository();

            LoadFacturas();
        }

        private void LoadFacturas()
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
    }
}
