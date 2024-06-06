using System;
using System.Linq;
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
    public partial class VistaPrincipal : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;
        private ScrollViewer _scrollViewer;
        private DispatcherTimer _dragTimer;

        public VistaPrincipal()
        {
            InitializeComponent();
            LoadProducts();

            _dragTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.25)
            };
            _dragTimer.Tick += DragTimer_Tick;
        }

        private void LoadProducts()
        {
            var productoRepository = new ProductoRepository();
            var productos = productoRepository.GetAllProductos();
            var random = new Random();

            HorizontalWrapPanel1.Children.Clear();
            VerticalGridPanel.Children.Clear();
            VerticalGridPanel.RowDefinitions.Clear();
            VerticalGridPanel.ColumnDefinitions.Clear();

            foreach (var producto in productos)
            {
                var viewModel = new ProductoDisplay
                {
                    NombreProducto = producto.NombreProducto,
                    CategoriaNombre = producto.CategoriaNombre,
                    BackgroundGradient = GetRandomGradient(),
                    ImageSource = GetImageSourceByCategory(producto.CategoriaNombre),
                     Precio = producto.Precio,
                    Descripcion = producto.Descripcion,
                };

                var productDisplay1 = new ProductDisplay
                {
                    DataContext = viewModel,
                    Margin = new Thickness(20, 0, 40, 0),
                    Width = 400,
                    Height = 300
                };

                HorizontalWrapPanel1.Children.Add(productDisplay1);
            }

            // Shuffle the products list for the second panel
            var shuffledProductos = productos.OrderBy(x => random.Next()).ToList();

            // Adding products to the second panel with rows and columns
            const int productsPerRow = 5;
            const int rowCount = 5;
            for (int i = 0; i < rowCount; i++)
            {
                VerticalGridPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int j = 0; j < productsPerRow; j++)
            {
                VerticalGridPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int i = 0; i < shuffledProductos.Count; i++)
            {
                var producto = shuffledProductos[i];
                var viewModel = new ProductoDisplay
                {
                    NombreProducto = producto.NombreProducto,
                    CategoriaNombre = producto.CategoriaNombre,
                    Precio = producto.Precio,
                    Descripcion = producto.Descripcion,
                    BackgroundGradient = GetRandomGradient(),
                    ImageSource = GetImageSourceByCategory(producto.CategoriaNombre)
                };

                var productDisplay = new ProductDisplay
                {
                    DataContext = viewModel,
                    Margin = new Thickness(20, 0, 40, 0),
                    Width = 400,
                    Height = 300
                };

                Grid.SetRow(productDisplay, i / productsPerRow);
                Grid.SetColumn(productDisplay, i % productsPerRow);

                VerticalGridPanel.Children.Add(productDisplay);
            }
        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)
                return; // Ignore the event if it comes from a button

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
                "Tablets" => "pack://application:,,,/Imagenes/2.png",
                "Accessories" => "pack://application:,,,/Imagenes/2.png",
                _ => "pack://application:,,,/Imagenes/3.png",
            };
            return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }
    }
}
