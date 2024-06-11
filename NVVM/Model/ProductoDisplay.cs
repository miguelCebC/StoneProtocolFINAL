
using System.Windows.Media;

namespace StoneProtocol.NVVM.Model
{
    public class ProductoDisplay
    {
        public int Id { get; set; }
        public string NombreProducto { get; set; }
        public string CategoriaNombre { get; set; }
        public Brush BackgroundGradient { get; set; }
        public ImageSource ImageSource { get; set; }
        public double Precio { get; set; }
        public string Descripcion { get; set; }

    }
}
