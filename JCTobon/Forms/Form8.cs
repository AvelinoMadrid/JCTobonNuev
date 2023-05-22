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
using System.Collections;
using JCTobon.Clases;

namespace JCTobon.Forms
{
    public partial class Form8 : Form
    {

        private Form21 form21;

        string tipo, nombre, existencia, precioMaquila, utilidad, precioVenta, descripcion, talla, marca, modelo, color, temporada, codigobarra;
        string id;

        public Form8()
        {
            InitializeComponent();
            cargarData();
            cargatipos.DropDownStyle = ComboBoxStyle.DropDownList;
            cargarmarca.DropDownStyle = ComboBoxStyle.DropDownList;
          

        }

        private void button3_Click(object sender, EventArgs e)
        {
            exportarPDF();
            
        }

        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click_1(object sender, EventArgs e)
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("SP_DELETE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", 1);
            cmd.Parameters.AddWithValue("@Tipo", "");
            cmd.Parameters.AddWithValue("@Nombre", "");
            cmd.Parameters.AddWithValue("@Existencia", "");
            cmd.Parameters.AddWithValue("@PrecioMaquila", "");
            cmd.Parameters.AddWithValue("@Utilidad", "");
            cmd.Parameters.AddWithValue("@PrecioVenta", "");
            cmd.Parameters.AddWithValue("@Descripcion", "");
            cmd.Parameters.AddWithValue("@Talla", "");
            cmd.Parameters.AddWithValue("@Marca", "");
            cmd.Parameters.AddWithValue("@Modelo", "");
            cmd.Parameters.AddWithValue("@Color", "");
            cmd.Parameters.AddWithValue("@Temporada", "");
            cmd.Parameters.AddWithValue("@CodigoBarra", codigobarra);
            cmd.Parameters.AddWithValue("@StatementType", "DELETE");

