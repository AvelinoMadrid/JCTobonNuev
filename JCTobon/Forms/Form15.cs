using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace JCTobon.Forms
{
    public partial class Form15 : Form
    {
        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");

        public Form15()
        {
            InitializeComponent();
            mostrarConfiguracion();
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void mostrarConfiguracion()
        {
            con.Open();

            SqlCommand query1 = new SqlCommand("select * from Configuracion where ID = 7",  con);
            SqlDataReader registro = query1.ExecuteReader();

            if (registro.HasRows)
            {
                registro.Read();
                MemoryStream ms = new MemoryStream((byte[])registro["Imagen"]);
                Bitmap bm = new Bitmap(ms);
                pictureBox2.Image = bm;

                label2.Text = registro["RazonSocial"].ToString();
                label3.Text = registro["Direccion"].ToString();
            }

            con.Close();

        }










    }
}
