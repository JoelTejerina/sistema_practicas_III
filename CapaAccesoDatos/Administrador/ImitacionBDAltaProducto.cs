using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.Imitacion_de_una_BD
{
    public class ImitacionBDAltaProducto
    {
        #region ATRIBUTOS
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public string TipoCantidadA { get; set; }
        public string TipoCantidadB { get; set; }
        public int CantidadA { get; set; }
        public int CantidadB { get; set; }
        #endregion

    }
}
