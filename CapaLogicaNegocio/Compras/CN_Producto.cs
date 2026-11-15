using System;
using System.Data;
using System.Text.RegularExpressions;
using CapaComun;
using CapaAccesoDatos.Compras;

namespace CapaLogicaNegocio.Compras
{
    public class CN_Producto
    {
        private CD_Producto producto = new CD_Producto();

        #region PROPERTIES
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string Medida { get; set; }
        public int IdProveedor { get; set; }
        public int CantidadInicial { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarProducto()
        {
            return producto.Mostrar();
        }

        // Nombre, marca y categoria: solo letras, numeros y signos comunes. No se repite el mismo producto.
        private static readonly Regex TextoPermitido = new Regex(@"^[\p{L}\p{N} .,'\-/()&%+°]+$", RegexOptions.Compiled);

        private void Validar()
        {
            foreach (string valor in new[] { Nombre, Marca, Categoria })
            {
                if (!string.IsNullOrWhiteSpace(valor) && !TextoPermitido.IsMatch(valor.Trim()))
                {
                    throw new InvalidOperationException(Idioma.Texto("Msg_CaracteresInvalidos"));
                }
            }
            if (CantidadInicial < 0)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CantidadInvalida"));
            }
            PasarDatos();
            if (producto.Duplicado())
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_ProductoDuplicado"));
            }
        }

        public void InsertarProducto()
        {
            IdProducto = 0;
            Validar();
            PasarDatos();
            producto.InsertarProducto();
        }

        public void ModificarProducto()
        {
            Validar();
            PasarDatos();
            producto.ModificarProducto();
        }

        public void EliminarProducto()
        {
            producto.IdProducto = IdProducto;
            producto.EliminarProducto();
        }

        private void PasarDatos()
        {
            producto.IdProducto = IdProducto;
            producto.Nombre = Nombre;
            producto.Descripcion = Descripcion;
            producto.Marca = Marca;
            producto.Categoria = Categoria;
            producto.Medida = Medida;
            producto.IdProveedor = IdProveedor;
            producto.CantidadInicial = CantidadInicial;
        }
        #endregion
    }
}
