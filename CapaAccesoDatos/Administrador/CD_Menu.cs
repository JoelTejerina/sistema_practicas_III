using CapaAccesoDatos;
using System;
using System.Data;
using System.Data.SqlClient;

public class CD_Menu
{
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
        string sSql = "SELECT * FROM Menu";
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        return Ejecutar.Ejecutar(sSql);
    }

    public void InsertarMenu()
    {
        string sSql = $"INSERT INTO Menu (nombre, region, categoria, popularidad, precio, temporada, tipoDeEvento, tiempoPreparacionEstimado, descripcion) " +
                      $"VALUES ('{Nombre}', '{Region}', '{Categoria}', {Popularidad}, {Precio}, '{Temporada}', '{TipoDeEvento}', {TiempoPreparacionEstimado}, '{Descripcion}')";
        Console.WriteLine(sSql);
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }

    public void ModificarMenu()
    {
        string sSql = $"UPDATE Menu SET " +
                      $"nombre = '{Nombre}', region = '{Region}', categoria = '{Categoria}', popularidad = {Popularidad}, " +
                      $"precio = {Precio}, temporada = '{Temporada}', tipoDeEvento = '{TipoDeEvento}', " +
                      $"tiempoPreparacionEstimado = {TiempoPreparacionEstimado}, descripcion = '{Descripcion}' " +
                      $"WHERE idMenu = {IdMenu}";
        Console.WriteLine(sSql);
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }

    public void EliminarMenu()
    {
        string sSql = $"DELETE FROM Menu WHERE idMenu = {IdMenu}";
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }

    public void obtenerPrecio(string idMenu)
    {
        string sSql = $"SELECT precio FROM Menu WHERE idMenu = {idMenu}";
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }
    #endregion
}
