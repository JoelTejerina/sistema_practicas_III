using System;
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
            string sSql = "SELECT * FROM Menu WHERE Activo = 1 ORDER BY Nombre";
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

        /// <summary>Muestra u oculta un platillo (no disponible = no se puede pedir) sin darlo de baja.</summary>
        public void CambiarDisponibilidad(int idMenu, bool disponible)
        {
            new clsEjecutarComando().EjecucionDirecta("UPDATE Menu SET Disponible = " + (disponible ? 1 : 0) + " WHERE IdMenu = " + idMenu);
        }

        /// <summary>Categorias de los platillos activos (para actualizar precios por categoria).</summary>
        public DataTable Categorias()
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT DISTINCT Categoria FROM Menu WHERE Activo = 1 AND ISNULL(Categoria, N'') <> N'' ORDER BY Categoria");
        }

        /// <summary>Cuantos platillos activos alcanza un cambio de precios (todos o de una categoria).</summary>
        public int CantidadAfectados(string categoria)
        {
            string filtro = string.IsNullOrEmpty(categoria) ? "" : " AND Categoria = N'" + T(categoria) + "'";
            return Convert.ToInt32(new clsEjecutarComando().EjecutarEscalar("SELECT COUNT(*) FROM Menu WHERE Activo = 1" + filtro));
        }

        /// <summary>Sube o baja los precios un porcentaje (redondeado a 2 decimales). Devuelve cuantos platillos cambio.</summary>
        public int ActualizarPrecios(decimal porcentaje, string categoria)
        {
            string filtro = string.IsNullOrEmpty(categoria) ? "" : " AND Categoria = N'" + T(categoria) + "'";
            object r = new clsEjecutarComando().EjecutarEscalar(
                "UPDATE Menu SET Precio = ROUND(Precio * (1 + (" + Num(porcentaje) + ") / 100.0), 2) WHERE Activo = 1" + filtro +
                "; SELECT @@ROWCOUNT");
            return Convert.ToInt32(r);
        }

        public void EliminarMenu()
        {
            string sSql = "UPDATE Menu SET Activo = 0 WHERE IdMenu=" + IdMenu;
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
