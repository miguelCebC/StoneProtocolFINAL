using System;
using System.Collections.Generic;
using System.Linq;
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

        public VistaFactura()
        {
            InitializeComponent();
            try
            {
                _facturaRepository = new FacturaRepository();
                LoadFacturas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing VistaFactura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFacturas()
        {
            try
            {
                FacturasDataGrid.ItemsSource = _facturaRepository.GetAllFacturas();
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

                var productDisplay = new ProductDisplay
                {
                    DataContext = viewModel,
                    Margin = new Thickness(20, 0, 40, 0),
                    Width = 400,
                    Height = 300
                };

                LineasFacturaWrapPanel.Children.Add(productDisplay);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fecha = SearchDateDatePicker.SelectedDate;
                var facturas = _facturaRepository.GetAllFacturas();
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
            Random rand = new Random();
            Color color1 = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
            Color color2 = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
            return new LinearGradientBrush(color1, color2, 45);
        }

        private static ImageSource GetImageSourceByCategory(string category)
        {
            string imagePath = category switch
            {
                "Smartphones" => "pack://application:,,,/Imagenes/movil.png",
                "Laptops" => "pack://application:,,,/Imagenes/portatil.png",
                "Tablets" => "pack://application:,,,/Imagenes/tablet.png",
                "Accessories" => "pack://application:,,,/Imagenes/acc.png",
                _ => "pack://application:,,,/Imagenes/3.png",
            };
            return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }
    }
}
