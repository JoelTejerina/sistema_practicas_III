using System.Data;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Compras
{
    public class CD_Proveedor
    {
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
        public DataTable Mostrar()
        {
            string sSql = "SELECT * FROM Proveedor ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public void InsertarProveedor()
        {
            string sSql = "INSERT INTO Proveedor " +
                "(Nombre, RazonSocial, Categoria, CUIT, CertificadoAfip, Direccion, Email, Telefono) VALUES (" +
                "'" + T(Nombre) + "','" + T(RazonSocial) + "','" + T(Categoria) + "','" + T(CUIT) + "','" +
                T(CertificadoAfip) + "','" + T(Direccion) + "','" + T(Email) + "','" + T(Telefono) + "')";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarProveedor()
        {
            string sSql = "UPDATE Proveedor SET " +
                "Nombre='" + T(Nombre) + "', RazonSocial='" + T(RazonSocial) + "', Categoria='" + T(Categoria) +
                "', CUIT='" + T(CUIT) + "', CertificadoAfip='" + T(CertificadoAfip) + "', Direccion='" + T(Direccion) +
                "', Email='" + T(Email) + "', Telefono='" + T(Telefono) + "' " +
                "WHERE IdProveedor=" + IdProveedor;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarProveedor()
        {
            string sSql = "DELETE FROM Proveedor WHERE IdProveedor=" + IdProveedor;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        // Duplica las comillas simples para no romper el SQL
        private string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }
        #endregion
    }
}
