using CapaAccesoDatos;
using System;
using System.Data;
using System.Data.SqlClient;

public class CD_Stock
{
    #region ATRIBUTOS
    private int idStock;
    private string fechaDeVencimiento;
    private string numeroDeLote;
    private int cantidad;
    private int idProveedor;
    private int idProducto;
    #endregion

    #region PROPERTIES
    public int IdStock
    {
        get => idStock;
        set { idStock = value; }
    }

    public string FechaDeVencimiento
    {
        get => fechaDeVencimiento;
        set { fechaDeVencimiento = value; }
    }

    public string NumeroDeLote
    {
        get => numeroDeLote;
        set { numeroDeLote = value; }
    }

    public int Cantidad
    {
        get => cantidad;
        set { cantidad = value; }
    }

    public int IdProveedor
    {
        get => idProveedor;
        set { idProveedor = value; }
    }

    public int IdProducto
    {
        get => idProducto;
        set { idProducto = value; }
    }
    #endregion

    #region METODOS
    public DataTable MostrarStock()
    {
        string sSql = "SELECT * FROM Stock";
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        return Ejecutar.Ejecutar(sSql);
    }

    public void InsertarStock()
    {
        string sSql = $"INSERT INTO Stock (fechaDeVencimiento, numeroDeLote, idProducto, idProveedor, cantidad) " +
                      $"VALUES ('{FechaDeVencimiento}', '{NumeroDeLote}', '{IdProducto}', '{IdProveedor}', '{Cantidad}')";
        Console.WriteLine(sSql);
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }

    public void ModificarStock()
    {
        string sSql = $"UPDATE Stock SET " +
                      $"fechaDeVencimiento = '{FechaDeVencimiento}', " +
                      $"numeroDeLote = '{NumeroDeLote}', " +
                      $"idProducto = '{IdProducto}' " +
                      $"idProveedor = '{IdProveedor}' " +
                      $"cantidad = '{cantidad}' " +
                      $"WHERE idStock = {IdStock}";
        Console.WriteLine(sSql);
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }

    public void EliminarStock()
    {
        string sSql = $"DELETE FROM Stock WHERE idStock = {IdStock}";
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        Ejecutar.Ejecutar(sSql);
    }
    #endregion
}
