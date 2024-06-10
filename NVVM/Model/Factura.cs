namespace StoneProtocol.NVVM.Model
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public List<LineaFactura> LineasFactura { get; set; }
        public bool Confirmado { get; set; } = false; // Nuevo campo
        public bool Enviado { get; set; } = false;    // Nuevo campo
    }
}
