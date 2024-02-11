using System;
using System.Data;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Facturacion
    {
        private CD_Facturacion cdFacturacion = new CD_Facturacion();

        #region ATRIBUTOS
        private int idFacturacion;
        private string idMenu;
        private string nombreDelCliente;
        private int cantidades;
        private string formaDePago;
        private float precioTotal;
        #endregion

        #region PROPERTIES

        public int IdFacturacion
        {
            get => idFacturacion;
            set { idFacturacion = value; }
        }

        public string IdMenu
        {
            get => idMenu;
            set { idMenu = value; }
        }

        public string NombreDelCliente
        {
            get => nombreDelCliente;
            set { nombreDelCliente = value; }
        }

        public int Cantidades
        {
            get => cantidades;
            set { cantidades = value; }
        }

        public string FormaDePago
        {
            get => formaDePago;
            set { formaDePago = value; }
        }

        public float PrecioTotal
        {
            get => precioTotal;
            set { precioTotal = value; }
        }
        #endregion

        #region METODOS

        public DataTable MostrarFacturacion()
        {
            DataTable tabla = new DataTable();
            tabla = cdFacturacion.MostrarFacturacion();
            return tabla;
        }

        public void InsertarFacturacion()
        {
            PasarDatos();
            try
            {
                cdFacturacion.InsertarFacturacion();
                CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmFacturacion");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmFacturacion");
            }
        }

        public void ModificarFacturacion()
        {
            PasarDatos();
            try
            {
                cdFacturacion.ModificarFacturacion();
                CL_clsBitacora Guardar = new CL_clsBitacora("Modificado con exito", "Exitoso", "frmFacturacion");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmFacturacion");
            }
        }

        public void EliminarFacturacion()
        {
            cdFacturacion.IdFacturacion = IdFacturacion;
            try
            {
                cdFacturacion.EliminarFacturacion();
                CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmFacturacion");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmFacturacion");
            }
        }

        private void PasarDatos()
        {
            cdFacturacion.IdFacturacion = idFacturacion;
            cdFacturacion.IdMenu = Convert.ToInt32(IdMenu);
            cdFacturacion.NombreDelCliente = NombreDelCliente;
            cdFacturacion.Cantidades = Cantidades;
            cdFacturacion.FormaDePago = FormaDePago;
            cdFacturacion.PrecioTotal = PrecioTotal;
        }

        #endregion
    }
}
