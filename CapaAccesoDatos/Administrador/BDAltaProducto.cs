﻿using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.Imitacion_de_una_BD
{
    public class BDAltaProducto
    {


        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidada;
        private string tipocantidadb;
        private int cantidada;
        private int cantidadb;
        private int idproducto;
        #endregion

        #region PROPERTIES

        public int IdPersona
        {
            get => idpersona; //Expresion Lambda (Se suprime el Return y las llaves)
            set { idpersona = value; }
        }

        public string Apellido
        {
            get => apellido;
            set { apellido = value; }
        }

        public string Nombre
        {
            get => nombre;
            set { nombre = value; }
        }

        public int TipoDoc
        {
            get => tipodoc;
            set { tipodoc = value; }
        }

        public int NroDoc
        {
            get => nrodoc;
            set { nrodoc = value; }
        }

        public string Telefono
        {
            get => telefono;
            set { telefono = value; }
        }

        public string Correo
        {
            get => correo;
            set { correo = value; }
        }

        public string Calle
        {
            get => calle;
            set { calle = value; }
        }

        public string Nro
        {
            get => nro;
            set { nro = value; }
        }

        public string Piso
        {
            get => piso;
            set { piso = value; }
        }

        public string Dto
        {
            get => dto;
            set { dto = value; }
        }

        public int IdLocalidad
        {
            get => idlocalidad;
            set { idlocalidad = value; }
        }

        public int IdProvincia
        {
            get => idprovincia;
            set { idprovincia = value; }
        }
        #endregion

        #region METODOS

        public DataTable Mostrar()
        {
            string sSql;
            sSql = "Select * from Personal ";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);

        }

        public void InsertarPersona()
        {
            string sSql = "INSERT INTO Personal " +
               "(Apellido, Nombres, IdTDoc, NroDoc, Telefono, Correo, Calle, Nro, Piso, Dto, IdLocalidad, IdCargo) " +
                "values" +
                " ('" + apellido + "','" + nombre + "'," + tipodoc + "," + nrodoc +
                ",'" + telefono + "','" + correo + "','" + calle + "','" + nro +
                "','" + piso + "','" + dto + "'," + idlocalidad + ", 6 )";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarPersona()
        {
            string sSql = "UPDATE Personal set " +
                "Apellido='" + apellido + "', Nombres='" + nombre + "', IdTDoc =" + tipodoc +
                ", NroDoc = " + nrodoc + ", Telefono = '" + telefono + "', Correo = '" + correo +
                "', Calle = '" + calle + "', Nro = '" + nro + "', Piso = '" + piso + "', Dto = '" + dto +
                "', IdLocalidad = " + idlocalidad +
                " WHERE IdPersona =" + idpersona;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarPersona()
        {
            string sSql = "DELETE FROM Personal WHERE IdPersona =" + idpersona;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        #endregion
    }
}
