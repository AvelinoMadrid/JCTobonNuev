using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace JCTobon.Clases
{
    public class ConexionBD
    {
        SqlConnection conex = new SqlConnection();


        //string cadenaConexion = "Data Source=DESKTOP-GD5MVN2;Initial Catalog=PuntoVenta;Integrated Security=True";
        string cadenaConexion = "Data Source=jctobon.cku8hyfumkfn.us-east-1.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007";

        public SqlConnection establecerConexion()
        {

            try
            {
                conex.ConnectionString = cadenaConexion;
                //conex.Open();
                //MessageBox.Show("se conectó correctamente a la base de datos");

            }
            catch (SqlException e)
            {

                /*MessageBox.Show("No se logró conectar a la Base de Datos")*/;
            }

            return conex;
        }

        
    }
}

