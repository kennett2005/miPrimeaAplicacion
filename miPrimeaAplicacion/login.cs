using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miPrimeaAplicacion
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conexion = new Conexion())
                {
                    DataSet dsUsuarios = conexion.ObtenerUsuarios();
                    DataTable tablaUsuarios = dsUsuarios.Tables["usuarios_usss090925"];

                    var usuarioValido = tablaUsuarios.AsEnumerable()
                        .FirstOrDefault(fila => fila.Field<string>("usuario") == txtUsuario.Text
                                             && fila.Field<string>("clave") == txtClave.Text);

                    if (usuarioValido != null)
                    {
                        string nombreCompleto = usuarioValido.Field<string>("nombre");
                        MessageBox.Show($"¡Bienvenido(a) {nombreCompleto}!",
                            "Acceso Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        var principal = new frmPrincipal();
                        principal.Show();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("Credenciales inválidas. Acceso denegado.",
                            "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtClave.Clear();
                        txtClave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void login_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }
    }
}