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
using System.Windows.Forms.VisualStyles;

namespace JCTobon.Forms
{
    public partial class Form21 : Form
    {
       
        public Form21()
        {
            InitializeComponent();
            cargarData();
            comboxstatus.DropDownStyle = ComboBoxStyle.DropDownList;
            combotemporada.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTipo.DropDownStyle = ComboBoxStyle.DropDownList;

        }
        string id;
        string tipo;
        string modelo;
        string marca;
        string talla;
        string color;
        string temporada;
        string status;
        string descripcion;
        string opcion;

        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");


        public void cargarData()
        {
            SqlDataAdapter sa = new SqlDataAdapter("Select ID,Tipo,Modelo,Marca,Talla,Color,Temporada,PrecioMaquila,PrecioProveedor,UtilidadEscuela,PrecioEscuela,Descripcion,UtilidadJCTobon,Status from Catalogos", con);
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            tipo = textBoxtipo.Text;
            modelo = textBoxmodelo.Text;
            marca = textBoxmarca.Text;
            talla = textBoxtalla.Text;
            color = textBoxcolor.Text;
            temporada = combotemporada.Text;
            status = comboxstatus.Text;
            descripcion = txtdescripcion.Text;


            if (status.Equals("Activo"))
            {
                opcion = "1";
            }

            else 
            {
                opcion = "0";
            }

            Form6 obs = new Form6();
          
            obs.Show();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 obs = new Form6();
            double ma = obs.getMaquila();
            double pro = obs.getProveedor();
            double ut = obs.getUtilidad();
            double es = obs.getEscuela();
            double ujctobon = obs.getUtilidadTobon();

           

            con.Open();
            SqlCommand query = new SqlCommand("Insert into Catalogos (Tipo,Modelo,Marca,Talla,Color,Temporada,PrecioMaquila,PrecioProveedor,UtilidadEscuela,PrecioEscuela,Status,Descripcion,UtilidadJCTobon) values (@Tipo,@Modelo,@Marca,@Talla,@Color,@Temporada,@PrecioMaquila,@PrecioProveedor,@UtilidadEscuela,@PrecioEscuela,@Status,@Descripcion,@UtilidadJCTobon)", con);
            query.Parameters.AddWithValue("@Tipo", tipo);
            query.Parameters.AddWithValue("@Modelo", modelo);
            query.Parameters.AddWithValue("@Marca", marca);
            query.Parameters.AddWithValue("@Talla", talla);
            query.Parameters.AddWithValue("@Color", color);
            query.Parameters.AddWithValue("@Temporada", temporada);
            query.Parameters.AddWithValue("@PrecioMaquila", ma);
            query.Parameters.AddWithValue("@PrecioProveedor", pro);
            query.Parameters.AddWithValue("@UtilidadEscuela", ut);
            query.Parameters.AddWithValue("@PrecioEscuela", es);
            query.Parameters.AddWithValue("@Status", opcion);
            query.Parameters.AddWithValue("@Descripcion", descripcion);
            query.Parameters.AddWithValue("@UtilidadJCTobon", ujctobon);
            query.ExecuteNonQuery();
            cargarData();
            con.Close();
            Limpiar();
        }
        public void Limpiar()
        {
            textBoxtipo.Text = null;
            textBoxmodelo.Text = null;
            textBoxmarca.Text = null;
            textBoxtalla.Text = null;
            combotemporada.Text = null;
            textBoxcolor.Text = null;
            comboxstatus.Text = null;
            txtdescripcion.Text = null;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBoxtipo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBoxmodelo.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBoxmarca.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBoxtalla.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBoxcolor.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            combotemporada.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            txtdescripcion.Text = dataGridView1.CurrentRow.Cells[11].Value.ToString();
            comboxstatus.Text = dataGridView1.CurrentRow.Cells[13].Value.ToString();
            //comboBoxstatus.Text = dataGridView1.CurrentRow.Cells[11].Value.ToString();

            DataGridViewRow row = dataGridView1.CurrentRow;

            // Obtener la celda del checkbox
            DataGridViewCheckBoxCell checkBoxCell = row.Cells[13] as DataGridViewCheckBoxCell;

            // Verificar si la celda es un checkbox y tiene un valor booleano
            if (checkBoxCell != null && checkBoxCell.Value != DBNull.Value)
            {
                // Obtener el valor del checkbox
                //bool value = Convert.ToBoolean(checkBoxCell.Value);
                bool value = (bool)checkBoxCell.Value;

                // Imprimir un mensaje en función del valor del checkbox
                if (value == true)
                {
                    comboxstatus.Text = "Activo";
                    //MessageBox.Show("Esta seleccionado el check");

                }
                else if(value == false)
                {
                    comboxstatus.Text = "Inactivo";
                    //MessageBox.Show("NO Esta seleccionado el check");
                }

                
            }

             PreciosActualizar precios = new PreciosActualizar();
            precios.muestraPrecios(id);




        }

