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
    public partial class frmMaterias : Form
    {
        public frmMaterias()
        {
            InitializeComponent();
        }

        private readonly Conexion _conexion = new Conexion();
        private DataSet _dataSet = new DataSet();
        private DataTable _tablaMaterias = new DataTable();
        private string _modoActual = "nuevo";
        private int _indiceActual = 0;

        private void frmMaterias_Load(object sender, EventArgs e)
        {
            EstilizarDataGridView();
            CargarMaterias();
        }

        private void EstilizarDataGridView()
        {
            grdDatosMaterias.EnableHeadersVisualStyles = false;
            grdDatosMaterias.GridColor = Color.FromArgb(230, 230, 230);

            var headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(0, 51, 102),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(0, 51, 102),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosMaterias.ColumnHeadersDefaultCellStyle = headerStyle;

            var defaultStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(51, 51, 51),
                SelectionBackColor = Color.FromArgb(0, 102, 204),
                SelectionForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            grdDatosMaterias.DefaultCellStyle = defaultStyle;
        }

        private void CargarMaterias()
        {
            _dataSet.Clear();
            _dataSet = _conexion.ObtenerDatosAcademicos();
            _tablaMaterias = _dataSet.Tables["materias"];
            _tablaMaterias.PrimaryKey = new DataColumn[] { _tablaMaterias.Columns["idMateria"] };

            grdDatosMaterias.DataSource = _tablaMaterias.DefaultView;
            MostrarRegistroActual();
        }

        private void MostrarRegistroActual()
        {
            if (_tablaMaterias.Rows.Count > 0)
            {
                var fila = _tablaMaterias.Rows[_indiceActual];
                idMateria.Text = fila["idMateria"].ToString();
                txtCodigoMateria.Text = fila["codigo"].ToString();
                txtNombreMateria.Text = fila["nombre"].ToString();
                txtUvMateria.Text = fila["uv"].ToString();

                lblRegistrosMateria.Text = $"{_indiceActual + 1} de {_tablaMaterias.Rows.Count}";
            }
        }

        private void LimpiarCampos()
        {
            idMateria.Text = "";
            txtCodigoMateria.Clear();
            txtNombreMateria.Clear();
            txtUvMateria.Clear();
        }

        private void CambiarEstadoControles(bool modoEdicion)
        {
            grbDatos.Enabled = modoEdicion;
            grbNavegacion.Enabled = !modoEdicion;
            btnEliminarMateria.Enabled = !modoEdicion;
            txtBuscarMaterias.Enabled = !modoEdicion;
        }

        private void btnAgregarMateria_Click(object sender, EventArgs e)
        {
            if (btnAgregarMateria.Text == "Agregar")
            {
                btnAgregarMateria.Text = "Guardar";
                btnModificarMateria.Text = "Cancelar";
                _modoActual = "nuevo";
                LimpiarCampos();
                CambiarEstadoControles(true);
            }
            else
            {
                string[] datosMateria = ObtenerDatosFormulario();
                string resultado = _conexion.GestionarMaterias(datosMateria, _modoActual);

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al guardar materia.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CambiarEstadoControles(false);
                    btnAgregarMateria.Text = "Agregar";
                    btnModificarMateria.Text = "Modificar";
                    CargarMaterias();
                }
            }
        }

        private string[] ObtenerDatosFormulario()
        {
            return new string[] {
                idMateria.Text, txtCodigoMateria.Text, txtNombreMateria.Text, txtUvMateria.Text
            };
        }

        private void btnModificarMateria_Click(object sender, EventArgs e)
        {
            if (btnModificarMateria.Text == "Modificar")
            {
                btnAgregarMateria.Text = "Guardar";
                btnModificarMateria.Text = "Cancelar";
                CambiarEstadoControles(true);
                _modoActual = "modificar";
            }
            else
            {
                MostrarRegistroActual();
                CambiarEstadoControles(false);
                btnAgregarMateria.Text = "Agregar";
                btnModificarMateria.Text = "Modificar";
            }
        }

        private void btnSiguienteMateria_Click(object sender, EventArgs e)
        {
            if (_indiceActual < _tablaMaterias.Rows.Count - 1)
            {
                _indiceActual++;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el último registro.", "Navegación de Materias", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUltimoMateria_Click(object sender, EventArgs e)
        {
            _indiceActual = _tablaMaterias.Rows.Count - 1;
            MostrarRegistroActual();
        }

        private void btnAnteriorMateria_Click(object sender, EventArgs e)
        {
            if (_indiceActual > 0)
            {
                _indiceActual--;
                MostrarRegistroActual();
            }
            else
            {
                MessageBox.Show("Estás en el primer registro.", "Navegación de Materias", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPrimeroMateria_Click(object sender, EventArgs e)
        {
            _indiceActual = 0;
            MostrarRegistroActual();
        }

        private void btnEliminarMateria_Click(object sender, EventArgs e)
        {
            string mensaje = $"¿Está seguro de eliminar {txtNombreMateria.Text}?";
            if (MessageBox.Show(mensaje, "Eliminando materia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string resultado = _conexion.GestionarMaterias(
                    new string[] { idMateria.Text, "", "", "" }, "eliminar");

                if (resultado != "1")
                {
                    MessageBox.Show(resultado, "Error al eliminar materia.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _indiceActual = 0;
                    CargarMaterias();
                }
            }
        }

        private void txtBuscarMaterias_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                FiltrarRegistros(txtBuscarMaterias.Text);
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
                DataView vista = _tablaMaterias.DefaultView;
                vista.RowFilter = $"Convert(codigo, 'System.String') LIKE '%{textoBusqueda}%' OR nombre LIKE '%{textoBusqueda}%'";
                grdDatosMaterias.DataSource = vista;
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
                if (grdDatosMaterias.CurrentRow == null)
                {
                    MessageBox.Show("No hay filas que mostrar");
                    return;
                }
                string id = grdDatosMaterias.CurrentRow.Cells["id"].Value.ToString();
                _indiceActual = _tablaMaterias.Rows.IndexOf(_tablaMaterias.Rows.Find(id));
                MostrarRegistroActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}