using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using StoneProtocol.NVVM.Model;
using StoneProtocol.NVVM.ViewModel;
using StoneProtocol.Theme;
namespace StoneProtocol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += new EventHandler(Window_SourceInitialized);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    Point mousePosition = e.GetPosition(this);
                    this.WindowState = WindowState.Normal;
                    this.Top = mousePosition.Y - (mousePosition.Y / this.ActualHeight * this.RestoreBounds.Height);
                    this.Left = mousePosition.X - (mousePosition.X / this.ActualWidth * this.RestoreBounds.Width);
                }
                this.DragMove();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    Point mousePosition = e.GetPosition(this);
                    this.WindowState = WindowState.Normal;
                    this.Top = mousePosition.Y - (mousePosition.Y / this.ActualHeight * this.RestoreBounds.Height);
                    this.Left = mousePosition.X - (mousePosition.X / this.ActualWidth * this.RestoreBounds.Width);
                }
                this.DragMove();
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual((Visual)sender);
            hwndSource.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0084: // WM_NCHITTEST
                    handled = false;
                    break;
            }
            return IntPtr.Zero;
        }


        public void HandleLogin(Usuario usuario)
        {
            if (usuario.Admin == true) {
                btnHome.Visibility = Visibility.Visible; 
                btnProductos.Visibility = Visibility.Visible;
                btnPepe.Visibility = Visibility.Visible;

                //btnTienda.Visibility = Visibility.Visible;
                //btnPepe.Visibility = Visibility.Visible;

            }
            else
            {
                btnHome.Visibility = Visibility.Visible;
                btnCesta.Visibility = Visibility.Visible;

                btnProductos.Visibility = Visibility.Hidden;
                btnTienda.Visibility = Visibility.Hidden;

                btnPepe.Visibility = Visibility.Hidden;
            }
            InvokeHomeCommand();


        }
        public void InvokeHomeCommand()
        {
            var viewModelPrincipal = (ViewModelPrincipal)this.DataContext;
            if (viewModelPrincipal.ShowHomeViewCommand.CanExecute(null))
            {
                viewModelPrincipal.ShowHomeViewCommand.Execute(null);
            }
        }
    }
        }

    
