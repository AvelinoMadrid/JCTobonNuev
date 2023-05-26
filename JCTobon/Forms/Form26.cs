
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
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;
using JCTobon.Clases;

namespace JCTobon.Forms
{
    public partial class Form26 : Form
    {

        private List<string> foliosValidados = new List<string>();

        public Form26()
        {
            InitializeComponent();
            cargarData();
        }

        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
        private void button1_Click(object sender, EventArgs e)
        {
            Form27 abrir = new Form27();
            abrir.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select Folio,Nombre,Talla,Marca,CantidadPiezas,PrecioVenta,Total,Fecha from ventasValidadas ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;


        }

        private void button3_Click(object sender, EventArgs e)
        {

            int id = int.Parse(txtfolio.Text);
            SqlDataAdapter sa = new SqlDataAdapter("buscarfolio", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@Folio", SqlDbType.Int).Value = id;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;

            



    }

      private void AgVenta_UpdateEventHandler(object sender, Form28.UpdateEventArgs args)
        {
            cargarData();
        }



        private void button4_Click(object sender, EventArgs e)
        {
            cargarData();
            txtfolio.Text = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
            Form28 abrir = new Form28(this);
            abrir.UpdateEventHandler += AgVenta_UpdateEventHandler;
            abrir.Show();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("BuscarValidacionesFecha", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@fechainicio", SqlDbType.DateTime).Value = inicio.Text;
            sa.SelectCommand.Parameters.Add("@fechafin", SqlDbType.DateTime).Value = fin.Text;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

       

        private void button6_Click(object sender, EventArgs e)
        {
        
            exportarPDF();
        }

        public void exportarPDF()
        {

            string rucValue = "";
            string folioValue = "";

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = "Reporte Ventas " + ".pdf";
            guardar.DefaultExt = "pdf";
            guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                string contenidoHTML = Properties.Resources.nueva2.ToString();

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
                    contenidoHTML = contenidoHTML.Replace("[RUC]", fecha);
                    contenidoHTML = contenidoHTML.Replace("[FOLIO]", generateFolio().ToString());

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
                                PdfPCell pcell = new PdfPCell(new Phrase(col.HeaderText.Replace("CantidadPiezas", "Cantidad")));
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


                            // obtenemos los totales 
                            int totales = 0;
                            //double utilidadtobon;
                            // creamos los acumuladores

                            int acumuladoresVentasTotales = 0;
                            double acumuladoresUtilidadesTobon = 0;


                            foreach (DataGridViewRow rows in dataGridView1.Rows)
                            {
                                totales = int.Parse(rows.Cells[6].Value.ToString());
                                //utilidadtobon = double.Parse(rows.Cells[7].Value.ToString());

                                // acumuladores de ventas totales
                                acumuladoresVentasTotales = acumuladoresVentasTotales + totales;
                                // acumuladores de Utilidades JCTobon 
                                //acumuladoresUtilidadesTobon = acumuladoresUtilidadesTobon + utilidadtobon;

                            }


                            Paragraph p1 = new Paragraph();
                            p1.Alignment = Element.ALIGN_LEFT;
                            p1.Add(" Total de Ventas             : " + acumuladoresVentasTotales.ToString());

                            //Paragraph p2 = new Paragraph();
                            //p2.Alignment = Element.ALIGN_LEFT;
                            //p2.Add(" Total de Utilidades JCTobon : " + acumuladoresUtilidadesTobon.ToString());


                            pdfDoc.Add(p1);
                            //pdfDoc.Add(p2);

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


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtfolio_TextChanged(object sender, EventArgs e)
        {

        }

        public void cargarMarcas()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Marca FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            cmdmarca.Items.Clear();
            cmdmarca.Items.Add("Todos");
            while (dr.Read())
            {
                cmdmarca.Items.Add(dr["Marca"].ToString());

            }
            dr.Close();
            con.Close();
        }

        private void txtmarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cmdmarca.Text;

            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarMarcaVentasValidadas", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@marca", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }
         
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        Catalogo_Marca cat = new Catalogo_Marca();
        private void Form26_Load(object sender, EventArgs e)
        {
            cmdmarca.DataSource = cat.CargarCombo();
            cmdmarca.DisplayMember = "Marca";

            cmdnombre.DataSource = cat.CargarNombres();
            cmdnombre.DisplayMember = "Nombre";

            dataGridView1.Columns["PrecioVenta"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "C";
            //dataGridView1.Columns["UtilidadJCTobon"].DefaultCellStyle.Format = "C";
        }

        public void cargarNombre()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Nombre FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            cmdnombre.Items.Clear();
            cmdnombre.Items.Add("Todos");
            while (dr.Read())
            {
                cmdnombre.Items.Add(dr["Nombre"].ToString());

            }
            dr.Close();
            con.Close();
        }
        private void cmdnombre_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cmdnombre.Text;

            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarNombreVentasValidadas", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }
            
        }
    }
}
