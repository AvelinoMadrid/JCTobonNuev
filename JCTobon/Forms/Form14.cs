﻿
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
using System.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;
using JCTobon.Clases;
using static System.Windows.Forms.Design.AxImporter;
using System.Windows.Controls;

namespace JCTobon.Forms
{
    public partial class Form14 : Form
    {
        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");


        public Form14()
        {
            InitializeComponent();
            cargarData();
            mostrarConfiguracion();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("buscarStockUserEscuelaFecha", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@fechainicio", SqlDbType.DateTime).Value = inicio.Text;
            sa.SelectCommand.Parameters.Add("@fechafin", SqlDbType.DateTime).Value = fin.Text;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;



        }

        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select CodigoBarra,Nombre,Marca,Talla,Existencia,Fecha from Productos order by Existencia asc ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;

        }

        Catalogo_Marca cat = new Catalogo_Marca();
       
        private void Form14_Load(object sender, EventArgs e)
        {
            //SqlCommand query = new SqlCommand("SELECT Tipo FROM Catalogos", con);
            //con.Open();
            //SqlDataReader registro = query.ExecuteReader();

            //comboBox1.Items.Add("Todos");

            //while (registro.Read())
            //{
            //    comboBox1.Items.Add(registro["Tipo"].ToString());

            //}

            //con.Close();

            //SqlCommand querys = new SqlCommand("SELECT Marca FROM Catalogos", con);
            //con.Open();
            //SqlDataReader leer = querys.ExecuteReader();
            //comboBox2.Items.Add("Todos");
            //while (leer.Read())
            //{
            //    comboBox2.Items.Add(leer["Marca"].ToString());
            //}
            //con.Close();

            comboBox2.DataSource = cat.CargarCombo();
            comboBox2.DisplayMember = "Marca";

            comboBox1.DataSource = cat.CargarNombres();
            comboBox1.DisplayMember = "Nombre";


            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void cargarNombre()
        {
            con.Open();
            
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Nombre FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Todos");
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["Nombre"].ToString());

            }

            dr.Close();
            con.Close();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = comboBox1.Text;
            if (opcion.Equals("Todos")) 
            {
                cargarData();
            }

            else 
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarStockUserEscuela", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }

        }

        private void fin_ValueChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        public void mostrarConfiguracion()
        {
            con.Open();



            SqlCommand query1 = new SqlCommand("select * from Configuracion where ID = 7", con);
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Existencia")
            {
                if (Convert.ToInt32(e.Value) <= 5)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.BackColor = Color.Red;

                }

                if (Convert.ToInt32(e.Value) >= 6)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.BackColor = Color.GreenYellow;
                }


            }

            if (e.ColumnIndex == 0 && e.Value is decimal)
            {
                e.Value = ((decimal)e.Value).ToString("N2");
            }
        }

        public void exportarPDF()
        {

            string rucValue = "";
            string folioValue = "";

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = " Inventario " + ".pdf";
            guardar.DefaultExt = "pdf";
            guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                string contenidoHTML = Properties.Resources.InvMostrador.ToString();

                // Obtener la fecha actual
                DateTime currentDate = DateTime.Now;
                string ruc = currentDate.ToString("dd/MM/yyyy");

                // Generar el número de folio automático
                int folio = generateFolio();

                string fileName = guardar.FileName;

                // Verificar si la extensión ".pdf" está presente en el nombre del archivo
                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    // Agregar la extensión ".pdf" al nombre del archivo
                    fileName = Path.ChangeExtension(fileName, ".pdf");
                }

                using (FileStream stream = new FileStream(guardar.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    pdfDoc.Add(new Phrase(""));

                    contenidoHTML = contenidoHTML.Replace("<span id='ruc'>ruc </span>", "<span id='ruc'>" + ruc + "</span>");
                    contenidoHTML = contenidoHTML.Replace("<span id='folio'>folio</span>", "<span id='folio'>" + folio + "</span>");


                    // Obtener la fecha actual
                    string fecha = getCurrentDate();

                    // Actualizar los valores en el HTML
                    contenidoHTML = contenidoHTML.Replace("RUC", fecha);
                    contenidoHTML = contenidoHTML.Replace("FOLIO", generateFolio().ToString());

                    using (StringReader sr = new StringReader(contenidoHTML))
                    {
                        XMLWorkerHelper helper = XMLWorkerHelper.GetInstance();
                        helper.ParseXHtml(writer, pdfDoc, sr);

                        if (dataGridView1.Rows.Count > 0)
                        {
                            PdfPTable pTable = new PdfPTable(dataGridView1.Columns.Count);
                            pTable.DefaultCell.Padding = 2;
                            pTable.WidthPercentage = 100;
                            pTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                            // Agregar el RUC a la tabla
                            PdfPCell rucCell = new PdfPCell(new Phrase("fecha: " + ruc));
                            rucCell.Colspan = dataGridView1.Columns.Count;
                            rucCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            rucCell.Border = PdfPCell.NO_BORDER;
                            rucCell.PaddingTop = -50;
                            rucCell.PaddingRight = 55;
                            pTable.AddCell(rucCell);

                            // Agregar el número de folio a la tabla
                            PdfPCell folioCell = new PdfPCell(new Phrase("folio: " + generateFolio()));
                            folioCell.Colspan = dataGridView1.Columns.Count;
                            folioCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            folioCell.Border = PdfPCell.NO_BORDER;
                            folioCell.PaddingRight = 10;
                            folioCell.PaddingTop = -50;
                            pTable.AddCell(folioCell);

                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                PdfPCell pcell = new PdfPCell(new Phrase(col.HeaderText));
                                pTable.AddCell(pcell);
                            }

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                foreach (DataGridViewCell dcell in row.Cells)
                                {
                                    pTable.AddCell(dcell.Value.ToString());
                                }
                            }

                            pdfDoc.Add(pTable);
                        }
                    }

                    pdfDoc.Close();
                    stream.Close();
                }
            }
        }

        private string getCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            int day = currentDate.Day;
            int month = currentDate.Month;
            int year = currentDate.Year;
            return $"{day}/{month}/{year}";
        }

        private int folioCounter = 0;

        private int generateFolio()
        {
            folioCounter++;
            return folioCounter;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exportarPDF();
        }

        public void cargarMarcas()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Marca FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Todos");
            while (dr.Read())
            {
                comboBox2.Items.Add(dr["Marca"].ToString());

            }
            dr.Close();
            con.Close();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = comboBox2.Text;
            if (opcion.Equals("Todos"))
            {
                cargarData();
            }
            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarMarcaMostrador", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@marca", SqlDbType.NVarChar, 150).Value = opcion;

                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }

        }
    }
}
