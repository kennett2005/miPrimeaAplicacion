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
    public partial class frm_usuario : Form
    {
        public frm_usuario()
        {
            InitializeComponent();
        }

        private readonly Conexion _conexion = new Conexion();
        private DataSet _dataSet = new DataSet();
        private DataTable _tablaUsuarios = new DataTable();
        private string _modoActual = "nuevo";
        private int _indiceActual = 0;

        private void frm_usuario_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView();
            CargarDatos();
        }

        private void EstilizarDataGridView()
        {
            grdDatosUsuarios.EnableHeadersVisualStyles = false;
            grdDatosUsuarios.GridColor = Color.FromArgb(230, 230, 230);

            var headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(0, 51, 102),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 51, 102),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosUsuarios.ColumnHeadersDefaultCellStyle = headerStyle;

            var defaultStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(51, 51, 51),
                SelectionBackColor = Color.FromArgb(0, 102, 204),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosUsuarios.DefaultCellStyle = defaultStyle;
        }

        private void CargarDatos()
        {
            _dataSet.Clear();
            _dataSet = _conexion.ObtenerUsuarios();
            _tablaUsuarios = _dataSet.Tables["usuarios_usss090925"];
            _tablaUsuarios.PrimaryKey = new DataColumn[] { _tablaUsuarios.Columns["idUsuario"] };

            grdDatosUsuarios.DataSource = _tablaUsuarios.DefaultView;
            MostrarRegistroActual();
        }

        private void MostrarRegistroActual()
        {
            if (_tablaUsuarios.Rows.Count > 0)
            {
                var fila = _tablaUsuarios.Rows[_indiceActual];
                txtIdUsuario.Text = fila["idUsuario"].ToString();
                txtUsuario.Text = fila["usuario"].ToString();
                txtClave.Text = fila["clave"].ToString();
                txtConfirmarClave.Text = fila["clave"].ToString();
                txtNombre.Text = fila["nombre"].ToString();
                txtDireccion.Text = fila["direccion"].ToString();
                txtTelefono.Text = fila["telefono"].ToString();

                lblRegistros.Text = $"{_indiceActual + 1} de {_tablaUsuarios.Rows.Count}";
            }
        }

        private void LimpiarCampos()
        {
            txtIdUsuario.Clear();
            txtUsuario.Clear();
            txtClave.Clear();
            txtConfirmarClave.Clear();
            txtNombre.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
        }

        private void CambiarEstadoControles(bool modoEdicion)
        {
            grbDatos.Enabled = modoEdicion;
            grbNavegacion.Enabled = !modoEdicion;
            btnEliminar.Enabled = !modoEdicion;
            txtBuscarUsuarios.Enabled = !modoEdicion;
        }

        private bool ValidarEntrada()
        {
            if (txtUsuario.Text.Length < 6 || txtUsuario.Text.Length > 16)
            {
                MostrarAdvertencia("El nombre de usuario debe tener entre 6 y 16 caracteres.", txtUsuario);
                return false;
            }

            if (txtClave.Text != txtConfirmarClave.Text)
            {
                MostrarAdvertencia("La contraseña y la confirmación no coinciden.", txtConfirmarClave);
                return false;
            }

            bool tieneLetra = txtClave.Text.Any(char.IsLetter);
            bool tieneNumero = txtClave.Text.Any(char.IsNumber);

            if (!tieneLetra || !tieneNumero)
            {
                MostrarAdvertencia("La contraseña debe contener al menos una letra y un número.", txtClave);
                return false;
            }

            return true;
        }

        private void MostrarAdvertencia(string mensaje, Control control)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (btnAgregar.Text == "Agregar")
            {
                btnAgregar.Text = "Guardar";
                btnModificar.Text = "Cancelar";
                _modoActual = "nuevo";
                LimpiarCampos();
                CambiarEstadoControles(true);
            }
            else
            {
                if (!ValidarEntrada()) return;

                string[] datosUsuario = ObtenerDatosFormulario();
                string resultado = _conexion.GestionarUsuarios(datosUsuario, _modoActual);

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al guardar usuario.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CambiarEstadoControles(false);
                    btnAgregar.Text = "Agregar";
                    btnModificar.Text = "Modificar";
                    CargarDatos();
                }
            }
        }

        private string[] ObtenerDatosFormulario()
        {
            return new string[] {
                txtIdUsuario.Text, txtUsuario.Text, txtClave.Text,
                txtNombre.Text, txtDireccion.Text, txtTelefono.Text
            };
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (btnModificar.Text == "Modificar")
            {
                btnAgregar.Text = "Guardar";
                btnModificar.Text = "Cancelar";
                CambiarEstadoControles(true);
                _modoActual = "modificar";
            }
            else
            {
                MostrarRegistroActual();
                CambiarEstadoControles(false);
                btnAgregar.Text = "Agregar";
                btnModificar.Text = "Modificar";
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string mensajeConfirmacion = $"¿Está seguro de eliminar al usuario {txtNombre.Text}?";
            if (MessageBox.Show(mensajeConfirmacion, "Eliminando usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string resultado = _conexion.GestionarUsuarios(
                    new string[] { txtIdUsuario.Text, "", "", "", "", "" }, "eliminar");

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al eliminar usuario.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _indiceActual = 0;
                    CargarDatos();
                }
            }
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            _indiceActual = 0;
            MostrarRegistroActual();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_indiceActual > 0)
            {
                _indiceActual--;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el primer registro.", "Navegación de Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_indiceActual < _tablaUsuarios.Rows.Count - 1)
            {
                _indiceActual++;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el último registro.", "Navegación de Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            _indiceActual = _tablaUsuarios.Rows.Count - 1;
            MostrarRegistroActual();
        }

        private void txtBuscarUsuarios_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                FiltrarRegistros(txtBuscarUsuarios.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FiltrarRegistros(string textoBusqueda)
        {
            try
            {
                DataView vista = _tablaUsuarios.DefaultView;
                vista.RowFilter = $"usuario LIKE '%{textoBusqueda}%' OR nombre LIKE '%{textoBusqueda}%'";
                grdDatosUsuarios.DataSource = vista;
                SeleccionarDesdeGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SeleccionarDesdeGrilla()
        {
            try
            {
                if (grdDatosUsuarios.CurrentRow == null)
                {
                    MessageBox.Show("No hay filas que mostrar");
                    return;
                }
                string id = grdDatosUsuarios.CurrentRow.Cells["colIdUsuario"].Value.ToString();
                _indiceActual = _tablaUsuarios.Rows.IndexOf(_tablaUsuarios.Rows.Find(id));
                MostrarRegistroActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}