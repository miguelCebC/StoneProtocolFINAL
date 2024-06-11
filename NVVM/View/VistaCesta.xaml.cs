using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using StoneProtocol.NVVM.Model;
using StoneProtocol.Theme;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaCesta : UserControl
    {
        private FacturaRepository _facturaRepository;
        private Factura _selectedFactura;
        private static readonly Random Random = new Random();
        private readonly Dictionary<string, ImageSource> _imageCache = new Dictionary<string, ImageSource>();
        private Point _startPoint;
        private bool _isDragging;
        private ScrollViewer _scrollViewer;
        private DispatcherTimer _dragTimer;

        public VistaCesta()
        {
            InitializeComponent();
            try
            {
                _facturaRepository = new FacturaRepository();
                LoadFirstFacturaAsync();

                _dragTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0.25)
                };
                _dragTimer.Tick += DragTimer_Tick;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing VistaCesta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadFirstFacturaAsync()
        {
            try
            {
                var facturas = await Task.Run(() => _facturaRepository.GetFacturasByUserId(AppState.UserId));
                var unconfirmedFacturas = facturas.Where(f => !f.Confirmado).ToList();

                if (unconfirmedFacturas.Any())
                {
                    _selectedFactura = unconfirmedFacturas.First();
                    PopulateLineasFacturaDisplays(_selectedFactura.LineasFactura);
                    CalculateTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                TotalTextBlock.Text = $"Total: ${total:F2}";
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

        private async void ConfirmarFacturaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFactura != null && !string.IsNullOrEmpty(DireccionTextBox.Text) && MetodoPagoComboBox.SelectedItem != null)
            {
                try
                {
                    // Guardar la dirección antes de confirmar la factura
                    _selectedFactura.Direccion = DireccionTextBox.Text;

                    // Confirmar la factura
                    _selectedFactura.Confirmado = true;

                    await Task.Run(() => _facturaRepository.UpdateFactura(_selectedFactura));
                    MessageBox.Show("Factura confirmada exitosamente.");

                    // Crear una nueva factura vacía para el mismo usuario
                    var nuevaFactura = new Factura
                    {
                        Fecha = DateTime.Now,
                        UsuarioId = _selectedFactura.UsuarioId,
                        Confirmado = false,
                        Enviado = false,
                        Direccion = string.Empty, // Inicializar la dirección como vacío
                        LineasFactura = new List<LineaFactura>()
                    };
                    await Task.Run(() => _facturaRepository.CreateFactura(nuevaFactura));
                    MessageBox.Show("Nueva factura vacía creada exitosamente.");

                    // Recargar las facturas
                    LoadFirstFacturaAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error confirming factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, rellena todos los campos requeridos.");
            }
        }


        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)
                return;

            if (sender is ScrollViewer scrollViewer)
            {
                _scrollViewer = scrollViewer;
                _startPoint = e.GetPosition(scrollViewer);
                _dragTimer.Start();
            }
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _scrollViewer != null)
            {
                var currentPoint = e.GetPosition(_scrollViewer);
                var offsetX = _startPoint.X - currentPoint.X;
                var offsetY = _startPoint.Y - currentPoint.Y;

                if (_scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset + offsetX);
                }

                if (_scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset + offsetY);
                }

                _startPoint = currentPoint;
            }
        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragTimer.Stop();
            _isDragging = false;
            if (_scrollViewer != null)
            {
                _scrollViewer.ReleaseMouseCapture();
                _scrollViewer = null;
            }
        }

        private void DragTimer_Tick(object sender, EventArgs e)
        {
            _dragTimer.Stop();
            if (_scrollViewer != null)
            {
                _scrollViewer.CaptureMouse();
                _isDragging = true;
            }
        }
    }
}
