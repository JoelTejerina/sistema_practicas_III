using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;

namespace CapaVistaUsuario
{
    /// <summary>
    /// Validacion de campos obligatorios: los que estan vacios se pintan de rojo, se avisa
    /// con un mensaje y el foco va al primero. La marca se saca sola cuando el usuario
    /// completa el campo o cuando el formulario se bloquea (Cancelar / Guardar).
    ///
    /// Uso (una instancia por formulario):
    ///     private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();
    ///     if (!obligatorios.Validar(txtNombre, cmbProveedor)) return false;
    /// </summary>
    class CV_Obligatorios
    {
        private static readonly Color ColorError = Color.FromArgb(255, 170, 170);

        private class Estilo
        {
            public Color Fondo;
            public FlatStyle? Plano;
        }

        private readonly Dictionary<Control, Estilo> marcados = new Dictionary<Control, Estilo>();
        private readonly HashSet<Control> observados = new HashSet<Control>();

        /// <summary>Marca todos los campos vacios. Devuelve true si estaban todos completos.</summary>
        public bool Validar(params Control[] campos)
        {
            Control primero = null;
            foreach (Control c in campos)
            {
                if (EstaVacio(c))
                {
                    Marcar(c);
                    if (primero == null)
                    {
                        primero = c;
                    }
                }
                else
                {
                    Desmarcar(c);
                }
            }

            if (primero == null)
            {
                return true;
            }

            MessageBox.Show(Idioma.Texto("Msg_CamposMarcados"));
            primero.Select();
            return false;
        }

        /// <summary>
        /// Un correo es valido solo si lleva @ y dominio (usuario@dominio.com). Si no lo es,
        /// se marca el campo en rojo y se avisa. Es la regla de todos los campos de correo.
        /// </summary>
        public bool ValidarCorreo(TextBox correo)
        {
            CV_Validar_Mail validador = new CV_Validar_Mail();
            validador.Correo = correo.Text.Trim();
            if (correo.Text.Contains("@") && validador.Valid())
            {
                return true;
            }

            Marcar(correo);
            MessageBox.Show(Idioma.Texto("Msg_CorreoInvalido"));
            correo.Select();
            return false;
        }

        /// <summary>La persona debe tener entre 16 y 100 años (edad laboral). Marca la fecha si no cumple.</summary>
        public bool ValidarEdad(DateTimePicker nacimiento)
        {
            int edad = CV_Edad.Calcular(nacimiento.Value);
            if (edad >= CV_Edad.Minima && edad <= CV_Edad.Maxima)
            {
                return true;
            }

            Marcar(nacimiento);
            MessageBox.Show(string.Format(Idioma.Texto("Msg_EdadInvalida"), CV_Edad.Minima, CV_Edad.Maxima));
            nacimiento.Select();
            return false;
        }

        /// <summary>Pinta un campo de rojo (por ejemplo, cuando el valor ingresado es invalido).</summary>
        public void Marcar(Control c)
        {
            if (!marcados.ContainsKey(c))
            {
                marcados[c] = new Estilo { Fondo = c.BackColor, Plano = (c as ComboBox)?.FlatStyle };
                Observar(c);
            }

            c.BackColor = ColorError;
            if (c is ComboBox combo)
            {
                // Con el estilo por defecto un combo desplegable ignora BackColor.
                combo.FlatStyle = FlatStyle.Flat;
            }
        }

        public void Desmarcar(Control c)
        {
            Estilo original;
            if (!marcados.TryGetValue(c, out original))
            {
                return;
            }

            // ResetBackColor (y no asignar el color anterior): asi un campo deshabilitado vuelve a verse
            // gris como el resto del formulario bloqueado.
            c.ResetBackColor();
            if (c is ComboBox combo && original.Plano.HasValue)
            {
                combo.FlatStyle = original.Plano.Value;
            }
            marcados.Remove(c);
        }

        public void LimpiarMarcas()
        {
            foreach (Control c in new List<Control>(marcados.Keys))
            {
                Desmarcar(c);
            }
        }

        public static bool EstaVacio(Control c)
        {
            TextBox texto = c as TextBox;
            if (texto != null)
            {
                return texto.Text.Trim().Length == 0;
            }

            ComboBox combo = c as ComboBox;
            if (combo != null)
            {
                // Los combos de claves (proveedor, menu, etc.) deben tener un elemento elegido.
                return combo.DataSource != null
                    ? combo.SelectedValue == null
                    : combo.Text.Trim().Length == 0;
            }

            // Una fecha con casilla (fecha de nacimiento) esta vacia mientras no se la tilde.
            DateTimePicker fecha = c as DateTimePicker;
            if (fecha != null)
            {
                return fecha.ShowCheckBox && !fecha.Checked;
            }

            // Los demas controles siempre tienen un valor.
            return false;
        }