        private void button3_Click(object sender, EventArgs e)
        {
            int flag = 0;
            con.Open();
            SqlCommand query = new SqlCommand("delete from Catalogos Where Tipo = '" + textBoxtipo.Text + "'", con);
            flag = query.ExecuteNonQuery();

            if (flag == 1)
            {
                MessageBox.Show("Dato Eliminado del Sistema");
            }
            else
            {
                MessageBox.Show("Dato no Eliminado del Sistema");
            }
            con.Close();
            Limpiar();
            cargarData();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void textBoxmodelo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //    if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            //    {
            //        MessageBox.Show("Ingrese existencia valida (carácteres numericos)", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        e.Handled = true;
            //        return;
            //    }
        }

        private void textBoxtalla_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            //{
            //    MessageBox.Show("Ingrese existencia valida (carácteres numericos)", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    e.Handled = true;
            //    return;
            //}
        }

        public void validardatos()
        {
            var vr = !string.IsNullOrEmpty(textBoxtipo.Text) &&
                     !string.IsNullOrEmpty(textBoxmodelo.Text) &&
                     !string.IsNullOrEmpty(textBoxmarca.Text) &&
                     !string.IsNullOrEmpty(textBoxtalla.Text) &&
                     !string.IsNullOrEmpty(textBoxcolor.Text) &&
                     !string.IsNullOrEmpty(combotemporada.Text) &&
                     !string.IsNullOrEmpty(comboxstatus.Text)&& 
                     !string.IsNullOrEmpty(txtdescripcion.Text);
            button1.Enabled = vr;
            button2.Enabled = vr;
        }

        private void Form21_Load(object sender, EventArgs e)
        {
            SqlCommand query = new SqlCommand("SELECT Tipo FROM Catalogos", con);
            con.Open();
            SqlDataReader registro = query.ExecuteReader();

            comboTipo.Items.Add("Todos");

            while (registro.Read())
            {
                comboTipo.Items.Add(registro["Tipo"].ToString());

            }
            con.Close();


            button1.Enabled = false;
            button2.Enabled = false;

            dataGridView1.Columns["PrecioMaquila"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["PrecioProveedor"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadEscuela"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["PrecioEscuela"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["UtilidadJCTobon"].DefaultCellStyle.Format = "C";
            
        }

        private void textBoxtipo_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void textBoxmodelo_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void textBoxmarca_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void textBoxtalla_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void textBoxcolor_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void combotemporada_SelectedIndexChanged(object sender, EventArgs e)
        {
            validardatos();
        }

     

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBoxtipo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBoxmodelo.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBoxmarca.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBoxtalla.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBoxcolor.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            combotemporada.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            txtdescripcion.Text = dataGridView1.CurrentRow.Cells[11].Value.ToString();

            DataGridViewRow row = dataGridView1.CurrentRow;

            // Obtener la celda del checkbox
            DataGridViewCheckBoxCell checkBoxCell = row.Cells[13] as DataGridViewCheckBoxCell;

            // Verificar si la celda es un checkbox y tiene un valor booleano
            if (checkBoxCell != null && checkBoxCell.Value != DBNull.Value)
            {
                // Obtener el valor del checkbox
                //bool value = Convert.ToBoolean(checkBoxCell.Value);
                bool value = (bool)checkBoxCell.Value;


                // Imprimir un mensaje en función del valor del checkbox
                if (value == true)
                {
                    comboxstatus.Text = "Activo";
                    //MessageBox.Show("Esta seleccionado el check");

                }
                else if (value == false)
                {
                    comboxstatus.Text = "Inactivo";
                    //MessageBox.Show("NO Esta seleccionado el check");
                }

                if (e.RowIndex >= 0) // si se hizo clic en una fila
                {
                    button1.Visible = false; // ocultar el botón button1
                }

            }

            PreciosActualizar precios = new PreciosActualizar();
            precios.muestraPrecios(id);



        }

        private void comboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = comboTipo.Text;
            SqlDataAdapter sa = new SqlDataAdapter("buscarCatalogo", con);
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sa.SelectCommand.Parameters.Add("@tipo", SqlDbType.NVarChar, 150).Value = opcion;
            DataTable dt = new DataTable();
            sa.Fill(dt);
            this.dataGridView1.DataSource = dt;

            if (opcion.Equals("Todos"))
            {
                cargarData();
            }

        }

