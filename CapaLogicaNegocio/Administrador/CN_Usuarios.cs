using System;
using System.Collections.Generic;
using System.Data;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Usuarios
    {
        private CD_Usuarios usuarios = new CD_Usuarios();

        #region ATRIBUTOS
        private int idUsuario;
        private string usuario;
        private string password;
        private int idPersona;
        private DateTime fechaAlta;// modificar tipo
        private DateTime fechaBaja;// modificar tipo
        private int cambiaCada;
        private DateTime fechaUltimoCambio;// modificar tipo
        private bool usuarioDesactivado;
        private DateTime fechaDesactivacion;// modificar tipo
        #endregion

        #region PROPERTIES

        public int IdUsuario
        {   get; set;  }

        public string Usuario
        { get; set; }

        public string Password
        { get; set; }

        public int IdPersona
        { get; set; }

        public DateTime FechaAlta
        { get; set; }

        public DateTime FechaBaja
        { get; set; }

        public int CambiaCada
        { get; set; }

        public DateTime UltimoCambio
        { get; set; }

        public bool UsuarioDesactivado
        { get; set; }

        public DateTime FechaDesativacion
        { get; set; }

        #endregion
        
        #region METODOS

        public DataTable MostrarUsuario()
        {
            DataTable tabla = new DataTable();
            tabla = usuarios.Mostrar();
            return tabla;
        }

        public void InsertarPersona()
        {
            //PasarDatos();
            //usuarios.InsertarUsuario();
        }

        public void ModificarUsuario()
        {
            //PasarDatos();
            //usuarios.ModificarUsuario();
        }

        public void EliminarUsuario()
        {
            usuarios.IdUsuario = idUsuario;
            usuarios.EliminarUsuario();
        }
        
        private void PasarDatos()
        {
            usuarios.IdUsuario = idUsuario;
            usuarios.Usuario = this.usuario;
            usuarios.Password = this.password;
            usuarios.IdPersona = Convert.ToInt32(this.idPersona);

            //Fecha de Alta
            if (this.fechaAlta == null)
            {   /*usuarios.FechaAlta = 0;*/  }
            else
            {   usuarios.FechaAlta = this.fechaAlta; }

            //Fecha de baja
            if (this.fechaBaja == null)
            { /*usuarios.FechaBaja = 0; */}
            else
            { usuarios.FechaBaja = this.fechaBaja; }

            //Cambia cada
            if (this.cambiaCada == null)
            { usuarios.CambiaCada = 0; }
            else
            { usuarios.CambiaCada = Convert.ToInt16(this.cambiaCada); }
            //usuarios.Telefono = this.telefono;
            // usuarios.Correo = this.correo;
            // usuarios.Calle = this.calle;
            //usuarios.Nro = this.nro;
            //usuarios.Piso = this.piso;
            //usuarios.Dto = this.dto;
            //usuarios.IdLocalidad = Convert.ToInt32(this.idlocalidad);
            //usuarios.IdProvincia = Convert.ToInt32(this.idprovincia);
        }
        
        #endregion
        
    }
}
