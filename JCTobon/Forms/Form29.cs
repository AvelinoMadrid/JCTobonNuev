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

namespace JCTobon.Forms
{
    public partial class Form29 : Form
    {
        public Form29()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        //SqlConnection con = new SqlConnection("Data Source=DESKTOP-GD5MVN2;Initial Catalog=PuntoVenta;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=jctobon.cku8hyfumkfn.us-east-1.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        public void mostrarFolio(string folio)
        {
            txtfolio.Text= folio;
                
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void txtfolio_TextChanged(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select Nombre,Talla,CantidadPiezas,IdCodigoBarra,Tipo,Existencia from Ventas where Folio = '" + txtfolio.Text+"' ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string codigobarra;
            int cantidad, existencia, nuevostock;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                codigobarra = row.Cells[3].Value.ToString();
                cantidad = int.Parse(row.Cells[2].Value.ToString());
                existencia = int.Parse(row.Cells[5].Value.ToString());

                nuevostock = existencia - cantidad;

                // Actualiza el stock en la tabla Productos
                con.Open();
                SqlCommand updateProductos = new SqlCommand("UPDATE Productos SET Existencia = @existencia WHERE CodigoBarra = @codigobarra", con);
                updateProductos.Parameters.AddWithValue("@existencia", nuevostock);
                updateProductos.Parameters.AddWithValue("@codigobarra", codigobarra);
                updateProductos.ExecuteNonQuery();
                con.Close();

                // Actualiza el stock en la tabla Ventas
                con.Open();
                SqlCommand updateVentas = new SqlCommand("UPDATE Ventas SET Existencia = @existencia WHERE IdCodigoBarra = @codigobarra", con);
                updateVentas.Parameters.AddWithValue("@existencia", nuevostock);
                updateVentas.Parameters.AddWithValue("@codigobarra", codigobarra);
                updateVentas.ExecuteNonQuery();
                con.Close();
            }

            MessageBox.Show("Stock actualizados");
            this.Close();
        }

    }
}
