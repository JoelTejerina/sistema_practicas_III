using CapaComun;
using System;
using System.Data;
using CapaAccesoDatos.Ventas;

namespace CapaLogicaNegocio.Ventas
{
    public class CN_Menu
    {
        private CD_Menu menu = new CD_Menu();

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
        public DataTable MostrarMenu()
        {
            return menu.Mostrar();
        }

        public void InsertarMenu()
        {
            PasarDatos();
            menu.InsertarMenu();
        }

        public void ModificarMenu()
        {
            PasarDatos();
            menu.ModificarMenu();
        }

        public void CambiarDisponibilidad(int idMenu, bool disponible)
        {
            menu.CambiarDisponibilidad(idMenu, disponible);
        }

        public DataTable Categorias()
        {
            return menu.Categorias();
        }

        public int CantidadAfectados(string categoria)
        {
            return menu.CantidadAfectados(categoria);
        }

        /// <summary>Actualizacion masiva de precios: de -90% a +300%.</summary>
        public int ActualizarPrecios(decimal porcentaje, string categoria)
        {
            if (porcentaje == 0 || porcentaje < -90 || porcentaje > 300)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_PorcentajeInvalido"));
            }
            return menu.ActualizarPrecios(porcentaje, categoria);
        }

        public void EliminarMenu()
        {
            menu.IdMenu = IdMenu;
            menu.EliminarMenu();
        }

        private void PasarDatos()
        {
            menu.IdMenu = IdMenu;
            menu.Nombre = Nombre;
            menu.Descripcion = Descripcion;
            menu.Categoria = Categoria;
            menu.Ingredientes = Ingredientes;
            menu.Precio = Precio;
            menu.Region = Region;
            menu.Temporada = Temporada;
            menu.Popularidad = Popularidad;
            menu.TiempoPreparacion = TiempoPreparacion;
            menu.TipoEvento = TipoEvento;
            menu.IdStock = IdStock;
        }
        #endregion
    }
}
