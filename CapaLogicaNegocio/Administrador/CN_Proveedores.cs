using System;
using System.Data;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Proveedores
    {
        private CD_Proveedores cdProveedor = new CD_Proveedores();

        #region ATRIBUTOS
        private int idProveedor;
        private string nombre;
        private string razonSocial;
        private string cuit;
        private string direccionDeEmail;
        private string telefono;
        private string certificadoAfip;
        private string categoria;
        private string producto;
        #endregion

        #region PROPERTIES
        public int IdProveedor
        {
            get => idProveedor;
            set { idProveedor = value; }
        }

        public string Nombre
        {
            get => nombre;
            set { nombre = value; }
        }

        public string RazonSocial
        {
            get => razonSocial;
            set { razonSocial = value; }
        }

        public string Cuit
        {
            get => cuit;
            set { cuit = value; }
        }

        public string DireccionDeEmail
        {
            get => direccionDeEmail;
            set { direccionDeEmail = value; }
        }

        public string Telefono
        {
            get => telefono;
            set { telefono = value; }
        }

        public string CertificadoAfip
        {
            get => certificadoAfip;
            set { certificadoAfip = value; }
        }

        public string Categoria
        {
            get => categoria;
            set { categoria = value; }
        }

        public string Producto
        {
            get => producto;
            set { producto = value; }
        }
        #endregion

        #region METODOS
        public DataTable MostrarProveedores()
        {
            DataTable tabla = new DataTable();
            tabla = cdProveedor.MostrarProveedores();
            return tabla;
        }

        public void InsertarProveedor()
        {
            PasarDatos();
            try
            {
                cdProveedor.InsertarProveedor();
                CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmProveedores");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProveedores");
            }
        }

        public void ModificarProveedor()
        {
            PasarDatos();
            try
            {
                cdProveedor.ModificarProveedor();
                CL_clsBitacora Guardar = new CL_clsBitacora("Modificacion con exito", "Exitoso", "frmProveedores");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProveedores");
            }
        }

        public void EliminarProveedor()
        {
            cdProveedor.IdProveedor = idProveedor;
            try
            {
                cdProveedor.EliminarProveedor();
                CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmProveedores");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProveedores");
            }

        }

        private void PasarDatos()
        {
            cdProveedor.IdProveedor = idProveedor;
            cdProveedor.Nombre = nombre;
            cdProveedor.RazonSocial = razonSocial;
            cdProveedor.Cuit = cuit;
            cdProveedor.DireccionDeEmail = direccionDeEmail;
            cdProveedor.Telefono = telefono;
            cdProveedor.CertificadoAfip = certificadoAfip;
            cdProveedor.Categoria = categoria;
            cdProveedor.Producto = producto;
        }
        #endregion
    }
}
