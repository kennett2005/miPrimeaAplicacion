using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace miPrimeaAplicacion
{
    internal class Conexion : IDisposable
    {
        private readonly SqlConnection _conexion;
        private readonly SqlCommand _comando;
        private readonly SqlDataAdapter _adaptador;
        private readonly DataSet _dataSet;

        public Conexion()
        {
            string cadenaConexion = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\db_academica.mdf;Integrated Security=True";
            _conexion = new SqlConnection(cadenaConexion);
            _comando = new SqlCommand();
            _adaptador = new SqlDataAdapter();
            _dataSet = new DataSet();
            _conexion.Open();
        }

        public DataSet ObtenerDatosAcademicos()
        {
            _dataSet.Clear();
            _comando.Connection = _conexion;
            _adaptador.SelectCommand = _comando;

            _comando.CommandText = "SELECT * FROM alumnos";
            _adaptador.Fill(_dataSet, "alumnos");

            _comando.CommandText = "SELECT * FROM materias";
            _adaptador.Fill(_dataSet, "materias");

            return _dataSet;
        }

        public DataSet ObtenerUsuarios()
        {
            _dataSet.Clear();
            _comando.Connection = _conexion;
            _adaptador.SelectCommand = _comando;

            _comando.CommandText = "SELECT * FROM usuarios_usss090925";
            _adaptador.Fill(_dataSet, "usuarios_usss090925");

            return _dataSet;
        }

        public string GestionarUsuarios(string[] datos, string operacion)
        {
            string sql = string.Empty;

            switch (operacion)
            {
                case "nuevo":
                    sql = $"INSERT INTO usuarios_usss090925(usuario, clave, nombre, direccion, telefono) " +
                          $"VALUES ('{datos[1]}', '{datos[2]}', '{datos[3]}', '{datos[4]}', '{datos[5]}')";
                    break;

                case "modificar":
                    sql = $"UPDATE usuarios_usss090925 SET usuario='{datos[1]}', clave='{datos[2]}', " +
                          $"nombre='{datos[3]}', direccion='{datos[4]}', telefono='{datos[5]}' " +
                          $"WHERE idUsuario='{datos[0]}'";
                    break;

                case "eliminar":
                    sql = $"DELETE FROM usuarios_usss090925 WHERE idUsuario='{datos[0]}'";
                    break;
            }

            return EjecutarComando(sql);
        }

        public string GestionarAlumnos(string[] datos, string operacion)
        {
            string sql = string.Empty;

            switch (operacion)
            {
                case "nuevo":
                    sql = $"INSERT INTO alumnos(codigo,nombre,direccion,telefono,email) " +
                          $"VALUES ('{datos[1]}', '{datos[2]}', '{datos[3]}', '{datos[4]}', '{datos[5]}')";
                    break;

                case "modificar":
                    sql = $"UPDATE alumnos SET codigo='{datos[1]}', nombre='{datos[2]}', " +
                          $"direccion='{datos[3]}', telefono='{datos[4]}', email='{datos[5]}' " +
                          $"WHERE idAlumno='{datos[0]}'";
                    break;

                case "eliminar":
                    sql = $"DELETE FROM alumnos WHERE idAlumno='{datos[0]}'";
                    break;
            }

            return EjecutarComando(sql);
        }

        public string GestionarMaterias(string[] datos, string operacion)
        {
            string sql = string.Empty;

            switch (operacion)
            {
                case "nuevo":
                    sql = $"INSERT INTO materias(codigo,nombre,uv) VALUES ('{datos[1]}', '{datos[2]}', '{datos[3]}')";
                    break;

                case "modificar":
                    sql = $"UPDATE materias SET codigo='{datos[1]}', nombre='{datos[2]}', uv='{datos[3]}' " +
                          $"WHERE idMateria='{datos[0]}'";
                    break;

                case "eliminar":
                    sql = $"DELETE FROM materias WHERE idMateria='{datos[0]}'";
                    break;
            }

            return EjecutarComando(sql);
        }

        private string EjecutarComando(string sql)
        {
            try
            {
                _comando.Connection = _conexion;
                _comando.CommandText = sql;
                return _comando.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Dispose()
        {
            if (_conexion?.State == ConnectionState.Open)
                _conexion.Close();
            _conexion?.Dispose();
            _comando?.Dispose();
            _adaptador?.Dispose();
            _dataSet?.Dispose();
        }
    }
}