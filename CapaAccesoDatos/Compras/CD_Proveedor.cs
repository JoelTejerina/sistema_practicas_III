using System.Data;
using System.Linq;
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
            string sSql = "SELECT * FROM Proveedor WHERE Activo = 1 ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        /// <summary>0 = no hay duplicado, 1 = otro proveedor activo con el mismo CUIT, 2 = con el mismo nombre.</summary>
        public int Duplicado(string cuit, string nombre, int excluirIdProveedor)
        {
            string soloCuit = new string((cuit ?? "").Where(char.IsDigit).ToArray());
            string sSql = "SELECT TOP 1 CASE WHEN REPLACE(REPLACE(ISNULL(CUIT, ''), '-', ''), ' ', '') = '" + soloCuit + "' THEN 1 ELSE 2 END " +
                "FROM Proveedor WHERE Activo = 1 AND IdProveedor <> " + excluirIdProveedor + " AND " +
                "(REPLACE(REPLACE(ISNULL(CUIT, ''), '-', ''), ' ', '') = '" + soloCuit + "' OR Nombre = N'" + T(nombre).Trim() + "') " +
                "ORDER BY CASE WHEN REPLACE(REPLACE(ISNULL(CUIT, ''), '-', ''), ' ', '') = '" + soloCuit + "' THEN 1 ELSE 2 END";
            DataTable dt = new clsEjecutarComando().Ejecutar(sSql);
            return dt.Rows.Count == 0 ? 0 : System.Convert.ToInt32(dt.Rows[0][0]);
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
            string sSql = "UPDATE Proveedor SET Activo = 0 WHERE IdProveedor=" + IdProveedor;
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
