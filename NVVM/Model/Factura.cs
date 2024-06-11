namespace StoneProtocol.NVVM.Model
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public bool Confirmado { get; set; }
        public bool Enviado { get; set; }
        public string Direccion { get; set; }  // Nueva propiedad
        public List<LineaFactura> LineasFactura { get; set; }
    }

}
