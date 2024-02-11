using CapaAccesoDatos.Administrador;
using CapaLogicaNegocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CN_Menu
{
    private CD_Menu cdMenu = new CD_Menu();

    #region ATRIBUTOS
    private int idMenu;
    private string nombre;
    private string region;
    private string categoria;
    private int popularidad;
    private float precio;
    private string temporada;
    private string tipoDeEvento;
    private int tiempoPreparacionEstimado;
    private string descripcion;
    #endregion

    #region PROPERTIES

    public int IdMenu
    {
        get => idMenu;
        set { idMenu = value; }
    }

    public string Nombre
    {
        get => nombre;
        set { nombre = value; }
    }

    public string Region
    {
        get => region;
        set { region = value; }
    }

    public string Categoria
    {
        get => categoria;
        set { categoria = value; }
    }

    public int Popularidad
    {
        get => popularidad;
        set { popularidad = value; }
    }

    public float Precio
    {
        get => precio;
        set { precio = value; }
    }

    public string Temporada
    {
        get => temporada;
        set { temporada = value; }
    }

    public string TipoDeEvento
    {
        get => tipoDeEvento;
        set { tipoDeEvento = value; }
    }

    public int TiempoPreparacionEstimado
    {
        get => tiempoPreparacionEstimado;
        set { tiempoPreparacionEstimado = value; }
    }

    public string Descripcion
    {
        get => descripcion;
        set { descripcion = value; }
    }
    #endregion

    #region METODOS
    public DataTable MostrarMenu()
    {
        DataTable tabla = new DataTable();
        tabla = cdMenu.MostrarMenu();
        return tabla;
    }

    public void InsertarMenu()
    {
        PasarDatos();
        try
        {
            cdMenu.InsertarMenu();
            CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmMenu");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmMenu");
        }
    }

    public void ModificarMenu()
    {
        PasarDatos();
        try
        {
            cdMenu.ModificarMenu();
            CL_clsBitacora Guardar = new CL_clsBitacora("Modificado con exito", "Exitoso", "frmMenu");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmMenu");
        }
    }

    public void EliminarMenu()
    {
        cdMenu.IdMenu = IdMenu;
        try
        {
            cdMenu.EliminarMenu();
            CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmMenu");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmMenu");
        }

    }

    public void obtenerPrecio(string idMenu)
    {
         cdMenu.obtenerPrecio(idMenu);
    }

    private void PasarDatos()
    {
        cdMenu.IdMenu = IdMenu;
        cdMenu.Nombre = Nombre;
        cdMenu.Region = Region;
        cdMenu.Categoria = Categoria;
        cdMenu.Popularidad = Popularidad;
        cdMenu.Precio = Precio;
        cdMenu.Temporada = Temporada;
        cdMenu.TipoDeEvento = TipoDeEvento;
        cdMenu.TiempoPreparacionEstimado = TiempoPreparacionEstimado;
        cdMenu.Descripcion = Descripcion;
    }
    #endregion
}