            if (MessageBox.Show("El producto " + 1 + " se eliminará", "Confirmación", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Se ha eliminado el registro");
                    cargarData();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                    throw;
                }
            }
            //conectar.mostrar("Productos", dataGridView1);
            con.Close();



        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AgPrd_UpdateEventHandler(object sender, Form25.UpdateEventArgs agrs)
        {
            cargarData();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            tipo = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            marca = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            existencia = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            precioMaquila = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            utilidad = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            precioVenta = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            descripcion = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            talla = dataGridView1.CurrentRow.Cells[9].Value.ToString();

            codigobarra = dataGridView1.CurrentRow.Cells[10].Value.ToString();
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            tipo = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            marca = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            existencia = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            precioMaquila = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            utilidad = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            precioVenta = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            descripcion = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            talla = dataGridView1.CurrentRow.Cells[9].Value.ToString();
            codigobarra = dataGridView1.CurrentRow.Cells[10].Value.ToString();

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Existencia")
            {
                if(Convert.ToInt32(e.Value) <= 5)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.BackColor = Color.Red;
                    
                }

                if(Convert.ToInt32(e.Value) >= 6)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.BackColor = Color.GreenYellow;
                }


            }
        }
        public void cargarMarcas()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Marca FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            cargarmarca.Items.Clear();
            cargarmarca.Items.Add("Todos");
            while (dr.Read())
            {   
                cargarmarca.Items.Add(dr["Marca"].ToString());
                
            }
            dr.Close();
            con.Close();
        }

        private void cargarmarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                string opcion = cargarmarca.Text;
            if (opcion.Equals("Todos"))
            {
                cargarData();
            }
            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarMarca", con);
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

        private void label6_Click(object sender, EventArgs e)
        {
           
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form25 abrir = new Form25(this);
            abrir.UpdateEventHandler += AgPrd_UpdateEventHandler;
            abrir.Show();

        }

        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select ID,Tipo,Nombre,Marca,Talla,Existencia,UtilidadEscuela,UtilidadJCTobon,PrecioVenta,Descripcion,CodigoBarra,Fecha from Productos ", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;

        }
        Catalogo_Marca cat = new Catalogo_Marca();
        
        public void cargarNombre()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Nombre FROM Productos", con);
            SqlDataReader dr = cmd.ExecuteReader();
            cargatipos.Items.Clear();
            cargatipos.Items.Add("Todos");
            while (dr.Read())
            {
                cargatipos.Items.Add(dr["Nombre"].ToString());

            }
            dr.Close();
            con.Close();
        }
        private void Form8_Load(object sender, EventArgs e)
        {

            //SqlCommand query = new SqlCommand("SELECT Nombre FROM Productos", con);
            //con.Open();
            //SqlDataReader registro = query.ExecuteReader();

            //cargatipos.Items.Add("Todos");

            //while (registro.Read())
            //{
            //    cargatipos.Items.Add(registro["Nombre"].ToString());

            //}
            //con.Close();

            //con.Open();
            //SqlCommand querys = new SqlCommand("SELECT Marca FROM Catalogos", con);
            //SqlDataReader leer = querys.ExecuteReader();
            //cargarmarca.Items.Add("Todos");

            //while (leer.Read())
            //{
            //    cargarmarca.Items.Add(leer["Marca"].ToString());
            //}

            //con.Close();

            //cargarMarcas();
            cargarmarca.DataSource = cat.CargarCombo();
            cargarmarca.DisplayMember = "Marca";
            //cargarNombre
            cargatipos.DataSource = cat.CargarNombres();
            cargatipos.DisplayMember = "Nombre";

            dataGridView1.Columns["UtilidadEscuela"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadJCTobon"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["PrecioVenta"].DefaultCellStyle.Format = "C";
            
        }

        // busqueda por tipo 
        private void cargatipos_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cargatipos.Text;

            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

            else
            {
                con.Open();
                SqlDataAdapter sa = new SqlDataAdapter("buscarTipoProductos", con);
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sa.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = opcion;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                this.dataGridView1.DataSource = dt;
                con.Close();

            }
           


        }

        // busqueda por fecha 
        private void button5_Click_1(object sender, EventArgs e)
        {
            SqlDataAdapter sa = new SqlDataAdapter("buscarP", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@fechainicio", SqlDbType.DateTime).Value = inicio.Text;
            sa.SelectCommand.Parameters.Add("@fechafinal", SqlDbType.DateTime).Value = fin.Text;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            tipo = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            marca = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            existencia = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            precioMaquila = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            utilidad = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            precioVenta = dataGridView1.CurrentRow.Cells[7].Value.ToString();   
            descripcion = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            talla = dataGridView1.CurrentRow.Cells[9].Value.ToString();
            
            codigobarra = dataGridView1.CurrentRow.Cells[10].Value.ToString();
     

        }

        private void UpdateEventHandler(object sender, Form17.UpdateEventArgs agrs)
        {
            cargarData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form17 leer = new Form17(this);
            //leer.mostradata(id,tipo, nombre, existencia, precioMaquila, utilidad, precioVenta, descripcion, talla, marca, modelo, color, temporada, codigobarra);
            leer.mostrarConfiguracion(id);
            leer.UpdateEventHandler += UpdateEventHandler;
            leer.Show();

        }
            

        public void ExportarExcel(DataGridView data)
        {
            Microsoft.Office.Interop.Excel.Application excelexport = new Microsoft.Office.Interop.Excel.Application();
            excelexport.Application.Workbooks.Add(true);

            int indicecolumnas = 0;

            foreach (DataGridViewColumn columna in data.Columns)
            {
                indicecolumnas++;
                excelexport.Cells[1, indicecolumnas] = columna.Name;
            }

            int indicefilas = 0;

            foreach (DataGridViewRow fila in data.Rows)
            {
                indicefilas++;
                indicecolumnas = 0;

                foreach (DataGridViewColumn columna in data.Columns)
                {
                    indicecolumnas++;
                    excelexport.Cells[indicefilas + 1, indicecolumnas] = fila.Cells[columna.Name].Value;
                }
            }
            excelexport.Visible = true;
        }

        private int folioCounter = 0;
        public void exportarPDF()
        {

            string rucValue = "";
            string folioValue = "";

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = "Inventario " + ".pdf";
            guardar.DefaultExt = "pdf";
            guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                string contenidoHTML = Properties.Resources.nueva1.ToString();

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

                            PdfPTable pTable = new PdfPTable(5);
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

                            string[] columnasDeseadas = { "Codigo de Barras", "Nombre", "Marca", "Talla", "Existencias" };
                            foreach (string columna in columnasDeseadas)
                            {
                                PdfPCell headerCell = new PdfPCell(new Phrase(columna));
                                pTable.AddCell(headerCell);
                            }

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                // Obtener los valores de las columnas deseadas por su nombre
                                string codigoBarras = row.Cells["CodigoBarra"].Value.ToString();
                                string nombre = row.Cells["Nombre"].Value.ToString();
                                string marca = row.Cells["Marca"].Value.ToString();
                                string talla = row.Cells["Talla"].Value.ToString();
                                string existencias = row.Cells["Existencia"].Value.ToString();

                                // Agregar los valores a las celdas de la tabla
                                pTable.AddCell(codigoBarras);
                                pTable.AddCell(nombre);
                                pTable.AddCell(marca);
                                pTable.AddCell(talla);
                                pTable.AddCell(existencias);
                            }

                            //foreach (DataGridViewColumn col in dataGridView1.Columns)
                            //{
                            //    PdfPCell pcell = new PdfPCell(new Phrase(col.HeaderText));
                            //    pTable.AddCell(pcell);
                            //}

                            //foreach (DataGridViewRow row in dataGridView1.Rows)
                            //{
                            //    foreach (DataGridViewCell dcell in row.Cells)
                            //    {
                            //        pTable.AddCell(dcell.Value.ToString());
                            //    }
                            //}

                            pdfDoc.Add(pTable);
                        }
                    }

                    pdfDoc.Close();
                    stream.Close();
                }
            }
        }

        public DataGridView getData()
        {
            return dataGridView1;
        }


        public void PDF(DataGridView data)
        {
                string rucValue = "";
                string folioValue = "";

                SaveFileDialog guardar = new SaveFileDialog();
                guardar.FileName = "Inventario " + ".pdf";
                guardar.DefaultExt = "pdf";
                guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";

            if (guardar.ShowDialog() == DialogResult.OK)
                {
                    string contenidoHTML = Properties.Resources.plantilla6.ToString();

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

        

        private int generateFolio()
        {
            folioCounter++;
            return folioCounter;
        }


    }

}
