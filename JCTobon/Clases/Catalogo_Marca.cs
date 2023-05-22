using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;
using DataTable = System.Data.DataTable;


namespace JCTobon.Clases
{
    public  class Catalogo_Marca
    {
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
       
        public DataTable CargarCombo()
        {
           
            SqlDataAdapter da = new SqlDataAdapter("Cat_Marca", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Columns.Add("Marca", typeof(string));
            dt.Rows.Add("Todos");
            da.Fill(dt);
            return dt;
            
        }

        public DataTable CargarNombres()
        {
            SqlDataAdapter da = new SqlDataAdapter("Cat_Nombre", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Columns.Add("Nombre", typeof(string));
            dt.Rows.Add("Todos");
            da.Fill(dt);
            return dt;

        }

        public DataTable CargarFolio()
        {
            SqlDataAdapter da = new SqlDataAdapter("Cat_Folio", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Columns.Add("Folio", typeof(string));
            dt.Rows.Add("Todos");
            da.Fill(dt);
            return dt;

        }
    }
}
