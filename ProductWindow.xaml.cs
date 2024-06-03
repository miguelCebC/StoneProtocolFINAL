using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoneProtocol
{
    /// <summary>
    /// Lógica de interacción para ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        public ProductWindow()
        {
            InitializeComponent();
        }

        private void OnBuyButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Producto comprado!");
            this.Close();
        }


    }
}
