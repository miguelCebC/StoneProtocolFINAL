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

            DataContext = DataContext; 
        }

        private void OnBuyButtonClick(object sender, RoutedEventArgs e)
        {
          /*  if (_factura == null || _producto == null)
            {
                MessageBox.Show("Factura o producto no están inicializados correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var lineaFactura = new LineaFactura
            {
                FacturaId = _factura.Id,
                ProductoId = _producto.Id,
                Cantidad = 1, // Puedes permitir al usuario especificar la cantidad si es necesario
                PrecioUnitario = (decimal)_producto.Precio,
                Producto = _producto
            };

            _factura.LineasFactura.Add(lineaFactura);
            MessageBox.Show("Producto añadido a la factura!");*/
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
