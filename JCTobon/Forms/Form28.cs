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
using iTextSharp.tool.xml.html;

namespace JCTobon.Forms
{
    public partial class Form28 : Form
    {
        private List<string> foliosValidados = new List<string>();

        public Form28(Form26 actualizar)
        {
            InitializeComponent();
        }

        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");


        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        public delegate void UpdateDelegate(object sender, UpdateEventArgs args);
        public event UpdateDelegate UpdateEventHandler;

        public class UpdateEventArgs : EventArgs
        {
            public string Data { get; set; }
        }

        protected void Agregar()
        {
            UpdateEventArgs args = new UpdateEventArgs();
            UpdateEventHandler.Invoke(this, args);

        }




        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string folio = txtvalidacion.Text;

            con.Open();
            SqlCommand select = new SqlCommand("SELECT Folio FROM ventasValidadas WHERE Folio = @folio", con);
            select.Parameters.AddWithValue("@folio", folio);
            SqlDataReader lectura = select.ExecuteReader();

            if (lectura.HasRows)
            {
                lectura.Close();
                MessageBox.Show("El folio ya ha sido registrado en una venta anterior", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                con.Close();

                con.Open();
                SqlCommand query = new SqlCommand("SELECT ID, Tipo, Nombre, Talla, Marca, PrecioVenta, CantidadPiezas, Total, utilidadjctobon,Fecha " +
                                                  "FROM Ventas WHERE Folio = @folio", con);
                query.Parameters.AddWithValue("@folio", folio);

                SqlDataAdapter adapter = new SqlDataAdapter(query);
                DataTable productosTable = new DataTable();
                adapter.Fill(productosTable);

                if (productosTable.Rows.Count > 0)
                {
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow row in productosTable.Rows)
                            {
                                int id = int.Parse(row["ID"].ToString());
                                string tipo = row["Tipo"].ToString();
                                string nombre = row["Nombre"].ToString();
                                string talla = row["Talla"].ToString();
                                string marca = row["Marca"].ToString();
                                int precioventa = int.Parse(row["PrecioVenta"].ToString());
                                int cantidadpzas = int.Parse(row["CantidadPiezas"].ToString());
                                int total = int.Parse(row["Total"].ToString());
                                double utilidadtobon = double.Parse(row["utilidadjctobon"].ToString());
                                DateTime fecha = DateTime.Parse(row["Fecha"].ToString());


                                SqlCommand insertCommand = new SqlCommand("INSERT INTO ventasValidadas (Folio, Tipo, Nombre, Marca,Talla, PrecioVenta, CantidadPiezas, Total, UtilidadJCTobon, Fecha) " +
                                                                           "VALUES (@folio, @tipo, @nombre, @marca, @talla, @precioventa, @cantidadpzas, @total, @utilidadtobon, @fecha)", con);
                                insertCommand.Parameters.AddWithValue("@folio", folio);
                                insertCommand.Parameters.AddWithValue("@tipo", tipo);
                                insertCommand.Parameters.AddWithValue("@nombre", nombre);
                                insertCommand.Parameters.AddWithValue("@marca", marca);
                                insertCommand.Parameters.AddWithValue("@talla", talla);
                                insertCommand.Parameters.AddWithValue("@precioventa", precioventa);
                                insertCommand.Parameters.AddWithValue("@cantidadpzas", cantidadpzas);
                                insertCommand.Parameters.AddWithValue("@total", total);
                                insertCommand.Parameters.AddWithValue("@utilidadtobon", utilidadtobon);
                                insertCommand.Parameters.AddWithValue("@fecha", fecha);
                                insertCommand.Transaction = transaction;
                                insertCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            MessageBox.Show("Venta encontrada dentro del sistema");

                            Form29 abrir = new Form29();
                            abrir.mostrarFolio(folio);
                            abrir.Show();

                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error al insertar los productos de la venta validada: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Venta NO encontrada dentro del sistema");
                    this.Close();
                }

                con.Close();
            }
        }



        private void txtvalidacion_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
