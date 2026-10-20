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
