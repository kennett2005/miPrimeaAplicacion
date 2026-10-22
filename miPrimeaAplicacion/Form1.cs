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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly Conexion _conexion = new Conexion();
        private DataSet _dataSet = new DataSet();
        private DataTable _tablaAlumnos = new DataTable();
        private string _modoActual = "nuevo";
        private int _indiceActual = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView();
            CargarAlumnos();
        }

        private void EstilizarDataGridView()
        {
            grdDatosAlumnos.EnableHeadersVisualStyles = false;
            grdDatosAlumnos.GridColor = Color.FromArgb(230, 230, 230);

            var headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(0, 51, 102),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 51, 102),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosAlumnos.ColumnHeadersDefaultCellStyle = headerStyle;

            var defaultStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(51, 51, 51),
                SelectionBackColor = Color.FromArgb(0, 102, 204),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosAlumnos.DefaultCellStyle = defaultStyle;
        }

        private void CargarAlumnos()
        {
            _dataSet.Clear();
            _dataSet = _conexion.ObtenerDatosAcademicos();
            _tablaAlumnos = _dataSet.Tables["alumnos"];
            _tablaAlumnos.PrimaryKey = new DataColumn[] { _tablaAlumnos.Columns["idAlumno"] };

            grdDatosAlumnos.DataSource = _tablaAlumnos.DefaultView;
            MostrarRegistroActual();
        }

        private void MostrarRegistroActual()
        {
            if (_tablaAlumnos.Rows.Count > 0)
            {
                var fila = _tablaAlumnos.Rows[_indiceActual];
                idAlumno.Text = fila["idAlumno"].ToString();
                txtCodigoAlumno.Text = fila["codigo"].ToString();
                txtNombreAlumno.Text = fila["nombre"].ToString();
                txtDireccionAlumno.Text = fila["direccion"].ToString();
                txtTelefonoAlumno.Text = fila["telefono"].ToString();
                txtEmailAlumno.Text = fila["email"].ToString();

                lblRegistrosAlumnos.Text = $"{_indiceActual + 1} de {_tablaAlumnos.Rows.Count}";
            }
        }

        private void LimpiarCampos()
        {
            idAlumno.Text = "";
            txtCodigoAlumno.Clear();
            txtNombreAlumno.Clear();
            txtDireccionAlumno.Clear();
            txtTelefonoAlumno.Clear();
            txtEmailAlumno.Clear();
        }

        private void CambiarEstadoControles(bool modoEdicion)
        {
            grbDatos.Enabled = modoEdicion;
            grbNavegacion.Enabled = !modoEdicion;
            btnEliminarAlumno.Enabled = !modoEdicion;
            txtBuscarAlumnos.Enabled = !modoEdicion;
        }

        private void btnAgregarAlumno_Click(object sender, EventArgs e)
        {
            if (btnAgregarAlumno.Text == "Agregar")
            {
                btnAgregarAlumno.Text = "Guardar";
                btnModificarALumno.Text = "Cancelar";
                _modoActual = "nuevo";
                LimpiarCampos();
                CambiarEstadoControles(true);
            }
            else
            {
                string[] datosAlumno = ObtenerDatosFormulario();
                string resultado = _conexion.GestionarAlumnos(datosAlumno, _modoActual);

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al guardar alumno.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CambiarEstadoControles(false);
                    btnAgregarAlumno.Text = "Agregar";
                    btnModificarALumno.Text = "Modificar";
                    CargarAlumnos();
                }
            }
        }

        private string[] ObtenerDatosFormulario()
        {
            return new string[] {
                idAlumno.Text, txtCodigoAlumno.Text, txtNombreAlumno.Text,
                txtDireccionAlumno.Text, txtTelefonoAlumno.Text, txtEmailAlumno.Text
            };
        }

        private void btnModificarALumno_Click(object sender, EventArgs e)
        {
            if (btnModificarALumno.Text == "Modificar")
            {
                btnAgregarAlumno.Text = "Guardar";
                btnModificarALumno.Text = "Cancelar";
                CambiarEstadoControles(true);
                _modoActual = "modificar";
            }
            else
            {
                MostrarRegistroActual();
                CambiarEstadoControles(false);
                btnAgregarAlumno.Text = "Agregar";
                btnModificarALumno.Text = "Modificar";
            }
        }

        private void btnSiguienteAlumno_Click(object sender, EventArgs e)
        {
            if (_indiceActual < _tablaAlumnos.Rows.Count - 1)
            {
                _indiceActual++;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el último registro.", "Navegación de Alumnos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAnteriorAlumno_Click(object sender, EventArgs e)
        {
            if (_indiceActual > 0)
            {
                _indiceActual--;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el primer registro.", "Navegación de Alumnos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUltimoAlumno_Click(object sender, EventArgs e)
        {
            _indiceActual = _tablaAlumnos.Rows.Count - 1;
            MostrarRegistroActual();
        }

        private void btnPrimeroAlumno_Click(object sender, EventArgs e)
        {
            _indiceActual = 0;
            MostrarRegistroActual();
        }

        private void btnEliminarAlumno_Click(object sender, EventArgs e)
        {
            string mensaje = $"¿Está seguro de eliminar a {txtNombreAlumno.Text}?";
            if (MessageBox.Show(mensaje, "Eliminando alumno", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string resultado = _conexion.GestionarAlumnos(
                    new string[] { idAlumno.Text, "", "", "", "", "" }, "eliminar");

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al eliminar alumno.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _indiceActual = 0;
                    CargarAlumnos();
                }
            }
        }

        private void txtBuscarAlumnos_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                FiltrarRegistros(txtBuscarAlumnos.Text);
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
                DataView vista = _tablaAlumnos.DefaultView;
                vista.RowFilter = $"codigo LIKE '%{textoBusqueda}%' OR nombre LIKE '%{textoBusqueda}%'";
                grdDatosAlumnos.DataSource = vista;
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
                if (grdDatosAlumnos.CurrentRow == null)
                {
                    MessageBox.Show("No hay filas que mostrar");
                    return;
                }
                string id = grdDatosAlumnos.CurrentRow.Cells["id"].Value.ToString();
                _indiceActual = _tablaAlumnos.Rows.IndexOf(_tablaAlumnos.Rows.Find(id));
                MostrarRegistroActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}