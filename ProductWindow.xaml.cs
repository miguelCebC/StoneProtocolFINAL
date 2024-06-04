using System;
using System.Windows;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol
{
    public partial class ProductWindow : Window
    {
        public ProductWindow()
        {
            InitializeComponent();

            // Crear un objeto Producto de ejemplo
            var product = new Producto
            {
                NombreProducto = "Producto de Ejemplo",
                CategoriaNombre = "Categoría de Ejemplo",
                ImageSource = "ruta/a/tu/imagen.jpg" // Asegúrate de que la ruta sea correcta
            };

            // Establecer el contexto de datos
            this.DataContext = product;
        }

        private void OnBuyButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Producto comprado!");
            this.Close();
        }
    }
}
