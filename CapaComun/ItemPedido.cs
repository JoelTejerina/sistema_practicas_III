namespace CapaComun
{
    /// <summary>Una linea del detalle de un pedido: un platillo y cuantas unidades se piden.</summary>
    public class ItemPedido
    {
        public int IdMenu { get; set; }
        public string Platillo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }
    }
}
