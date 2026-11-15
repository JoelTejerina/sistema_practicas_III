using System;
using System.Data;
using CapaAccesoDatos.Ventas;
using CapaComun;

namespace CapaLogicaNegocio.Ventas
{
    /// <summary>Receta de un platillo: ingredientes principales y secundarios con su cantidad.</summary>
    public class CN_Receta
    {
        private CD_Receta receta = new CD_Receta();

        public DataTable Listar(int idMenu)
        {
            return receta.Listar(idMenu);
        }

        public DataTable ProductosDisponibles()
        {
            return receta.ProductosDisponibles();
        }

        /// <summary>Cuantas unidades de cada platillo se pueden preparar con el stock actual.</summary>
        public DataTable Capacidad()
        {
            return receta.Capacidad();
        }

        /// <summary>
        /// Busca platillos (CU-007-001). Agrega la columna Capacidad (unidades que se pueden preparar ahora;
        /// vacia si el platillo no tiene receta) y, si se pide, deja solo los que se pueden preparar.
        /// </summary>
        public DataTable Buscar(int idProducto, string region, string categoria, string popularidad, string temporada,
            string tipoEvento, decimal? precioMin, decimal? precioMax, int? tiempoMax, bool soloPreparables)
        {
            if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_RangoPrecioInvalido"));
            }

            DataTable resultado = receta.Buscar(idProducto, region, categoria, popularidad, temporada, tipoEvento, precioMin, precioMax, tiempoMax);
            DataTable capacidad = receta.Capacidad();
            resultado.Columns.Add("Capacidad", typeof(int));
            for (int i = resultado.Rows.Count - 1; i >= 0; i--)
            {
                DataRow[] cap = capacidad.Select("IdMenu = " + resultado.Rows[i]["IdMenu"]);
                object valor = cap.Length == 0 ? DBNull.Value : cap[0]["Capacidad"];
                resultado.Rows[i]["Capacidad"] = valor;
                if (soloPreparables && (valor == DBNull.Value || Convert.ToInt32(valor) <= 0))
                {
                    resultado.Rows.RemoveAt(i);
                }
            }
            return resultado;
        }

        public void Guardar(int idMenu, int idProducto, decimal cantidad, bool principal)
        {
            if (cantidad <= 0)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CantidadInvalida"));
            }

            // Todos los ingredientes deben tener su medida (ml/gr) para poder calcular el consumo.
            foreach (DataRow fila in receta.ProductosDisponibles().Rows)
            {
                if (Convert.ToInt32(fila["IdProducto"]) == idProducto && Convert.ToString(fila["Medida"]).Trim().Length == 0)
                {
                    throw new InvalidOperationException(Idioma.Texto("Msg_IngredienteSinMedida"));
                }
            }
            receta.Guardar(idMenu, idProducto, cantidad, principal);
        }

        public void Quitar(int idMenu, int idProducto)
        {
            receta.Quitar(idMenu, idProducto);
        }
    }
}
