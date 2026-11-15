using System;
using System.Data;
using CapaComun;
using CapaAccesoDatos.Compras;

namespace CapaLogicaNegocio.Compras
{
    public class CN_Proveedor
    {
        private CD_Proveedor proveedor = new CD_Proveedor();

        #region PROPERTIES
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Categoria { get; set; }
        public string CUIT { get; set; }
        public string CertificadoAfip { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarProveedor()
        {
            return proveedor.Mostrar();
        }

        // CUIT valido (digito verificador) y sin duplicados: lo que el sistema puede controlar de las "normas legales".
        private void Validar()
        {
            if (!ValidadorCuit.EsValido(CUIT))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CuitInvalido"));
            }
            switch (proveedor.Duplicado(CUIT, Nombre, IdProveedor))
            {
                case 1:
                    throw new InvalidOperationException(Idioma.Texto("Msg_ProveedorCuitDuplicado"));
                case 2:
                    throw new InvalidOperationException(Idioma.Texto("Msg_ProveedorNombreDuplicado"));
            }
        }

        public void InsertarProveedor()
        {
            IdProveedor = 0;
            Validar();
            PasarDatos();
            proveedor.InsertarProveedor();
        }

        public void ModificarProveedor()
        {
            Validar();
            PasarDatos();
            proveedor.ModificarProveedor();
        }

        public void EliminarProveedor()
        {
            proveedor.IdProveedor = IdProveedor;
            proveedor.EliminarProveedor();
        }

        private void PasarDatos()
        {
            proveedor.IdProveedor = IdProveedor;
            proveedor.Nombre = Nombre;
            proveedor.RazonSocial = RazonSocial;
            proveedor.Categoria = Categoria;
            proveedor.CUIT = CUIT;
            proveedor.CertificadoAfip = CertificadoAfip;
            proveedor.Direccion = Direccion;
            proveedor.Email = Email;
            proveedor.Telefono = Telefono;
        }
        #endregion
    }
}