        private void txtdescripcion_TextChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void comboxstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            validardatos();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            PreciosActualizar obs = new PreciosActualizar();
            double ma = obs.getMaquila();
            double pro = obs.getProveedor();
            double ut = obs.getUtilidad();
            double es = obs.getEscuela();
            double ujctobon = obs.getUtilidadTobon();

            string tipo = textBoxtipo.Text;

            // actualizamos en catalogo 
            int flag = 0;

            string opcion = comboxstatus.Text;
            int status;

            if (opcion.Equals("Activo"))
            {
                status = 1;
            }
            else
            {
                status = 0;
            }
          
            con.Open();
            SqlCommand query = new SqlCommand("Update Catalogos set Tipo = @tipo, Modelo = @modelo, Marca = @marca, Talla = @talla, Color = @color,Temporada = @temporada, PrecioMaquila = @precioMaquila,PrecioProveedor = @precioProveedor,UtilidadEscuela = @utilidadescuela,PrecioEscuela = @precioescuela,Status = @status,Descripcion = @descripcion, UtilidadJCTobon = @utilidadtobon Where ID = '" + id + "' ", con);
            query.Parameters.AddWithValue("@tipo", textBoxtipo.Text);
            query.Parameters.AddWithValue("@modelo", textBoxmodelo.Text);
            query.Parameters.AddWithValue("@marca", textBoxmarca.Text);
            query.Parameters.AddWithValue("@talla", textBoxtalla.Text);
            query.Parameters.AddWithValue("@color", textBoxcolor.Text);
            query.Parameters.AddWithValue("@temporada",combotemporada.Text );
            query.Parameters.AddWithValue("@precioMaquila", ma);
            query.Parameters.AddWithValue("@precioProveedor", pro);
            query.Parameters.AddWithValue("@utilidadescuela", ut);
            query.Parameters.AddWithValue("@precioescuela", es);
            query.Parameters.AddWithValue("@status", status);
            query.Parameters.AddWithValue("@descripcion", txtdescripcion.Text);
            query.Parameters.AddWithValue("@utilidadtobon", ujctobon);

            flag = query.ExecuteNonQuery();

            if (flag == 1)
            {
                MessageBox.Show("Dato Actualizado en el Sistema");
                con.Close();
                Limpiar();
                cargarData();
               
            }
            else
            {
                MessageBox.Show("Dato no Actualizado en el Sistema");
            }

            con.Close();

            // actualizamos en productos 

            con.Open();
            SqlCommand query2 = new SqlCommand("Update Productos set PrecioVenta = '" + es + "', UtilidadEscuela = '" + ut + "', UtilidadJCTobon = '" + ujctobon + "' where Tipo =  '" + tipo + "' ", con);
            query2.ExecuteNonQuery();
            con.Close();


        }

        private void button6_Click(object sender, EventArgs e)
        {
            PreciosActualizar act = new PreciosActualizar();
            act.muestraPrecios(id);
            act.Show();
        }

        
        

    }
}
