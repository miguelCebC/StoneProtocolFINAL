using System.Windows;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol
{
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
