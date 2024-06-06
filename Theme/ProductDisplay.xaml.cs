using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StoneProtocol.Theme
{
    /// <summary>
    /// Lógica de interacción para ProductDisplay.xaml
    /// </summary>
    public partial class ProductDisplay : UserControl
    {
        public ProductDisplay()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public static readonly DependencyProperty NombreProductoProperty =
            DependencyProperty.Register("NombreProducto", typeof(string), typeof(ProductDisplay), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty CategoriaNombreProperty =
            DependencyProperty.Register("CategoriaNombre", typeof(string), typeof(ProductDisplay), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty BackgroundGradientProperty =
            DependencyProperty.Register("BackgroundGradient", typeof(Brush), typeof(ProductDisplay), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ProductDisplay), new PropertyMetadata(default(ImageSource)));

        public string NombreProducto
        {
            get { return (string)GetValue(NombreProductoProperty); }
            set { SetValue(NombreProductoProperty, value); }
        }

        public string CategoriaNombre
        {
            get { return (string)GetValue(CategoriaNombreProperty); }
            set { SetValue(CategoriaNombreProperty, value); }
        }

        public Brush BackgroundGradient
        {
            get { return (Brush)GetValue(BackgroundGradientProperty); }
            set { SetValue(BackgroundGradientProperty, value); }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is NVVM.Model.ProductoDisplay viewModel)
            {
                NombreProducto = viewModel.NombreProducto;
                CategoriaNombre = viewModel.CategoriaNombre;
                BackgroundGradient = viewModel.BackgroundGradient;
                ImageSource = viewModel.ImageSource;
            }
        }

        private void OnBuyButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Comprando");
            var productWindow = new StoneProtocol.ProductWindow
            {
                DataContext = this.DataContext
            };
            productWindow.ShowDialog();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
