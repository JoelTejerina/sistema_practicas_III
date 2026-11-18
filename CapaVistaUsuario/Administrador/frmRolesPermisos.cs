using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador;

namespace CapaVistaUsuario.Administrador
{
    /// <summary>
    /// Gestion de roles y permisos (CU-USU-004): crea y elimina roles propios y elige que permisos tiene cada uno.
    /// El Gerente siempre tiene todos los permisos. Los cambios rigen desde el proximo ingreso de cada usuario.
    /// </summary>
    public class frmRolesPermisos : Form
    {
        private readonly CN_RolesPermisos roles = new CN_RolesPermisos();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        private readonly Label lblRoles = new Label();
        private readonly ListBox lstRoles = new ListBox();
        private readonly Label lblNuevo = new Label();
        private readonly TextBox txtNuevo = new TextBox();
        private readonly Button btnNuevo = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Label lblPermisos = new Label();
        private readonly CheckedListBox lstPermisos = new CheckedListBox();
        private readonly Label lblAyuda = new Label();
        private readonly Button btnGuardar = new Button();
        private readonly Button btnCerrar = new Button();
        private DataTable tablaRoles;

        public frmRolesPermisos()
        {
            Name = "frmRolesPermisos";
            Text = Idioma.Texto("RolesPermisos_Titulo");
            ClientSize = new Size(700, 470);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            lblRoles.AutoSize = true;
            lblRoles.Location = new Point(12, 12);
            lstRoles.Location = new Point(12, 32);
            lstRoles.Size = new Size(250, 240);
            lstRoles.SelectedIndexChanged += (s, e) => MostrarPermisosDelRol();

            lblNuevo.AutoSize = true;
            lblNuevo.Location = new Point(12, 290);
            txtNuevo.Location = new Point(12, 310);
            txtNuevo.Size = new Size(250, 20);
            btnNuevo.Location = new Point(12, 340);
            btnNuevo.Size = new Size(120, 30);
            btnNuevo.Click += btnNuevo_Click;
            btnEliminar.Location = new Point(142, 340);
            btnEliminar.Size = new Size(120, 30);
            btnEliminar.Click += btnEliminar_Click;

            lblPermisos.AutoSize = true;
            lblPermisos.Location = new Point(290, 12);
            lstPermisos.Location = new Point(290, 32);
            lstPermisos.Size = new Size(398, 300);
            lstPermisos.CheckOnClick = true;

            lblAyuda.Location = new Point(290, 340);
            lblAyuda.Size = new Size(398, 50);
            lblAyuda.ForeColor = SystemColors.GrayText;

            btnGuardar.Location = new Point(290, 410);
            btnGuardar.Size = new Size(160, 36);
            btnGuardar.Click += btnGuardar_Click;
            btnCerrar.Location = new Point(598, 410);
            btnCerrar.Size = new Size(90, 36);
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblRoles, lstRoles, lblNuevo, txtNuevo, btnNuevo, btnEliminar,
                lblPermisos, lstPermisos, lblAyuda, btnGuardar, btnCerrar
            });

            lblRoles.Text = Idioma.Texto("Roles");
            lblNuevo.Text = Idioma.Texto("NuevoRol");
            btnNuevo.Text = Idioma.Texto("Btn_CrearRol");
            btnEliminar.Text = Idioma.Texto("Btn_EliminarRol");
            lblPermisos.Text = Idioma.Texto("Permisos");
            lblAyuda.Text = Idioma.Texto("RolesPermisos_Ayuda");
            btnGuardar.Text = Idioma.Texto("Btn_GuardarPermisos");
            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");

            Load += (s, e) => CargarTodo();
        }

        private int IdRolElegido()
        {
            DataRowView fila = lstRoles.SelectedItem as DataRowView;
            return fila == null ? 0 : Convert.ToInt32(fila["IdGrupo"]);
        }

        private string NombreRolElegido()
        {
            DataRowView fila = lstRoles.SelectedItem as DataRowView;
            return fila == null ? "" : Convert.ToString(fila["Grupo"]);
        }

        private void CargarTodo(int seleccionar = 0)
        {
            try
            {
                lstPermisos.Items.Clear();
                DataTable permisos = roles.Permisos();
                foreach (DataRow p in permisos.Rows)
                {
                    lstPermisos.Items.Add(new ItemPermiso(Convert.ToInt32(p["IdPermiso"]), Convert.ToString(p["Funcionalidad"])), false);
                }

                tablaRoles = roles.Roles();
                tablaRoles.Columns.Add("Etiqueta", typeof(string), "Grupo + '  (' + Usuarios + ')'");
                lstRoles.DataSource = null;
                lstRoles.DataSource = tablaRoles;
                lstRoles.DisplayMember = "Etiqueta";
                lstRoles.ValueMember = "IdGrupo";
                if (seleccionar > 0) lstRoles.SelectedValue = seleccionar;
                MostrarPermisosDelRol();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Tilda en la lista los permisos que el rol elegido tiene asignados.
        private void MostrarPermisosDelRol()
        {
            int id = IdRolElegido();
            if (id == 0) return;
            bool esGerente = NombreRolElegido() == PermisosSistema.GrupoGerente;
            List<int> asignados = roles.PermisosDelRol(id);
            for (int i = 0; i < lstPermisos.Items.Count; i++)
            {
                lstPermisos.SetItemChecked(i, esGerente || asignados.Contains(((ItemPermiso)lstPermisos.Items[i]).Id));
            }
            lstPermisos.Enabled = !esGerente;
            btnGuardar.Enabled = !esGerente;
            btnEliminar.Enabled = !CN_RolesPermisos.EsRolBase(NombreRolElegido());
        }

        // Guarda los permisos tildados; los del Gerente no se pueden tocar y eso lo controla la capa de negocio.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            int id = IdRolElegido();
            if (id == 0) return;
            try
            {
                List<int> elegidos = lstPermisos.CheckedItems.Cast<ItemPermiso>().Select(p => p.Id).ToList();
                roles.GuardarPermisos(id, NombreRolElegido(), elegidos);
                new CL_clsBitacora("Modificación de permisos", "Rol " + NombreRolElegido() + ": " + elegidos.Count + " permisos", Name);
                MessageBox.Show(Idioma.Texto("Msg_PermisosGuardados"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!obligatorios.Validar(txtNuevo)) return;
            try
            {
                int id = roles.CrearRol(txtNuevo.Text);
                new CL_clsBitacora("Alta de rol", "Rol " + txtNuevo.Text.Trim(), Name);
                txtNuevo.Clear();
                CargarTodo(id);
            }
            catch (InvalidOperationException ex)
            {
                obligatorios.Marcar(txtNuevo);
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // No se eliminan los roles base ni los que todavia tienen usuarios asignados.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = IdRolElegido();
            if (id == 0) return;
            string rol = NombreRolElegido();
            if (MessageBox.Show(string.Format(Idioma.Texto("Msg_ConfirmEliminarRol"), rol), Text,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            try
            {
                roles.EliminarRol(id, rol);
                new CL_clsBitacora("Baja de rol", "Rol " + rol, Name);
                CargarTodo();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private class ItemPermiso
        {
            public readonly int Id;
            private readonly string nombre;

            public ItemPermiso(int id, string nombre)
            {
                Id = id;
                this.nombre = nombre;
            }

            public override string ToString()
            {
                return nombre;
            }
        }
    }
}