        private void Observar(Control c)
        {
            if (!observados.Add(c))
            {
                return;
            }

            EventHandler alCambiar = (s, e) =>
            {
                if (!EstaVacio(c))
                {
                    Desmarcar(c);
                }
            };
            c.TextChanged += alCambiar;
            ComboBox combo = c as ComboBox;
            if (combo != null)
            {
                combo.SelectedIndexChanged += alCambiar;
            }

            c.EnabledChanged += (s, e) =>
            {
                if (!c.Enabled)
                {
                    Desmarcar(c);
                }
            };
        }
    }

    /// <summary>
    /// Regla del numero de documento: solo digitos, mayor a 0 y, si el tipo es DNI, hasta 8 digitos.
    /// Para otros tipos el tope es 9 digitos porque la columna NroDoc es int.
    /// </summary>
    static class CV_Documento
    {
        private const int MaxDni = 8;
        private const int MaxOtros = 9;

        public static bool EsDni(ComboBox tipo)
        {
            return string.Equals((tipo.Text ?? string.Empty).Trim(), "DNI", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Limita el campo a digitos y ajusta su largo maximo segun el tipo elegido.</summary>
        public static void Configurar(ComboBox tipo, TextBox nro)
        {
            EventHandler ajustar = (s, e) => nro.MaxLength = EsDni(tipo) ? MaxDni : MaxOtros;
            tipo.SelectedIndexChanged += ajustar;
            tipo.TextChanged += ajustar;
            nro.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
            };
            ajustar(null, EventArgs.Empty);
        }

        public static bool Validar(CV_Obligatorios obligatorios, ComboBox tipo, TextBox nro)
        {
            string texto = nro.Text.Trim();
            int valor;
            bool soloDigitos = texto.Length > 0 && texto.IndexOfAny("0123456789".ToCharArray()) >= 0
                && texto.Trim("0123456789".ToCharArray()).Length == 0;

            if (!soloDigitos || !int.TryParse(texto, out valor) || valor <= 0)
            {
                return Rechazar(obligatorios, nro, "Msg_NroDocInvalido");
            }

            if (EsDni(tipo) && texto.Length > MaxDni)
            {
                return Rechazar(obligatorios, nro, "Msg_NroDocDni");
            }

            return true;
        }

        private static bool Rechazar(CV_Obligatorios obligatorios, TextBox nro, string clave)
        {
            obligatorios.Marcar(nro);
            MessageBox.Show(Idioma.Texto(clave));
            nro.Select();
            return false;
        }
    }
}

namespace CapaVistaUsuario
{
    /// <summary>Edad a partir de la fecha de nacimiento y el selector que se agrega a los formularios.</summary>
    static class CV_Edad
    {
        public const int Minima = 16;
        public const int Maxima = 100;

        /// <summary>Deja la fecha sin elegir (o la fija) y refresca la edad que se muestra al lado.</summary>
        public static void Establecer(System.Windows.Forms.DateTimePicker selector, System.DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                selector.Value = fecha.Value;
                selector.Checked = true;
            }
            else
            {
                selector.Checked = false;
            }
            System.Action actualizar = selector.Tag as System.Action;
            if (actualizar != null) actualizar();
        }

        public static int Calcular(System.DateTime nacimiento)
        {
            System.DateTime hoy = System.DateTime.Today;
            int edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }
            return edad;
        }

        /// <summary>
        /// Crea el selector de fecha de nacimiento (con casilla: hasta que se elige queda vacio y,
        /// por ser obligatorio, se marca en rojo) y una etiqueta que muestra la edad.
        /// </summary>
        public static System.Windows.Forms.DateTimePicker CrearSelector(System.Windows.Forms.Control contenedor,
            System.Windows.Forms.Label etiqueta, System.Drawing.Point ubicacion, int ancho)
        {
            System.Windows.Forms.DateTimePicker selector = new System.Windows.Forms.DateTimePicker();
            selector.Name = "dtpNacimiento";
            selector.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            selector.ShowCheckBox = true;
            selector.Checked = false;
            selector.MinDate = new System.DateTime(1900, 1, 1);
            selector.MaxDate = System.DateTime.Today;
            selector.Location = ubicacion;
            selector.Size = new System.Drawing.Size(ancho, 20);

            System.Windows.Forms.Label edad = new System.Windows.Forms.Label();
            edad.AutoSize = true;
            edad.Location = new System.Drawing.Point(ubicacion.X + ancho + 8, ubicacion.Y + 3);
            Action actualizar = () => edad.Text = selector.Checked ? "(" + Calcular(selector.Value) + ")" : "";
            selector.Tag = actualizar;   // permite refrescar la edad cuando se cambia el estado por programa
            selector.ValueChanged += (s, e) => actualizar();
            selector.MouseUp += (s, e) => actualizar();
            selector.KeyUp += (s, e) => actualizar();

            etiqueta.AutoSize = true;
            etiqueta.Location = new System.Drawing.Point(etiqueta.Left, ubicacion.Y + 3);
            contenedor.Controls.Add(etiqueta);
            contenedor.Controls.Add(selector);
            contenedor.Controls.Add(edad);
            return selector;
        }
    }
}
