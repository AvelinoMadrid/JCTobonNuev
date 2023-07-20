
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace JCTobon.Forms
{
    

    public partial class Form6 : Form
    {

        public Form6()
        {
            InitializeComponent();

        }

        static double  precioMaquila;
        static double precioProveedor;
        static double Escuela;
        static double utilidad;
        static double utilidadtobon;



        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        //SqlConnection con = new SqlConnection("Data Source=DESKTOP-GD5MVN2;Initial Catalog=PuntoVenta;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=jctobon.cku8hyfumkfn.us-east-1.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form6_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            precioMaquila = double.Parse(txtpmaquila.Text);
            precioProveedor = double.Parse(txtpproveedor.Text);

            if (precioMaquila == 0 && precioProveedor == 0)
            {

                Escuela = double.Parse(precioescuela.Text);
                utilidad = 0;
                txtutilidad.Text = utilidad.ToString();
                //utilidadjctobon.Text = precioProveedor.ToString() + utilidad.ToString();
                utilidadtobon = 0;
                utilidadjctobon.Text = utilidadtobon.ToString();
            }

            else
            {
                Escuela = double.Parse(precioescuela.Text);
                utilidad = (Escuela - precioProveedor) / 2;
                txtutilidad.Text = utilidad.ToString();
                //utilidadjctobon.Text = precioProveedor.ToString() + utilidad.ToString(); 
                utilidadtobon = precioProveedor + utilidad;
                utilidadjctobon.Text = utilidadtobon.ToString();
            }




           
            this.Close();
        }

        public double getMaquila()
        {
            return precioMaquila;
        }


        public double getProveedor()
        {
            return precioProveedor;
        }

        public double getEscuela()
        {
            return Escuela;
        }

        public double getUtilidad()
        {
            return utilidad;
        }

        public double getUtilidadTobon()
        {
            return utilidadtobon;
        }




        private void txtpmaquila_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                MessageBox.Show("Ingrese existencia valida (carácteres numericos)", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void txtpproveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                MessageBox.Show("Ingrese existencia valida (carácteres numericos)", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        private void precioescuela_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                MessageBox.Show("Ingrese existencia valida (carácteres numericos)", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
        }

        public void validar()
        {
            var vr = !string.IsNullOrEmpty(txtpmaquila.Text) &&
                     !string.IsNullOrEmpty(txtpproveedor.Text) &&
                     !string.IsNullOrEmpty(precioescuela.Text);
            button1.Enabled = vr;
           
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void txtpmaquila_TextChanged(object sender, EventArgs e)
        {
            validar();
        }

        private void txtpproveedor_TextChanged(object sender, EventArgs e)
        {
            validar();
        }

        private void precioescuela_TextChanged(object sender, EventArgs e)
        {
            validar();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        //public void muestraPrecios(string id)
        //{
        //    con.Open();

        //    SqlCommand query1 = new SqlCommand("select * from Catalogos where ID = " + id + " ", con);
        //    SqlDataReader registro = query1.ExecuteReader();

        //    if (registro.HasRows)
        //    {
        //        registro.Read();

        //        txtpmaquila.Text = registro["PrecioMaquila"].ToString();
        //        txtpproveedor.Text = registro["PrecioProveedor"].ToString();
        //        precioescuela.Text = registro["PrecioEscuela"].ToString();
               
        //    }

        //    con.Close();
        //}






    }
    

   
}
