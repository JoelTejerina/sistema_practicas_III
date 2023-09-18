using System;
using System.Collections.Generic;
using System.Data;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Personas
    {
        private CD_Personas personas = new CD_Personas();

        #region ATRIBUTOS
        private int idpersona;
        private string apellido;
        private string nombre;
        private string tipodoc;
        private string nrodoc;
        private string telefono;
        private string correo;
        private string calle;
        private string nro;
        private string piso;
        private string dto;
        private string idlocalidad;
        private string idprovincia;
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

        public string TipoDoc
        {
            get => tipodoc;
            set { tipodoc = value; }
        }

        public string NroDoc
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

        public string IdLocalidad
        {
            get => idlocalidad;
            set { idlocalidad = value; }
        }

        public string IdProvincia
        {
            get => idprovincia;
            set { idprovincia = value; }
        }
        #endregion

        #region METODOS

        public DataTable MostrarPersona()
        {
            DataTable tabla = new DataTable();
            tabla = personas.Mostrar();
            return tabla;
        }

        public void InsertarPersona()
        {
            PasarDatos();
            personas.InsertarPersona();
        }

        public void ModificarPersona()
        {
            PasarDatos();
            personas.ModificarPersona();
        }

        public void EliminarPersona()
        {
            personas.IdPersona = idpersona;
            personas.EliminarPersona();
        }

        private void PasarDatos()
        {
            personas.IdPersona = idpersona;
            personas.Apellido = this.apellido;
            personas.Nombre = this.nombre;
            personas.TipoDoc = Convert.ToInt32(this.tipodoc);
            if (this.nrodoc == "")
            {
                personas.NroDoc = 0;
            }
            else
            {
                personas.NroDoc = Convert.ToInt32(this.nrodoc);
            }
            personas.Telefono = this.telefono;
            personas.Correo = this.correo;
            personas.Calle = this.calle;
            personas.Nro = this.nro;
            personas.Piso = this.piso;
            personas.Dto = this.dto;
            personas.IdLocalidad = Convert.ToInt32(this.idlocalidad);
            personas.IdProvincia = Convert.ToInt32(this.idprovincia);
        }

        #endregion
    }
}
