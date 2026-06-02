using System.Data;
using System.Globalization;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Ventas
{
    public class CD_Menu
    {
        #region PROPERTIES
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string Ingredientes { get; set; }
        public decimal Precio { get; set; }
        public string Region { get; set; }
        public string Temporada { get; set; }
        public string Popularidad { get; set; }
        public int TiempoPreparacion { get; set; }
        public string TipoEvento { get; set; }
        public int IdStock { get; set; }
        #endregion

        #region METODOS
        public DataTable Mostrar()
        {
            string sSql = "SELECT * FROM Menu ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public void InsertarMenu()
        {
            string sSql = "INSERT INTO Menu " +
                "(Nombre, Descripcion, Categoria, Ingredientes, Precio, Region, Temporada, Popularidad, TiempoPreparacion, TipoEvento, IdStock) VALUES (" +
                "'" + T(Nombre) + "','" + T(Descripcion) + "','" + T(Categoria) + "','" + T(Ingredientes) + "'," +
                Num(Precio) + ",'" + T(Region) + "','" + T(Temporada) + "','" + T(Popularidad) + "'," +
                TiempoPreparacion + ",'" + T(TipoEvento) + "'," + IdStock + ")";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarMenu()
        {
            string sSql = "UPDATE Menu SET " +
                "Nombre='" + T(Nombre) + "', Descripcion='" + T(Descripcion) + "', Categoria='" + T(Categoria) +
                "', Ingredientes='" + T(Ingredientes) + "', Precio=" + Num(Precio) + ", Region='" + T(Region) +
                "', Temporada='" + T(Temporada) + "', Popularidad='" + T(Popularidad) + "', TiempoPreparacion=" + TiempoPreparacion +
                ", TipoEvento='" + T(TipoEvento) + "', IdStock=" + IdStock + " " +
                "WHERE IdMenu=" + IdMenu;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarMenu()
        {
            string sSql = "DELETE FROM Menu WHERE IdMenu=" + IdMenu;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        private string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        private string Num(decimal valor)
        {
            return valor.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
