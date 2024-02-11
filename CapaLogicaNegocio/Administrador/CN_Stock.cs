using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Stock
    {
        private CD_Stock cdStock = new CD_Stock();

        #region ATRIBUTOS
        private int idStock;
        private string fechaDeVencimiento;
        private string numeroDeLote;
        private int cantidad;
        private string idProveedor;
        private string idProducto;
        #endregion

        #region PROPERTIES
        public int IdStock
        {
            get => idStock;
            set { idStock = value; }
        }

        public string FechaDeVencimiento
        {
            get => fechaDeVencimiento;
            set { fechaDeVencimiento = value; }
        }

        public string NumeroDeLote
        {
            get => numeroDeLote;
            set { numeroDeLote = value; }
        }

        public int Cantidad
        {
            get => cantidad;
            set { cantidad = value; }
        }
        public string IdProveedor
        {
            get => idProveedor;
            set { idProveedor = value; }
        }

        public string IdProducto
        {
            get => idProducto;
            set { idProducto = value; }
        }
        #endregion

        #region METODOS
        public DataTable MostrarStock()
        {
            DataTable tabla = new DataTable();
            tabla = cdStock.MostrarStock();
            return tabla;
        }

        public void InsertarStock()
        {
            PasarDatos();
            try
            {
                cdStock.InsertarStock();
                CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmStock");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmStock");
            }
        }

        public void ModificarStock()
        {
            PasarDatos();
            try
            {
                cdStock.ModificarStock();
                CL_clsBitacora Guardar = new CL_clsBitacora("Modificado con exito", "Exitoso", "frmStock");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmStock");
            }
        }

        public void EliminarStock()
        {
            cdStock.IdStock = IdStock;
            try
            {
                cdStock.EliminarStock();
                CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmStock");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmStock");
            }
        }

        private void PasarDatos()
        {
            cdStock.IdStock = idStock;
            cdStock.FechaDeVencimiento = FechaDeVencimiento;
            cdStock.NumeroDeLote = NumeroDeLote;
            cdStock.Cantidad = Cantidad;
            cdStock.IdProducto = Convert.ToInt32(IdProducto);
            cdStock.IdProveedor = Convert.ToInt32(IdProveedor);

        }
        #endregion
    }

}
