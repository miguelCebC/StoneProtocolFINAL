
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoneProtocol.NVVM.Model;
using StoneProtocol.Theme;

namespace StoneProtocol.NVVM.View
{
  
    public partial class VistaPrincipal : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;
        private TranslateTransform _transform;

        public VistaPrincipal()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var productoRepository = new ProductoRepository();
            var productos = productoRepository.GetAllProductos();
            var random = new Random();

            HorizontalStackPanel1.Children.Clear();
            VerticalStackPanel.Children.Clear();

            foreach (var producto in productos)
            {
                var viewModel = new ProductoDisplay
                {
                    NombreProducto = producto.NombreProducto,
                    CategoriaNombre = producto.CategoriaNombre,
                    BackgroundGradient = GetRandomGradient(),
                    ImageSource = GetImageSourceByCategory(producto.CategoriaNombre)
                };

                var productDisplay1 = new ProductDisplay
                {
                    DataContext = viewModel,
                    Margin = new Thickness(20, 0, 40, 0),
                    Width = 400,
                    Height = 300
                };

                HorizontalStackPanel1.Children.Add(productDisplay1);
            }

            // Shuffle the products list for the second panel
            var shuffledProductos = productos.OrderBy(x => random.Next()).ToList();

            // Adding products to the second panel with rows of 5 products each
            const int productsPerRow = 5;
            for (int i = 0; i < shuffledProductos.Count; i += productsPerRow)
            {
                var rowStackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 0, 20)
                };

                for (int j = i; j < i + productsPerRow && j < shuffledProductos.Count; j++)
                {
                    var producto = shuffledProductos[j];
                    var viewModel = new ProductoDisplay
                    {
                        NombreProducto = producto.NombreProducto,
                        CategoriaNombre = producto.CategoriaNombre,
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

                    rowStackPanel.Children.Add(productDisplay);
                }

                VerticalStackPanel.Children.Add(rowStackPanel);
            }
        }

        private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel stackPanel && e.OriginalSource is not Button)
            {
                _startPoint = e.GetPosition(this);
                _transform = stackPanel.RenderTransform as TranslateTransform ?? new TranslateTransform();
                stackPanel.RenderTransform = _transform;
                _isDragging = true;
                stackPanel.CaptureMouse();
            }
        }

        private void StackPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && sender is StackPanel stackPanel)
            {
                var currentPoint = e.GetPosition(this);
                if (stackPanel.Orientation == Orientation.Horizontal)
                {
                    var offset = currentPoint.X - _startPoint.X;
                    var newTransformX = _transform.X + offset;

                    // Limitar el desplazamiento dentro del contenido
                    var maxOffset = ((FrameworkElement)stackPanel.Parent).ActualWidth - stackPanel.ActualWidth;
                    if (newTransformX < maxOffset)
                    {
                        newTransformX = maxOffset;
                    }
                    if (newTransformX > 0)
                    {
                        newTransformX = 0;
                    }

                    _transform.X = newTransformX;
                    _startPoint = currentPoint; // Reset start point for smoother dragging
                }
                else
                {
                    var offset = currentPoint.Y - _startPoint.Y;
                    var newTransformY = _transform.Y + offset;

                    // Limitar el desplazamiento dentro del contenido
                    var maxOffset = ((FrameworkElement)stackPanel.Parent).ActualHeight - stackPanel.ActualHeight;
                    if (newTransformY < maxOffset)
                    {
                        newTransformY = maxOffset;
                    }
                    if (newTransformY > 0)
                    {
                        newTransformY = 0;
                    }

                    _transform.Y = newTransformY;
                    _startPoint = currentPoint; // Reset start point for smoother dragging
                }
            }
        }

        private void StackPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                _isDragging = false;
                stackPanel.ReleaseMouseCapture();
            }
        }

        private void StackPanel_TouchDown(object sender, TouchEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                _startPoint = e.GetTouchPoint(this).Position;
                _transform = stackPanel.RenderTransform as TranslateTransform ?? new TranslateTransform();
                stackPanel.RenderTransform = _transform;
                _isDragging = true;
                stackPanel.CaptureTouch(e.TouchDevice);
            }
        }

        private void StackPanel_TouchMove(object sender, TouchEventArgs e)
        {
            if (_isDragging && sender is StackPanel stackPanel)
            {
                var currentPoint = e.GetTouchPoint(this).Position;
                if (stackPanel.Orientation == Orientation.Horizontal)
                {
                    var offset = currentPoint.X - _startPoint.X;
                    var newTransformX = _transform.X + offset;

                    // Limitar el desplazamiento dentro del contenido
                    var maxOffset = ((FrameworkElement)stackPanel.Parent).ActualWidth - stackPanel.ActualWidth;
                    if (newTransformX < maxOffset)
                    {
                        newTransformX = maxOffset;
                    }
                    if (newTransformX > 0)
                    {
                        newTransformX = 0;
                    }

                    _transform.X = newTransformX;
                    _startPoint = currentPoint; // Reset start point for smoother dragging
                }
                else
                {
                    var offset = currentPoint.Y - _startPoint.Y;
                    var newTransformY = _transform.Y + offset;

                    // Limitar el desplazamiento dentro del contenido
                    var maxOffset = ((FrameworkElement)stackPanel.Parent).ActualHeight - stackPanel.ActualHeight;
                    if (newTransformY < maxOffset)
                    {
                        newTransformY = maxOffset;
                    }
                    if (newTransformY > 0)
                    {
                        newTransformY = 0;
                    }

                    _transform.Y = newTransformY;
                    _startPoint = currentPoint; // Reset start point for smoother dragging
                }
            }
        }

        private void StackPanel_TouchUp(object sender, TouchEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                _isDragging = false;
                stackPanel.ReleaseTouchCapture(e.TouchDevice);
            }
        }

        private static LinearGradientBrush GetRandomGradient()
        {
            Random rand = new();
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
