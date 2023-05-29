
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;
using JCTobon.Clases;
using iTextSharp.tool.xml.html;

using Microsoft.VisualBasic.ApplicationServices;


namespace JCTobon.Forms
{
    public partial class Form19 : Form
    {
        public Form19()
        {
            InitializeComponent();
            mostrarConfiguracion();
            cargarData();

            comboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            combomarca.DropDownStyle = ComboBoxStyle.DropDownList;
            button4.Visible = false;

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;

        }

        string id; 




        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");

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
                pictureBox3.Image = bm;


                label2.Text = registro["RazonSocial"].ToString();
                label3.Text = registro["Direccion"].ToString();
            }

            con.Close();

        }

        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select ID,Folio,Nombre,Talla,PrecioVenta,CantidadPiezas,Marca,Total,UtilidadEscuela,UtilidadJCTobon,Fecha from Ventas ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

        Catalogo_Marca cat = new Catalogo_Marca();


        private void Form19_Load(object sender, EventArgs e)
        {
           
            combomarca.DataSource = cat.CargarCombo();
            combomarca.DisplayMember = "Marca";
            //cargarNombre
            comboTipo.DataSource = cat.CargarNombres();
            comboTipo.DisplayMember = "Nombre";

            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadEscuela"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadJCTobon"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["PrecioVenta"].DefaultCellStyle.Format = "C";

            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void cargarNombre()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Nombre FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            comboTipo.Items.Clear();
            comboTipo.Items.Add("Todos");
            while (dr.Read())
            {
                comboTipo.Items.Add(dr["Nombre"].ToString());

            }
            dr.Close();
            con.Close();
        }

        private void comboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = comboTipo.Text;
            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarVentaMostradoresTipo", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }
            
        }

        public void cargarMarcas()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Marca FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            combomarca.Items.Clear();
            combomarca.Items.Add("Todos");
            while (dr.Read())
            {
                combomarca.Items.Add(dr["Marca"].ToString());

            }
            dr.Close();
            con.Close();
        }

        private void combomarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = combomarca.Text;

            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarVentaEscuelaMarca", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@marca", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("BuscarVentaMostradoresFecha", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@fechainicio", SqlDbType.DateTime).Value = inicio.Text;
            sa.SelectCommand.Parameters.Add("@fechafin", SqlDbType.DateTime).Value = fin.Text;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exportarPDF();
            MessageBox.Show("PDF exportado con éxito");
        }

        // exportarPDF

        public void exportarPDF()
        {

            string rucValue = "";
            string folioValue = "";

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = "Reporte ventas " + ".pdf";
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
                            BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                            foreach (DataGridViewRow outerRow in dataGridView1.Rows)
                            { 
                                foreach (DataGridViewCell dcell in outerRow.Cells)
                                {
                                    if (dcell.OwningColumn.Name == "Total" || dcell.OwningColumn.Name == "UtilidadEscuela" || dcell.OwningColumn.Name == "UtilidadJCTobon" || dcell.OwningColumn.Name == "PrecioVenta")
                                    {
                                        PdfPCell cell = new PdfPCell(new Phrase(string.Format("{0:C}", dcell.Value), font));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        pTable.AddCell(cell);
                                    }
                                    else
                                    {
                                        pTable.AddCell(dcell.Value.ToString());
                                    }
                                }
                            }

                            pdfDoc.Add(pTable);


                            /// acumuladores
                            
                            int acumuladadorVentastotales = 0;
                            double acumuladadorUtilidadesJCTobon = 0;
                            double acumuladadorUtilidades = 0;

                            // obtencion valores 
                            int ventas = 0;
                            double utilidades = 0;
                            double jctobon = 0;




                            // Obtencion de totales
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {

                                ventas = int.Parse(row.Cells[7].Value.ToString());
                                acumuladadorVentastotales = acumuladadorVentastotales + ventas;

                                utilidades = double.Parse(row.Cells[8].Value.ToString());
                                acumuladadorUtilidades = acumuladadorUtilidades + utilidades;

                                jctobon = double.Parse(row.Cells[9].Value.ToString());
                                acumuladadorUtilidadesJCTobon = acumuladadorUtilidadesJCTobon + jctobon;


                            }

                            // fin de totales 


                            Paragraph p1 = new Paragraph();
                            p1.Alignment = Element.ALIGN_LEFT;
                            p1.Add(" Total de Ventas            : $  " + acumuladadorVentastotales.ToString());

                            Paragraph p2 = new Paragraph();
                            p2.Alignment = Element.ALIGN_LEFT;
                            p2.Add("Total de Utilidades Escuela : $ " + acumuladadorUtilidades.ToString());

                            Paragraph p3 = new Paragraph();
                            p3.Alignment = Element.ALIGN_LEFT;
                            p3.Add("Total de Utilidades JCTobon : $ " + acumuladadorUtilidadesJCTobon.ToString());


                            pdfDoc.Add(p1);
                            pdfDoc.Add(p2);
                            pdfDoc.Add(p3);

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

        public DataGridView getData()
        {
            return dataGridView1;
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            con.Open();

            int flag = 0;
            SqlCommand query = new SqlCommand("Delete from Ventas Where ID = '" + id + "'", con);


            flag = query.ExecuteNonQuery();


            if (flag == 1)
            {
                MessageBox.Show("Venta eliminada en el Sistema");
                cargarData();
            }
            else
            {
                MessageBox.Show("Venta no eliminada en el Sistema");
            }
            


            con.Close();
        }

        private void Form19_Shown(object sender, EventArgs e)
        {
            Form1 abrir = new Form1();
            string usuario = abrir.getUser();
            string rol;


            con.Open();
            SqlCommand query = new SqlCommand("Select Rol from Usuarios where Nombre = '" + usuario + "'", con);
            SqlDataReader lectura = query.ExecuteReader();

            if (lectura.HasRows)

            {
                lectura.Read();

                rol = lectura["Rol"].ToString();

                if (rol.Equals("admin"))
                {
                    button4.Visible = true;
                }

                else
                {
                    button4.Visible = false;
                }
            }

            con.Close();

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.Value is decimal)
            {
                e.Value = ((decimal)e.Value).ToString("N2");
            }
        }
    }
}
