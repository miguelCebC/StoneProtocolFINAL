
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
                FacturasDataGrid.ItemsSource = facturas;
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

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fecha = SearchDateDatePicker.SelectedDate;
                var facturas = await Task.Run(() => _facturaRepository.GetAllFacturas());
                if (fecha.HasValue)
                {
                    facturas = facturas.Where(f => f.Fecha.Date == fecha.Value.Date).ToList();
                }
                FacturasDataGrid.ItemsSource = facturas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void OnAddProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (_selectedFactura != null)
            {
                Producto producto = ObtenerProductoSeleccionado(); // Implementa este método según tus necesidades

                var productWindow = new ProductWindow();
                productWindow.ShowDialog();

                // Actualiza la vista de las líneas de factura después de añadir un producto
                PopulateLineasFacturaDisplays(_selectedFactura.LineasFactura);
            }
            else
            {
                MessageBox.Show("Selecciona una factura primero.");
            }
        }

        private Producto ObtenerProductoSeleccionado()
        {
            // Simulación de selección de producto
            return new Producto
            {
                Id = 1,
                NombreProducto = "Producto de Ejemplo",
                CategoriaId = 1,
                CategoriaNombre = "Categoría de Ejemplo",
                ImageSource = "pack://application:,,,/Imagenes/ejemplo.png",
                Descripcion = "Descripción de ejemplo",
                Precio = 100.0
            };
        }
    }
}
