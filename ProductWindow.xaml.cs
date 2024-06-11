using System.Windows;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol
{
    public partial class ProductWindow : Window
    {
        private readonly FacturaRepository _facturaRepository;

        public ProductWindow()
        {
            InitializeComponent();
            _facturaRepository = new FacturaRepository();
        }

        private void OnBuyButtonClick(object sender, RoutedEventArgs e)
        {
            // Obtener el producto del DataContext
            var producto = DataContext as ProductoDisplay;
            if (producto == null)
            {
                MessageBox.Show("Producto no está inicializado correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar si hay una factura seleccionada en AppState
            if (AppState.FacturaId == 0)
            {
                MessageBox.Show("No hay ninguna factura seleccionada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Crear una nueva línea de factura
            var lineaFactura = new LineaFactura
            {
                FacturaId = AppState.FacturaId,
                ProductoId = producto.Id,
                Cantidad = 1, // Puedes permitir al usuario especificar la cantidad si es necesario
                PrecioUnitario = (decimal)producto.Precio
            };

            // Añadir la línea de factura a la base de datos
            try
            {
                _facturaRepository.AddLineaFactura(lineaFactura);
                MessageBox.Show("Producto añadido a la factura!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al añadir el producto a la factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Cerrar la ventana
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
