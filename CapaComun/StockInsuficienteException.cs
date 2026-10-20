using System;

namespace CapaComun
{
    /// <summary>
    /// El stock actual no alcanza para preparar un pedido. El mensaje ya viene listo para
    /// mostrarse al usuario (incluye que ingredientes faltan y cuanto se puede producir),
    /// por lo que no es un error del sistema y no debe registrarse como tal.
    /// </summary>
    public class StockInsuficienteException : Exception
    {
        public StockInsuficienteException(string mensaje) : base(mensaje)
        {
        }
    }
}
