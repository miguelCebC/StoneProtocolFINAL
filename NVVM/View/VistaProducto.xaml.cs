using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaProducto : UserControl
    {
        private ProductoRepository _productoRepository;
        private Producto _selectedProducto;

        public VistaProducto()
        {
            InitializeComponent();
            _productoRepository = new ProductoRepository();
            LoadProducts();
            LoadCategories();
        }

        private void LoadProducts()
        {
            ProductsDataGrid.ItemsSource = _productoRepository.GetAllProductos();
        }

        private void LoadCategories()
        {
            CategoryComboBox.Items.Add("Smartphones");
            CategoryComboBox.Items.Add("Laptops");
            CategoryComboBox.Items.Add("Tablets");
            CategoryComboBox.Items.Add("Accessories");
            CategoryComboBox.Items.Add("Other");
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Producto selectedProducto)
            {
                _selectedProducto = selectedProducto;
                ProductNameTextBox.Text = _selectedProducto.NombreProducto;
                CategoryComboBox.SelectedItem = _selectedProducto.CategoriaNombre;
                ProductImage.Source = GetImageSourceByCategory(_selectedProducto.CategoriaNombre);
            }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducto != null)
            {
                _selectedProducto.NombreProducto = ProductNameTextBox.Text;
                _selectedProducto.CategoriaNombre = CategoryComboBox.SelectedItem as string;

                _productoRepository.UpdateProducto(_selectedProducto);
                MessageBox.Show($"Producto actualizado:\nNombre: {_selectedProducto.NombreProducto}\nCategoría: {_selectedProducto.CategoriaNombre}");
                LoadProducts(); // Refrescar la lista de productos
            }
            else
            {
                MessageBox.Show("Seleccione un producto para actualizar.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducto != null)
            {
                _productoRepository.DeleteProducto(_selectedProducto.Id);
                MessageBox.Show($"Producto eliminado:\nNombre: {_selectedProducto.NombreProducto}");
                _selectedProducto = null;
                LoadProducts(); // Refrescar la lista de productos
            }
            else
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newProducto = new Producto
            {
                NombreProducto = ProductNameTextBox.Text,
                CategoriaNombre = CategoryComboBox.SelectedItem as string
            };

            _productoRepository.AddProducto(newProducto);
            MessageBox.Show($"Producto añadido:\nNombre: {newProducto.NombreProducto}\nCategoría: {newProducto.CategoriaNombre}");
            LoadProducts(); // Refrescar la lista de productos
        }
    }
}
