﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace JCTobon.Forms
{
    public partial class Form20 : Form
    {
        public Form20()
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        //SqlConnection con = new SqlConnection("Data Source=LAPTOP-OM95FUOE\\SQLEXPRESS;Initial Catalog=PuntoVentaJCTobon;Integrated Security=True");
        SqlConnection con = new SqlConnection("Data Source=sqlpuntoventa.cjl3v0f7izez.us-east-2.rds.amazonaws.com;Initial Catalog=PuntoVenta;User ID=admin;Password=admin007");
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form8 export = new Form8();
            DataGridView data = export.getData();

            

            string combobox = comboBox1.Text;

            if (combobox.Equals("EXCEL"))
            {
                MessageBox.Show("Archivo exportado a Excel ", "Aviso informativo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox1.Text = null;
                export.ExportarExcel(data);


            }

            else
            {
                export.PDF(data);
                MessageBox.Show("Archivo exportado a PDF ", "Aviso informativo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox1.Text = null;

            }
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form19 export = new Form19();
            Form8 exportar = new Form8();
            DataGridView data = export.getData();

            string combobox = comboBox2.Text;
            if (combobox.Equals("EXCEL"))
            {
                MessageBox.Show("Archivo exportado a Excel ", "Aviso informativo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox1.Text = null;
                exportar.ExportarExcel(data);
                comboBox2.Text = null;


            }

            else if(combobox.Equals("VISTAS PREVIAS"))
            {
                Form19 abrir = new Form19();
                abrir.Show();
            }

            else
            {
                //export.exportarPDF();
                //MessageBox.Show("Archivo exportado a PDF ", "Aviso informativo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                VentaValidada abrir = new VentaValidada();
                abrir.Show();
                comboBox2.Text = null;

            }

          


        }

        public void validar1()
        {
            var vr = !string.IsNullOrEmpty(comboBox1.Text);
            button1.Enabled = vr;

        }
        
        public void validar2()
        {
            var vr = !string.IsNullOrEmpty(comboBox2.Text);
            button2.Enabled = vr;
        }

        private void Form20_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            validar1();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            validar2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form23 abrir = new Form23();
            abrir.Show(); 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            VentaValidada abrir = new VentaValidada();
            abrir.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form26 abrir = new Form26();
            abrir.Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
