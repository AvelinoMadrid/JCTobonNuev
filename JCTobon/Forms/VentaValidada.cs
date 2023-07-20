
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
    public partial class VentaValidada : Form
    {
        public VentaValidada()
        {
            InitializeComponent();
            cargarData();

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        string ObtenerFolio;

        //SqlConnection con = new SqlConnection("Data Source=DESKTOP-GD5MVN2;Initial Catalog=PuntoVenta;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=jctobon.cku8hyfumkfn.us-east-1.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exportarPDF();
        }

        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select ID,Folio,Nombre,Talla,CantidadPiezas,PrecioVenta,Total,UtilidadJCTobon,Fecha from ventasValidadas ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

        public void exportarPDF()
        {

            string rucValue = "";
            string folioValue = "";

            SaveFileDialog guardar = new SaveFileDialog();

            guardar.FileName = "Reporte ventas " + ".pdf";
            guardar.DefaultExt = "pdf";
            guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";
            guardar.FileName = "Reporte Ventas " + ".pdf";


            if (guardar.ShowDialog() == DialogResult.OK)
            {
                string contenidoHTML = Properties.Resources.Ventas.ToString();

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
                            rucCell.PaddingTop = -45;
                            rucCell.PaddingRight = 45;
                            pTable.AddCell(rucCell);

                            // Agregar el número de folio a la tabla
                            PdfPCell folioCell = new PdfPCell(new Phrase("folio: " + generateFolio()));
                            folioCell.Colspan = dataGridView1.Columns.Count;
                            folioCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            folioCell.Border = PdfPCell.NO_BORDER;
                            folioCell.PaddingRight = 50;
                            folioCell.PaddingTop = -30;
                            pTable.AddCell(folioCell);

                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                PdfPCell pcell = new PdfPCell(new Phrase(col.HeaderText));
                                pTable.AddCell(pcell);
                            }
                            BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                            foreach (DataGridViewRow outerRow in dataGridView1.Rows)
                            {
                                foreach (DataGridViewCell dcell in outerRow.Cells)
                                {
                                    if (dcell.OwningColumn.Name == "Total"  || dcell.OwningColumn.Name == "UtilidadJCTobon" || dcell.OwningColumn.Name == "PrecioVenta")
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

                            int acumulador = 0;
                            int valores = 0;

                            double utilidadestobon=0;
                            double acumuladortobon=0;

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                valores = int.Parse(row.Cells[5].Value.ToString());
                                acumulador = acumulador + valores;

                                utilidadestobon= double.Parse(row.Cells[7].Value.ToString());
                                acumuladortobon = acumuladortobon + utilidadestobon;

                            }

                            Paragraph p1 = new Paragraph();
                            p1.Alignment = Element.ALIGN_LEFT;
                            p1.Add(" Total de Ventas : $ " + acumulador.ToString());


                            Paragraph p2 = new Paragraph();
                            p2.Alignment = Element.ALIGN_LEFT;
                            p2.Add(" Utilidades JCTobon : $ " + acumuladortobon.ToString());


                            pdfDoc.Add(p1);
                            pdfDoc.Add(p2);

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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.Value is decimal)
            {
                e.Value = ((decimal)e.Value).ToString("N2");
            }
        }

        private void VentaValidada_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns["PrecioVenta"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadJCTobon"].DefaultCellStyle.Format = "C";

            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("buscarFechaVentaValidada", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@fechainicio", SqlDbType.DateTime).Value = Inicio.Text;
            sa.SelectCommand.Parameters.Add("@fechafin", SqlDbType.DateTime).Value = Fin.Text;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }



        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["ID"].Value);

                    try
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM ventasValidadas WHERE ID = @id", con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // Actualizar la vista de datos
                        cargarData();

                        MessageBox.Show("Registro eliminado correctamente.", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un registro para eliminar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void VentaValidada_Shown(object sender, EventArgs e)
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
                    btnEliminar.Visible = true;
                }

                else
                {
                    btnEliminar.Visible = false;
                }
            }

            con.Close();
        }
    }
}
